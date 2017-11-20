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
/*! \file */

//Основа
using System;
//Подключаем коллекции
using System.Collections.Generic;
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
using System.Linq;
// Использование запросов
using System.Reflection;

namespace SimplePM_Server
{

    /*!
     * \brief
     * Базовый класс сервера контролирует
     * работу сервера проверки решений в
     * целом, содержит множество
     * инициализирующих что-либо функций,
     * множество переменных и т.д.
     */

    public class SimplePM_Worker
    {

        ///////////////////////////////////////////////////
        // РАЗДЕЛ ОБЪЯВЛЕНИЯ ГЛОБАЛЬНЫХ ПЕРЕМЕННЫХ
        ///////////////////////////////////////////////////

        /*!
            Объявляем переменную указателя на менеджер журнала собылий
            и присваиваем ей указатель на журнал событий текущего класса
        */
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        //Текущее количество подсоединённых пользователей
        private ulong _customersCount;
        private ulong _maxCustomersCount;

        //Объявляем дескриптор конфигурационного файла
        public IniData sConfig;

        //Устанавливаем время ожидания
        private int SleepTime = 500;

        //Список поддерживаемых языков программирования
        //для использования в выборочных SQL запросах
        private string EnabledLangs;

        private List<ICompilerPlugin> _compilerPlugins = new List<ICompilerPlugin>();

        ///////////////////////////////////////////////////
        /// Функция загружает в память компиляционные
        /// модули, которые собирает из специально
        /// заготовленной директории
        ///////////////////////////////////////////////////
        
        private void LoadCompilerPlugins()
        {

            // Инициализируем массив путей к сборкам
            string[] pluginFiles = Directory.GetFiles(sConfig["Program"]["ICompilerPlugin_directory"], "ICompilerPlugin.*.dll");

            /*
             * В цикле осуществляем поиск модулей, которые добавляют
             * поддержку языков программирования.
             */
            foreach (string pluginPath in pluginFiles)
            {
                
                logger.Debug("Start loading plugin [" + pluginPath + "]...");

                try
                {

                    // Загружаем сборку
                    Assembly assembly = Assembly.LoadFrom(pluginPath);

                    // Ищем необходимую для нас реализацию интерфейса
                    foreach (Type type in assembly.GetTypes())
                    {
                        
                        // Если мы нашли то, что искали - добавляем плагин в список
                        if (type.FullName == "CompilerPlugin.Compiler")
                            _compilerPlugins.Add((ICompilerPlugin) Activator.CreateInstance(type));

                    }

                }
                catch (Exception ex)
                {
                    logger.Debug(ex);
                }

            }

        }

        ///////////////////////////////////////////////////
        /// Функция генерирует строку из допустимых для
        /// проверки языков программирования, на которых
        /// написаны пользовательские программы
        ///////////////////////////////////////////////////

        public void GenerateEnabledLangsList()
        {

            /*
             * Инициализируем список строк и собираем
             * поддерживаемые языки программирования в массив
             */
            List<string> EnabledLangsList = new List<string>();

            /*
             * В цикле перебираем все поддерживаемые языки
             * программирования подключаемыми модулями и
             * приводим список поддерживаемых системой
             * языков к требуемому виду.
             */
            foreach (ICompilerPlugin compilerPlugin in _compilerPlugins)
            {
                
                //Добавляем язык программирования в список
                EnabledLangsList.Add("'" + compilerPlugin.CompilerPluginLanguageName + "'");

            }

            /*
             * Формируем список доступных языков
             */
            EnabledLangs = string.Join(", ", EnabledLangsList);
            
        }

        ///////////////////////////////////////////////////
        /// Функция устанавливает "улавливатель"
        /// непредвиденных исключений
        ///////////////////////////////////////////////////

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

        ///////////////////////////////////////////////////
        /// Функция, вызывающаяся при получении информации
        /// о необработанном фатальном исключении
        ///////////////////////////////////////////////////

        private void ExceptionEventLogger(object sender, UnhandledExceptionEventArgs e)
        {

            /* Записываем сообщение об ошибке в журнал событий */
            logger.Fatal(e.ExceptionObject);

        }

        ///////////////////////////////////////////////////
        /// Функция, инициализирующая все необходимые
        /// переменные и прочий хлам для возможности
        /// работы сервера проверки решений
        ///////////////////////////////////////////////////

        private void LoadResources(string[] args)
        {

            ///////////////////////////////////////////////////
            // ИНИЦИАЛИЗАЦИЯ СЕРВЕРНЫХ ПОДСИСТЕМ И ПРОЧИЙ ХЛАМ
            ///////////////////////////////////////////////////

            // Устанавливаем "улавливатель исключений"
            SetExceptionHandler();

            // Открываем конфигурационный файл для чтения
            FileIniDataParser iniParser = new FileIniDataParser();

            // Присваиваем глобальной переменной sConfig дескриптор файла конфигурации
            sConfig = iniParser.ReadFile("server_config.ini", Encoding.UTF8);

            // Конфигурируем журнал событий (библиотека NLog)
            try
            {
                LogManager.Configuration = new XmlLoggingConfiguration(sConfig["Program"]["NLogConfig_path"]);
            }
            catch
            {
                /* Deal with it */
            }

            /*
             * Получаем информацию с конфигурационного файла
             * для некоторых переменных
             */
            _maxCustomersCount = ulong.Parse(sConfig["Connection"]["maxConnectedClients"]);
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
             * поддерживаемых языков программирования
             */
            GenerateEnabledLangsList();

            ///////////////////////////////////////////////////
            // Вызов метода обработки аргументов запуска
            // консольного приложения
            ///////////////////////////////////////////////////

            new SimplePM_Commander().SplitArguments(args);

        }

        ///////////////////////////////////////////////////
        /// Функция, которая запускает соновной цикл
        /// сервера проверки решений. Работает постоянно
        /// в высшем родительском потоке.
        ///////////////////////////////////////////////////

        private void ServerLoop()
        {

            ///////////////////////////////////////////////////
            // Основной вечный цикл сервера
            ///////////////////////////////////////////////////

            uint rechecksCount = 0; // количество перепроверок без

            while (42 == 42)
            {

                if (_customersCount < _maxCustomersCount)
                {

                    // Отлавливаем все ошибки
                    try
                    {
                        //Получаем дескриптор соединения с базой данных
                        MySqlConnection conn = StartMysqlConnection(sConfig);

                        //Вызов чекера (если всё "хорошо")
                        if (conn != null)
                            GetSubIdAndRunCompile(conn);

                    }
                    // В случае ошибки передаём информацию о ней логгеру событий
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }

                }

                bool tmpCheck = rechecksCount >= uint.Parse(sConfig["Connection"]["rechecks_without_timeout"]);

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

        ///////////////////////////////////////////////////
        /// Базовая точка входа из-под всего сущего
        ///////////////////////////////////////////////////

        public void Run(string[] args)
        {
            
            // Загружаем все необходимые ресурсы
            LoadResources(args);

            // Запускаем основной цикл
            ServerLoop();
            
        }

        ///////////////////////////////////////////////////
        /// Функция обработки запросов на проверку решений
        ///////////////////////////////////////////////////

        public void GetSubIdAndRunCompile(MySqlConnection connection)
        {
            
            // Создаём новую задачу, без неё - никак!
            new Task(() =>
            {

                // Формируем запрос на выборку
                string querySelect = $@"
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

                // Объявляем словарь, который будет содержать информацию о запросе
                Dictionary<string, string> submissionInfo = new Dictionary<string, string>();

                // Создаём запрос на выборку из базы данных
                MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);

                // Производим выборку полученных результатов из временной таблицы
                MySqlDataReader dataReader = cmdSelect.ExecuteReader();

                // Получаем все поля запроса на тестирование
                if (dataReader.Read())
                {

                    submissionInfo["submissionId"] = dataReader["submissionId"].ToString(); // идентификатор
                    submissionInfo["classworkId"] = dataReader["classworkId"].ToString(); // идентификатор урока
                    submissionInfo["olympId"] = dataReader["olympId"].ToString(); // идентификатор олимпиады
                    submissionInfo["codeLang"] = dataReader["codeLang"].ToString(); // язык исходного кода
                    submissionInfo["userId"] = dataReader["userId"].ToString(); // идентификатор пользователя
                    submissionInfo["problemId"] = dataReader["problemId"].ToString(); // идентификатор задачи
                    submissionInfo["testType"] = dataReader["testType"].ToString(); // тип теста
                    submissionInfo["problemCode"] = HttpUtility.HtmlDecode(dataReader["problemCode"].ToString()); // код программы
                    submissionInfo["customTest"] = HttpUtility.HtmlDecode(dataReader["customTest"].ToString()); // собственный тест пользователя
                    submissionInfo["setAsAuthorSolution"] = HttpUtility.HtmlDecode(dataReader["setAsAuthorSolution"].ToString()); // собственный тест пользователя

                }

                // Закрываем чтение временной таблицы
                dataReader.Close();

                // Производим проверку на успешное получение данных о запросе
                if (submissionInfo.Count > 0)
                {

                    // Получаем сложность поставленной задачи
                    string queryGetDifficulty = $@"
                        SELECT 
                            `difficulty` 
                        FROM 
                            `spm_problems` 
                        WHERE 
                            `id` = '{submissionInfo["problemId"]}' 
                        LIMIT 
                            1
                        ;
                    ";

                    MySqlCommand cmdGetProblemDifficulty = new MySqlCommand(queryGetDifficulty, connection);
                    submissionInfo["difficulty"] = cmdGetProblemDifficulty.ExecuteScalar().ToString();

                    // Устанавливаем статус запроса на "в обработке"
                    string queryUpdate = $@"
                        UPDATE 
                            `spm_submissions` 
                        SET 
                            `status` = 'processing' 
                        WHERE 
                            `submissionId` = '{submissionInfo["submissionId"]}'
                        LIMIT 
                            1
                        ;
                    ";

                    new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
                    _customersCount++;

                    // Зовём официанта-шляпочника
                    // уж он знает, что делать в таких вот ситуациях
                    new SimplePM_Officiant(
                        connection,
                        ref sConfig,
                        ref _compilerPlugins,
                        submissionInfo
                    ).ServeSubmission();

                    // Уменьшаем количество текущих соединений
                    // чтобы другие соединения были возможны.
                    _customersCount--;

                }

                // Закрываем соединение с БД
                connection.Close();

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
                        Charset={sConfig["Database"]["db_chst"]}
                    "
                );

                // Открываем соединение с БД
                db.Open();

            }
            catch (MySqlException)
            {
                /* Deal with it */
            }

            //Возвращаем дескриптор подключения к базе данных
            return db;

        }

        ///////////////////////////////////////////////////

    }

}
