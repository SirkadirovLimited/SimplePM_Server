/*
 * Copyright (C) 2018, Yurij Kadirov.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Yurij Kadirov
 * @Website: https://spm.sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

using NLog;
using System;
using System.IO;
using NLog.Config;
using CompilerBase;
using Newtonsoft.Json;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using JudgeBase;

namespace SimplePM_Server
{

    /*
     * Базовый класс сервера контролирует
     * работу сервера  проверки решений в
     * целом,      содержит     множество
     * инициализирующих что-либо функций,
     * множество переменных и т.д.
     */

    public partial class SimplePM_Worker
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

        public static List<ICompilerPlugin> _compilerPlugins; // Список, содержащий ссылки на модули компиляторов
        public static List<IJudgePlugin> _judgePlugins; // Список, содержащий ссылки на модули оценивания
        
        /*
         * Функция загружает в память компиляционные
         * модули,  которые  собирает  из специально
         * заготовленной директории.
         */

        private void LoadCompilerPlugins()
        {

            // Получаем список модулей компиляции
            _compilerPlugins = SimplePM_PluginsLoader.LoadPlugins<ICompilerPlugin>(
                (string)_serverConfiguration.path.ICompilerPlugin,
                "CompilerPlugin.Compiler"
            );

        }

        /*
         * Функция загружает в память что-то зачем-то.
         */

        private void LoadJudgePlugins()
        {

            // Получаем список модулей оценивания
            _judgePlugins = SimplePM_PluginsLoader.LoadPlugins<IJudgePlugin>(
                (string)_serverConfiguration.path.IJudgePlugin,
                "JudgePlugin.Judge"
            );

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
             * для  хранения  лог-файлов, так  как
             * некоторые версии NLog не создают
             * её автоматически.
             */
            
            try
            {

                Directory.CreateDirectory("./log/");

            }
            catch
            {

                /*
                 * Выполнения дополнительных действий не требуется
                 */
            }

            /*
             * Устанавливаем глобальный
             * обработчик исключений.
             */

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
            
            /*
             * Блок обработки исключительных ситуаций
             * в данном случае  необходим для обеспе-
             * чения работы сервера проверки  решений
             * даже без наличия некоторых видов прав.
             */

            try
            {

                /*
                 * Удаляем все файлы в папке
                 */

                foreach (var file in Directory.GetFiles((string)(_serverConfiguration.path.temp)))
                    File.Delete(file);

                /*
                 * Удаляем все директории в папке
                 */

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
            
            /*
             * Конфигурируем журнал событий (библиотека NLog)
             */

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
             * В  случае  запроса  на  автоматическое
             * выставление   лимитов   по   обработке
             * запросов  на  тестирование, необходимо
             * их всё-таки выставить.  Будем считать,
             * что   максимально    возможное   число
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
             * плагины сервера в память.
             */

            //TODO:LoadServerPlugins();
            
            /*
             * Вызываем метод, который загружает
             * модули компиляции в память.
             */

            LoadCompilerPlugins();

            /*
             * Вызываем метод, который загружает
             * модули оценивания в память.
             */

            LoadJudgePlugins();

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

                        // Записываем все исключения как ошибки в лог
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
