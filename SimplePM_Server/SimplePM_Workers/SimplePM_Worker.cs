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

//Основа
using System;
//Подключаем коллекции
using System.Collections.Generic;
using System.Diagnostics;
//Работа с текстом
using System.Text;
//Многопоточность
using System.Threading;
using System.Threading.Tasks;
//Подключение к MySQL серверу
using MySql.Data.MySqlClient;
//Парсер конфигурационного файла
using IniParser;
using IniParser.Model;
//Для безопасности
using System.Web;
//ICompilerPlugin
using CompilerBase;
//Журнал событий
using NLog;
using NLog.Config;
// Работа с файловой системой
using System.IO;
// Использование запросов
using System.Reflection;

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
         * Объявляем переменную указателя на менеджер журнала собылий
         * и присваиваем ей указатель на журнал событий текущего класса
         */
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        private ulong _customersCount;    // Количество текущих обрабатываемых запросов
        private ulong _maxCustomersCount; // Максимальное количество обрабатываемых запросов
        
        public IniData sConfig;          // Дескриптор конфигурационного файла
        public IniData sCompilersConfig; // Дескриптор конфигурационного файла модулей компиляции
        
        private int SleepTime = 500; // Период ожидания
        private string EnabledLangs; // Список поддерживаемых ЯП для SQL запросов

        public List<ICompilerPlugin> _compilerPlugins; // Список, содержащий ссылки на модули компиляторов
        
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

            /*
             * Учитывая тот факт, что каждый раздел  в  конфигурационном
             * файле сторонних модулей компиляции определяет собственных
             * модуль компиляции, осуществляем перебор всех секций этого
             * конфигурационного  файла  и  загружаем найденные  модули,
             * которые подпадают под все требования и условия поиска.
             */
            foreach (var section in sCompilersConfig.Sections)
            {

                /*
                 * Формируем полный путь к
                 * предполагаемому  модулю
                 * компиляции.
                 */
                var compilerPluginPath = Path.Combine(
                    sConfig["Program"]["ICompilerPlugin_directory"],
                    "ICompilerPlugin." + section.SectionName + ".dll"
                );

                // Проверка на существование и доступность модуля
                if (section.Keys["Enabled"] != "true" || !File.Exists(compilerPluginPath))
                    continue;

                /*
                 * Указываем в логе, что начинаем
                 * загружать  определённый модуль
                 * компиляции.
                 */
                logger.Debug("Start loading plugin [" + compilerPluginPath + "]...");

                try
                {

                    // Загружаем сборку из файла по указанному пути
                    var assembly = Assembly.LoadFrom(compilerPluginPath);

                    // Ищем необходимую для нас реализацию интерфейса
                    foreach (var type in assembly.GetTypes())
                    {

                        /*
                         * Если мы нашли то, что
                         * искали  -   добавляем
                         * плагин в список.
                         */
                        if (type.FullName == "CompilerPlugin.Compiler")
                            _compilerPlugins.Add((ICompilerPlugin)Activator.CreateInstance(type));

                    }

                }
                catch (Exception ex)
                {

                    /*
                     * В случае возникновения ошибок записываем
                     * информацию о них в лог-файле
                     */
                    logger.Debug(ex);

                }

            }

            /*
             * Записываем в лог-файл информацию о том,
             * что мы завершили процесс загрузки всех
             * модулей компиляции (ну или не всех)
             **/
            logger.Debug("ICompilerPlugin modules were loaded...");

        }
        
        /*
         * Функция генерирует  строку из допустимых для
         * проверки языков программирования, на которых
         * написаны пользовательские программы.
         */
        public void GenerateEnabledLangsList()
        {

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
            foreach (var compilerPlugin in _compilerPlugins)
            {

                // Добавляем язык программирования в список
                EnabledLangsList.Add("'" + compilerPlugin.CompilerPluginLanguageName + "'");

            }

            /*
             * Формируем список доступных языков
             */
            EnabledLangs = string.Join(", ", EnabledLangsList);
            
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
            AppDomain.CurrentDomain.UnhandledException += ExceptionEventLogger;

        }
        
        /*
         * Функция, вызывающаяся при получении информации
         * о необработанном фатальном исключении.
         */
        private void ExceptionEventLogger(object sender, UnhandledExceptionEventArgs e)
        {

            /*
             * Записываем сообщение об ошибке в журнал событий
             */
            logger.Fatal(e.ExceptionObject);

        }
        
        /*
         * Функция,    очищающая    директорию   хранения
         * временных  файлов  сервера  проверки  решений.
         * В оснвном используется лишь при запуске нового
         * экземпляра сервера  для  избежания конфликтов.
         */
        public bool CleanTempDirectory()
        {

            /*
             * Объявляем переменную, которая будет
             * хранить информацию о том, успешна
             * ли очистка временных файлов или нет.
             */
            var f = true;
            
            try
            {

                /* Удаляем все файлы в папке */
                foreach (string file in Directory.GetFiles(sConfig["Program"]["temp_path"]))
                    File.Delete(file);

                /* Удаляем все директории в папке */
                foreach (string dir in Directory.GetDirectories(sConfig["Program"]["temp_path"]))
                    Directory.Delete(dir, true);

            }
            catch
            {

                /* Указываем, что очистка произведена с ошибкой */
                f = false;

            }

            // Возвращаем результат выполнения операции
            return f;

        }
        
        /*
         * Функция, инициализирующая все необходимые
         * переменные и прочий  хлам для возможности
         * работы сервера проверки решений
         */
        private void LoadResources(string[] args)
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

            // Открываем конфигурационный файл для чтения
            var iniParser = new FileIniDataParser();

            // Присваиваем глобальной переменной sConfig дескриптор файла конфигурации
            sConfig = iniParser.ReadFile("server_config.ini", Encoding.UTF8);

            /*
             * Присваиваем глобальной переменной
             * sCompilersConfig дескриптор файла
             * конфигурации модулей компиляции.
             */
            sCompilersConfig = iniParser.ReadFile("ICompilerPlugin.ini", Encoding.UTF8);

            // Конфигурируем журнал событий (библиотека NLog)
            try
            {

                LogManager.Configuration = new XmlLoggingConfiguration(
                    sConfig["Program"]["NLogConfig_path"]
                );

            }
            catch
            {
                /* Deal with it */
            }

            /*
             * Получаем информацию с конфигурационного
             * файла для некоторых переменных.
             */
            _maxCustomersCount = ulong.Parse(sConfig["Connection"]["max_connected_clients"]);
            SleepTime = int.Parse(sConfig["Connection"]["check_timeout"]);

            ///////////////////////////////////////////////////
            // Загрузка в память информации о плагинах сервера
            // проверки пользовательских решений задач
            ///////////////////////////////////////////////////

            // Модули компиляторов
            LoadCompilerPlugins();

            // Модули сервера
            //TODO:LoadServerPlugins();

            /*
             * Вызываем функцию получения строчного списка
             * поддерживаемых языков программирования.
             */
            GenerateEnabledLangsList();

            ///////////////////////////////////////////////////
            // Вызов метода обработки аргументов запуска
            // консольного приложения
            ///////////////////////////////////////////////////

            new SimplePM_Commander().SplitArguments(args);

        }
        
        /*
         * Функция,  которая  запускает  соновной  цикл
         * сервера проверки решений. Работает постоянно
         * в высшем родительском потоке.
         */
        private void ServerLoop()
        {
            

            uint rechecksCount = 0; // количество перепроверок без ожидания

#if DEBUG
            Console.WriteLine(EnabledLangs);
#endif

            /*
             * В бесконечном цикле опрашиваем базу данных
             * на наличие новых не обработанных  запросов
             * на тестирование решений задач.
             */
            while (true)
            {

#if DEBUG
                Console.WriteLine(_customersCount + "/" + _maxCustomersCount);
#endif

                if (_customersCount < _maxCustomersCount)
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
                        var conn = StartMysqlConnection(sConfig);

                        //Вызов чекера (если всё "хорошо")
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
                    sConfig["Connection"]["rechecks_without_timeout"]
                );

                if (_customersCount < _maxCustomersCount && tmpCheck)
                {

                    // Ожидание для уменьшения нагрузки на сервер
                    Thread.Sleep(SleepTime);

                    // Обнуляем итератор
                    rechecksCount = 0;

                }
                else
                    rechecksCount++;

            }

            ///////////////////////////////////////////////////
            
        }
        
        /*
         * Точка входа с автозапуском бесконечного цикла
         */
        public void Run(string[] args)
        {
            
            // Загружаем все необходимые ресурсы
            LoadResources(args);

            // Запускаем основной цикл
            ServerLoop();
            
        }
        
        /*
         * Функция обработки запросов на проверку решений
         */
        public void GetSubIdAndRunCompile(MySqlConnection conn)
        {
            
            // Создаём новую задачу, без неё - никак!
            new Task(() =>
            {
                
                // Формируем запрос на выборку
                var querySelect = $@"
                    SELECT 
                        * 
                    FROM 
                        `spm_submissions` 
                    WHERE 
                        `status` = 'waiting' 
                    AND 
                        `codeLang` IN ({EnabledLangs})
                    ORDER BY 
                        `submissionId` ASC 
                    LIMIT 
                        1
                    ;
                ";
                
                // Создаём запрос на выборку из базы данных
                var cmdSelect = new MySqlCommand(querySelect, conn);

                // Производим выборку полученных результатов из временной таблицы
                var dataReader = cmdSelect.ExecuteReader();

                // Объявляем временную переменную, так называемый "флаг"
                bool f;

                // Делаем различные проверки в безопасном контексте
                lock (new object())
                {

                    f = _customersCount >= _maxCustomersCount | !dataReader.Read();

                }

                // Проверка на пустоту полученного результата
                if (f)
                {

                    // Закрываем чтение пустой временной таблицы
                    dataReader.Close();

                    // Закрываем соединение с БД
                    conn.Close();

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

                        _customersCount++;

                    }

                    /*
                     * Объявляем объект, который будет хранить
                     * всю информацию об отправке и записываем
                     * в него только что полученные данные.
                     */
                    var submissionInfo = new SubmissionInfo.SubmissionInfo
                    (
                        int.Parse(dataReader["submissionId"].ToString()),
                        int.Parse(dataReader["userId"].ToString()),
                        int.Parse(dataReader["problemId"].ToString()),
                        int.Parse(dataReader["classworkId"].ToString()),
                        int.Parse(dataReader["olympId"].ToString()),
                        dataReader["codeLang"].ToString(),
                        dataReader["testType"].ToString(),
                        HttpUtility.HtmlDecode(dataReader["customTest"].ToString()),
                        (byte[])dataReader["problemCode"]
                    );

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

                    // Получаем сложность поставленной задачи
                    var queryGetDifficulty = $@"
                        SELECT 
                            `difficulty` 
                        FROM 
                            `spm_problems` 
                        WHERE 
                            `id` = '{submissionInfo.ProblemId}' 
                        LIMIT 
                            1
                        ;
                    ";

                    var cmdGetProblemDifficulty = new MySqlCommand(queryGetDifficulty, conn);

                    submissionInfo.ProblemDifficulty = int.Parse(
                        cmdGetProblemDifficulty.ExecuteScalar().ToString()
                    );

                    /*
                     * Зовём официанта-шляпочника
                     * уж он знает, что делать в таких
                     * вот неожиданных ситуациях
                     */
                    new SimplePM_Officiant(
                        conn,
                        ref sConfig,
                        ref sCompilersConfig,
                        ref _compilerPlugins,
                        submissionInfo
                    ).ServeSubmission();

                    /*
                     * Уменьшаем количество текущих соединений
                     * чтобы другие соединения были возможны.
                     */
                    lock (new object())
                    {
                        _customersCount--;
                    }

                    /*
                     * Останавливаем секундомер и записываем
                     * полученное значение в Debug log поток
                     */
                    sw.Stop();

#if DEBUG
                    // Выводим затраченное время на экран
                    Console.WriteLine(sw.ElapsedMilliseconds);
#endif

                    // Закрываем соединение с БД
                    conn.Close();

                }

            }).Start();

        }

        ///////////////////////////////////////////////////
        /// Функция инициализирует соединение с базой
        /// данных MySQL используя данные аутенфикации,
        /// расположенные в конфигурационном файле сервера
        /// /return Указатель на соединение с БД
        ///////////////////////////////////////////////////

        public MySqlConnection StartMysqlConnection(IniData sConfig)
        {

            // Объявляем переменную, которая будет хранить
            // дескриптор соединения с базой данных системы.
            MySqlConnection db = null;

            try
            {

                // Подключаемся к базе данных на удалённом
                // MySQL сервере и получаем дескриптор подключения к ней
                db = new MySqlConnection(
                    $@"
                        server={sConfig["Database"]["db_host"]};
                        uid={sConfig["Database"]["db_user"]};
                        pwd={sConfig["Database"]["db_pass"]};
                        database={sConfig["Database"]["db_name"]};
                        Charset={sConfig["Database"]["db_chst"]};
                        SslMode=Preferred;
                        Pooling=False;
                    "
                );

                // Открываем соединение с БД
                db.Open();

            }
            catch (MySqlException ex)
            {
                
                Console.WriteLine(ex);
                /* Deal with it */

            }

            //Возвращаем дескриптор подключения к базе данных
            return db;

        }

        ///////////////////////////////////////////////////

    }

}
