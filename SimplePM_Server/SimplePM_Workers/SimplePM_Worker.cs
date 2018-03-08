/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

using NLog;
using System;
using System.IO;
using NLog.Config;
using CompilerBase;
using SubmissionInfo;
using Newtonsoft.Json;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace SimplePM_Server
{

    /*
     * Базовый класс сервера контролирует
     * работу сервера  проверки решений в
     * целом,      содержит     множество
     * инициализирующих что-либо функций,
     * множество переменных и т.д.
     */

    public class SimplePM_Worker
    {
        
        /*
         * Объявляем переменную указателя
         * на менеджер  журнала собылий и
         * присваиваем  ей  указатель  на
         * журнал событий текущего класса
         */
        private readonly Logger logger = LogManager.GetLogger("SimplePM_Worker");

        private dynamic _serverConfiguration; // переменная хранит основную конфигурацию сервера
        private dynamic _compilerConfigurations; // переменная хранит конфигурацию модулей компиляции

        private ulong _aliveTestersCount; // Количество текущих обрабатываемых запросов
        
        private string EnabledLangs; // Список поддерживаемых сервером ЯП для SQL запросов
        private List<ICompilerPlugin> _compilerPlugins; // Список, содержащий ссылки на модули компиляторов
        
        /*
         * Функция загружает в память компиляционные
         * модули,  которые  собирает  из специально
         * заготовленной директории.
         */

        private void LoadCompilerPlugins()
        {

            /*
             * Записываем в лог-файл информацию о том,
             * что  собираемся   подгружать  сторонние
             * модули компиляции.
             */

            logger.Debug("ICompilerPlugin modules are being loaded...");

            /*
             * Проводим инициализацию необходимых
             * для продолжения работы переменных.
             */
            _compilerPlugins = new List<ICompilerPlugin>();

            var pluginFilesList = Directory.GetFiles(
                (string)_serverConfiguration.path.ICompilerPlugin,
                "ICompilerPlugin.*.dll"
            );

            foreach (var pluginFilePath in pluginFilesList)
            {
                
                /*
                 * Указываем в логе, что начинаем
                 * загружать  определённый модуль
                 * компиляции.
                 */
                logger.Debug("Start loading plugin [" + pluginFilePath + "]...");

                try
                {

                    /*
                     * Загружаем сборку из файла по указанному пути
                     */
                    var assembly = Assembly.LoadFrom(pluginFilePath);

                    /*
                     * Ищем необходимую для нас реализацию интерфейса
                     */
                    foreach (var type in assembly.GetTypes())
                    {

                        /*
                         * Если мы не нашли то, что искали - переходим
                         * к следующей итерации цикла foreach,  в ином
                         * случае  продолжаем  выполнение  необходимых
                         * действий по добавлению плагина в список.
                         */

                        if (type.FullName != "CompilerPlugin.Compiler") continue;
                        
                        // Добавляем плагин в список
                        _compilerPlugins.Add(
                            (ICompilerPlugin)Activator.CreateInstance(type)
                        );

                        logger.Debug("Plugin successfully loaded [" + pluginFilePath + "]");

                        // Выходим из цикла foreach
                        break;

                    }

                }
                catch (Exception ex)
                {

                    /*
                     * В случае возникновения ошибок
                     * записываем информацию о них в
                     * лог-файле.
                     */

                    logger.Debug("Plugin loading failed [" + pluginFilePath + "]:");
                    logger.Debug(ex);

                }

            }

            /*
             * Записываем в лог-файл информацию о том,
             * что мы завершили процесс загрузки всех
             * модулей компиляции (ну или не всех)
             */

            logger.Debug("ICompilerPlugin modules were loaded...");

        }
        
        /*
         * Функция генерирует  строку из допустимых для
         * проверки языков программирования, на которых
         * написаны пользовательские программы.
         */

        private void GenerateEnabledLangsList()
        {

            /*
             * Записываем  в  лог, что генерация списка
             * доступных   на   этом  сервере  проверки
             * пользовательских   решений    задач   по
             * программированию   скриптовых  языков  а
             * также языков программирования начинается
             * с этого великолепного момента.
             */

            logger.Debug("Generation of enabled programming languages list started.");

            /*
             * Инициализируем список строк и собираем
             * поддерживаемые языки программирования в массив
             */

            var EnabledLangsList = new List<string>();
            
            /*
             * В цикле перебираем все поддерживаемые языки
             * программирования подключаемыми модулями и
             * приводим список поддерживаемых системой
             * языков к требуемому виду.
             */

            foreach (var compilerConfig in _compilerConfigurations)
            {

                // Добавляем в список только включённые конфигурации
                if (compilerConfig.enabled != "true") continue;

                // Добавляем текущую конфигурацию в список
                EnabledLangsList.Add("'" + (string)compilerConfig.language_name + "'");

                // Записываем информацию об этом в лог-файл
                logger.Debug(
                    "Compiler configuration '" +
                    (string)compilerConfig.language_name +
                    "' loaded!"
                );

            }

            /*
             * Формируем список доступных языков в виде строки
             */

            EnabledLangs = string.Join(", ", EnabledLangsList);
            
            /*
             * Выводим список доступных языков программирования
             * на данной сессии работы сервера в лог-файл.  Это
             * возможно  поможет  системному  администратору  с
             * решением  проблем,   связанных  с  подключаемыми
             * модулями и другими  изменяемыми частями  сервера
             * проверки решений SimplePM Server.
             */
            
            logger.Debug("Enabled compiler configurations list: " + EnabledLangs);

        }
        
        /*
         * Функция устанавливает "улавливатель"
         * непредвиденных исключений.
         */

        private void SetExceptionHandler()
        {

            /*
             * На всякий случай создаём директорию
             * для хранения лог-файлов, так как
             * некоторые версии NLog не создают
             * её автоматически.
             */
            
            try
            {

                Directory.CreateDirectory("./log/");

            }
            catch
            {
                /* Deal with it */
            }

            /* Устанавливаем обработчик необработанных исключений */
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {

                /*
                 * Записываем сообщение об
                 * ошибке в журнал событий
                 */
                logger.Fatal(e.ExceptionObject);

            };

        }
        
        /*
         * Функция,    очищающая    директорию   хранения
         * временных  файлов  сервера  проверки  решений.
         * В оснвном используется лишь при запуске нового
         * экземпляра сервера  для  избежания конфликтов.
         */

        private void CleanTempDirectory()
        {
            
            try
            {

                /* Удаляем все файлы в папке */
                foreach (var file in Directory.GetFiles((string)(_serverConfiguration.path.temp)))
                    File.Delete(file);

                /* Удаляем все директории в папке */
                foreach (var dir in Directory.GetDirectories((string)(_serverConfiguration.path.temp)))
                    Directory.Delete(dir, true);

            }
            catch(Exception ex)
            {
                
                // Записываем информацию об исключении в лог-файл
                logger.Warn("An error occured while trying to clear temp folder: " + ex);

            }
            
        }
        
        /*
         * Функция, инициализирующая все необходимые
         * переменные и прочий  хлам для возможности
         * работы сервера проверки решений
         */

        private void LoadResources()
        {
            
            /*
             * Производим инициализацию
             * всех подсистем и модулей
             * сервера проверки решений
             */

            // Устанавливаем "улавливатель исключений"
            SetExceptionHandler();

            // Очищаем директорию временных файлов
            CleanTempDirectory();
            
            // Загружаем конфигурацию сервера в память
            _serverConfiguration = JsonConvert.DeserializeObject(
                File.ReadAllText("./config/server.json")
            );
            
            // Загружаем конфигурации модулей компиляции в память
            _compilerConfigurations = JsonConvert.DeserializeObject(
                File.ReadAllText("./config/compilers.json")
            );
            
            // Конфигурируем журнал событий (библиотека NLog)
            try
            {

                LogManager.Configuration = new XmlLoggingConfiguration(
                    "./NLog.config"
                );

            }
            catch
            {
                /* Deal with it */
            }

            /*
             * В случае запроса на автоматическое
             * выставление лимитов по обработке
             * запросов на тестирование, необходимо
             * их всё-таки выставить. Будем считать,
             * что максимально возможное число
             * одновременных проверок будет равняться
             * общему числу логических процессоров на
             * данной машине Тьюринга.
             */

            if (_serverConfiguration.submission.rechecks_without_timeout == "auto")
                _serverConfiguration.submission.rechecks_without_timeout = Environment.ProcessorCount.ToString();

            if (_serverConfiguration.submission.max_threads == "auto")
                _serverConfiguration.submission.max_threads = Environment.ProcessorCount.ToString();

            /*
             * Вызываем метод, который загружает
             * модули компиляции в память.
             */
            LoadCompilerPlugins();

            /*
             * Вызываем метод, который загружает
             * плагины сервера в память.
             */
            //TODO:LoadServerPlugins();

            /*
             * Вызываем функцию получения строчного списка
             * поддерживаемых языков программирования. Это
             * необходимо для формирования соответствующих
             * SQL-запросов  к  системе  управления базами
             * данных, а также для выборки тех запросов на
             * тестирование, которые соответствуют возмож-
             * ностям текущего экземпляра сервера проверки
             * пользовательских решений поставленных задач
             */
            GenerateEnabledLangsList();
            
        }
        
        /*
         * Функция,  которая  запускает  соновной  цикл
         * сервера проверки решений. Работает постоянно
         * в высшем родительском потоке.
         */

        private void ServerLoop()
        {
            
            /*
             * Объявляем переменную,  которая хранит количество
             * произведенных проверок на наличие необработанных
             * запросов  на  тестирование,   после  которых  не
             * производилась задержка.
             */

            uint rechecksCount = 0;
            
            /*
             * В бесконечном цикле опрашиваем базу данных
             * на наличие новых не обработанных  запросов
             * на тестирование решений задач.
             */

            while (true)
            {
                
                if (_aliveTestersCount < (ulong)_serverConfiguration.submission.max_threads)
                {

                    /*
                     * Действия  необходимо   выполнять  в  блоке
                     * обработки    непредвиденных    исключений,
                     * так   как   при   выполнении   операций  с
                     * удалённой  базой  данных  могут  возникать
                     * непредвиденные ошибки,   которые не должны
                     * повлиять   на    общую    стабильность   и
                     * работоспособность сервер проверки решений.
                     */

                    try
                    {

                        /*
                         * Инициализируем   новое   уникальное
                         * соединение с базой данных для того,
                         * чтобы не мешать остальным потокам.
                         */

                        var conn = StartMysqlConnection();

                        /*
                         * В случае успешного подключения к
                         * базе данных SimplePM Server,
                         * вызываем метод, который занимается
                         * поиском и дальнейшей обработкой
                         * пользовательских запросов на тестирование.
                         */

                        if (conn != null)
                            GetSubIdAndRunCompile(conn);
                        
                    }

                    /*
                     * В случае  обнаружения  каких-либо
                     * ошибок, записываем их в лог-файл.
                     */

                    catch (Exception ex)
                    {

                        // Записываем все исключения как ошибки
                        logger.Error(ex);

                    }

                }

                /*
                 * Проверяем, необходимо ли установить
                 * таймаут для ослабления  нагрузки на
                 * процессор, или нет.
                 */

                var tmpCheck = rechecksCount >= uint.Parse(
                    (string)_serverConfiguration.submission.rechecks_without_timeout
                );

                if (_aliveTestersCount < (ulong)_serverConfiguration.submission.max_threads && tmpCheck)
                {

                    // Ожидание для уменьшения нагрузки на сервер
                    Thread.Sleep((int)_serverConfiguration.submission.check_timeout);

                    // Обнуляем итератор
                    rechecksCount = 0;

                }
                else
                    rechecksCount++;

            }
            
            // ReSharper disable once FunctionNeverReturns

        }
        
        /*
         * Точка входа с автозапуском бесконечного цикла
         */

        public void Run(string[] args)
        {
            
            // Загружаем все необходимые ресурсы
            LoadResources();

            // Запускаем основной цикл
            ServerLoop();
            
        }
        
        /*
         * Функция обработки запросов на проверку решений
         */

        private void GetSubIdAndRunCompile(MySqlConnection conn)
        {
            
            // Создаём новую задачу, без неё - никак!
            new Task(() =>
            {

                try
                {

                    // Записываем в лог информацию о событии
                    logger.Trace(
                        "Starting submission query; Running threads: " +
                        _aliveTestersCount + " from " +
                        (ulong)_serverConfiguration.submission.max_threads
                    );
                    
                    /*
                     * Создаём новый запрос к базе данных на
                     * выборку из неё информации  о  запросе
                     * на тестирование.
                     */

                    var sqlCmd = new MySqlCommand(
                        Properties.Resources.submission_query.Replace(
                            "@EnabledLanguages",
                            EnabledLangs
                        ),
                        conn
                    );

                    // Добавляем в запрос требуемые параметры
                    sqlCmd.Parameters.AddWithValue("@EnabledLanguages", EnabledLangs);

                    // Выполняем запрос к БД и получаем ответ
                    var dataReader = sqlCmd.ExecuteReader();

                    // Объявляем временную переменную, так называемый "флаг"
                    bool f;

                    // Делаем различные проверки в безопасном контексте
                    lock (new object())
                    {

                        f = _aliveTestersCount >= (ulong)_serverConfiguration.submission.max_threads | !dataReader.Read();

                    }

                    // Проверка на пустоту полученного результата
                    if (f)
                    {

                        // Закрываем чтение пустой временной таблицы
                        dataReader.Close();

                        // Закрываем соединение с БД
                        conn.Close();

                        // Очищаем не управляемую память
                        conn.Dispose();

                    }
                    else
                    {

                        /* 
                         * Запускаем   секундомер  для  того,
                         * чтобы определить время, за которое
                         * запрос на проверку  обрабатывается
                         * сервером проверки решений задач.
                         */
                        var sw = Stopwatch.StartNew();

                        // Увеличиваем количество текущих соединений
                        lock (new object())
                        {

                            _aliveTestersCount++;

                        }

                        /*
                         * Объявляем объект, который будет хранить
                         * всю информацию об отправке и записываем
                         * в него только что полученные данные.
                         */
                        var submissionInfo = new SubmissionInfo.SubmissionInfo
                        {

                            /*
                             * Основная информация о запросе
                             */
                            SubmissionId = int.Parse(dataReader["submissionId"].ToString()),
                            UserId = int.Parse(dataReader["userId"].ToString()),

                            /*
                             * Привязка к уроку и соревнованию
                             */
                            ClassworkId = int.Parse(dataReader["classworkId"].ToString()),
                            OlympId = int.Parse(dataReader["olympId"].ToString()),

                            /*
                             * Тип тестирования и доплнительные поля
                             */
                            TestType = dataReader["testType"].ToString(),
                            CustomTest = (byte[])dataReader["customTest"],

                            /*
                             * Исходный код решения задачи
                             * и дополнительная информация
                             * о нём.
                             */
                            ProblemCode = (byte[])dataReader["problemCode"],
                            CodeLang = dataReader["codeLang"].ToString(),

                            /*
                             * Информация о задаче
                             */
                            ProblemInformation = new ProblemInfo
                            {

                                ProblemId = int.Parse(dataReader["problemId"].ToString()),
                                ProblemDifficulty = int.Parse(dataReader["difficulty"].ToString()),
                                AdaptProgramOutput = bool.Parse(dataReader["adaptProgramOutput"].ToString()),

                                AuthorSolutionCode = (byte[])dataReader["authorSolution"],
                                AuthorSolutionCodeLanguage = dataReader["authorSolutionLanguage"].ToString()

                            }

                        };

                        // Закрываем чтение временной таблицы
                        dataReader.Close();

                        // Устанавливаем статус запроса на "в обработке"
                        var queryUpdate = $@"
                            UPDATE 
                                `spm_submissions` 
                            SET 
                                `status` = 'processing' 
                            WHERE 
                                `submissionId` = '{submissionInfo.SubmissionId}'
                            LIMIT 
                                1
                            ;
                        ";

                        // Выполняем запрос к базе данных
                        new MySqlCommand(queryUpdate, conn).ExecuteNonQuery();

                        /*
                         * Зовём официанта-шляпочника
                         * уж он знает, что делать в таких
                         * вот неожиданных ситуациях.
                         */

                        new SimplePM_Officiant(
                            conn,
                            ref _serverConfiguration,
                            ref _compilerConfigurations,
                            ref _compilerPlugins,
                            submissionInfo
                        ).ServeSubmission();

                        /*
                         * Уменьшаем количество текущих соединений
                         * чтобы другие соединения были возможны.
                         */

                        lock (new object())
                        {
                            _aliveTestersCount--;
                        }

                        /*
                         * Останавливаем секундомер и записываем
                         * полученное значение в Debug log поток
                         */

                        sw.Stop();

                        // Выводим затраченное время на экран
                        logger.Trace("Submission checking time (ms): " + sw.ElapsedMilliseconds);

                        // Закрываем соединение с БД
                        conn.Close();

                        // Очищаем не управляемую память
                        conn.Dispose();

                    }

                }
                catch (Exception ex)
                {

                    /*
                     * Записываем информацию об ошибке в лог-файл
                     */

                    logger.Error(ex);

                    /*
                     * Пытаемся закрыть соединение с БД
                     */

                    try
                    {

                        // Закрываем соединение с БД
                        conn.Close();

                        // Очищаем не управляемую память
                        conn.Dispose();

                    }
                    catch
                    {
                        /* Никаких действий не предвидится */
                    }

                }

            }).Start();

        }
        
        /*
         * Функция   инициализирует   соединение  с  базой
         * данных  MySQL  используя  данные  аутенфикации,
         * расположенные в конфигурационном файле сервера.
         */

        private MySqlConnection StartMysqlConnection()
        {

            /*
             * Объявляем  переменную, которая  будет хранить
             * дескриптор соединения с базой данных системы.
             */

            MySqlConnection db = null;

            /*
             * Объявляем переменную,  которая  будет
             * хранить конфигурацию подключения к БД
             */

            dynamic databaseConfig;

            /*
             * Потоконебезопасные  действия
             * необходимо синхронизировать.
             */

            lock (new object())
            {

                /*
                 * Получаем информацию о конфигурации подключения
                 * к базе данных комплекса программных продуктов
                 * АСПРЗП "SimplePM".
                 */

                databaseConfig = JsonConvert.DeserializeObject(
                    File.ReadAllText("./config/database.json")
                );

            }
            
            try
            {

                /*
                 * Подключаемся к базе данных на удалённом
                 * MySQL  сервере  и  получаем  дескриптор
                 * подключения к ней.
                 */
                db = new MySqlConnection(
                    $@"
                        server={databaseConfig.hostname};
                        uid={databaseConfig.username};
                        pwd={databaseConfig.password};
                        database={databaseConfig.basename};
                        Charset={databaseConfig.mainchst};
                        SslMode=Preferred;
                        Pooling=False;
                    "
                );

                // Открываем соединение с БД
                db.Open();

            }
            catch (MySqlException ex)
            {
                
                // Записываем информацию об ошибке в лог-файл
                logger.Warn("An error occured while trying to connect to MySQL server: " + ex);

            }

            /*
             * Возвращаем дескриптор
             * подключения к БД.
             */

            return db;

        }
        
    }

}
