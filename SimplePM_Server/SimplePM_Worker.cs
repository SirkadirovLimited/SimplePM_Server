/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 license.
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
//Журнал событий
using NLog;
using NLog.Config;

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

    class SimplePM_Worker
    {

        ///////////////////////////////////////////////////
        // РАЗДЕЛ ОБЪЯВЛЕНИЯ ГЛОБАЛЬНЫХ ПЕРЕМЕННЫХ
        ///////////////////////////////////////////////////

        /*!
            Объявляем переменную указателя на менеджер журнала собылий
            и присваиваем ей указатель на журнал событий текущего класса
        */
        public static Logger logger = LogManager.GetCurrentClassLogger();

        //Текущее количество подсоединённых пользователей
        public static ulong _customersCount = 0;
        public static ulong _maxCustomersCount = 10;

        //Объявляем дескриптор конфигурационного файла
        public static IniData sConfig;

        //Устанавливаем время ожидания
        public static int SleepTime = 1000;

        //Список поддерживаемых языков программирования
        //для использования в выборочных SQL запросах
        public static string EnabledLangs;

        ///////////////////////////////////////////////////
        /// Функция генерирует строку из допустимых для
        /// проверки языков программирования, на которых
        /// написаны пользовательские программы
        ///////////////////////////////////////////////////

        public static void GenerateEnabledLangsList()
        {

            /* Инициализируем список строк */
            List<string> EnabledLangsList = new List<string>();

            /* Добавляем элементы в массив */

            //Pascal (Free Pascal, Object Pascal, etc.)
            if (sConfig["Compilers"].ContainsKey("freepascal"))
                EnabledLangsList.Add("'" + sConfig["Compilers"]["freepascal"] + "'");
            
            // C#
            if (sConfig["Compilers"].ContainsKey("csharp"))
                EnabledLangsList.Add("'" + sConfig["Compilers"]["csharp"] + "'");
            
            // C++
            if (sConfig["Compilers"].ContainsKey("cpp"))
                EnabledLangsList.Add("'" + sConfig["Compilers"]["cpp"] + "'");
            
            // C
            if (sConfig["Compilers"].ContainsKey("c"))
                EnabledLangsList.Add("'" + sConfig["Compilers"]["c"] + "'");
            
            // Lua
            if (sConfig["Compilers"].ContainsKey("lua"))
                EnabledLangsList.Add("'" + sConfig["Compilers"]["lua"] + "'");
            
            // Python
            if (sConfig["Compilers"].ContainsKey("python"))
                EnabledLangsList.Add("'" + sConfig["Compilers"]["python"] + "'");
            
            // PHP
            if (sConfig["Compilers"].ContainsKey("php"))
                EnabledLangsList.Add("'" + sConfig["Compilers"]["php"] + "'");
            
            // Java
            if (sConfig["Compilers"].ContainsKey("java"))
                EnabledLangsList.Add("'" + sConfig["Compilers"]["java"] + "'");

            //Формируем список доступных языков
            EnabledLangs = string.Join(", ", EnabledLangsList);
            
        }

        ///////////////////////////////////////////////////
        /// Функция устанавливает "улавливатель"
        /// непредвиденных исключений
        ///////////////////////////////////////////////////

        public static void SetExceptionHandler()
        {

            //Устанавливаем обработчик необработанных исключений
            AppDomain.CurrentDomain.UnhandledException += ExceptionEventLogger;

        }

        ///////////////////////////////////////////////////
        /// Функция, вызывающаяся при получении информации
        /// о необработанном фатальном исключении
        ///////////////////////////////////////////////////

        public static void ExceptionEventLogger(object sender, UnhandledExceptionEventArgs e)
        {

            //Записываем сообщение об ошибке в журнал событий
            logger.Fatal(e.ExceptionObject);

        }

        ///////////////////////////////////////////////////
        /// Автоматически запускаемая при запуске
        /// сервера функция. Является точкой входа.
        ///////////////////////////////////////////////////

        public static void Main(string[] args)
        {

            ///////////////////////////////////////////////////
            // ИНИЦИАЛИЗАЦИЯ СЕРВЕРНЫХ ПОДСИСТЕМ И ПРОЧИЙ ХЛАМ
            ///////////////////////////////////////////////////

            //Устанавливаем "улавливатель исключений"
            SetExceptionHandler();

            //Генерирую "шапку" консоли сервера
            GenerateProgramHeader();
            
            //Открываем конфигурационный файл для чтения
            FileIniDataParser iniParser = new FileIniDataParser();

            //Присваиваем глобальной переменной sConfig дескриптор файла конфигурации
            sConfig = iniParser.ReadFile("server_config.ini", Encoding.UTF8);

            //Конфигурируем журнал событий (библиотека NLog)
            try
            {
                LogManager.Configuration = new XmlLoggingConfiguration(sConfig["Program"]["NLogConfig_path"]);
            }
            catch (Exception) { /* Deal with it */ }

            //Получаем информацию с конфигурационного файла
            //для некоторых переменных
            _maxCustomersCount = ulong.Parse(sConfig["Connection"]["maxConnectedClients"]);
            SleepTime = int.Parse(sConfig["Connection"]["check_timeout"]);

            // Вызываем функцию получения строчного списка
            // поддерживаемых языков программирования данным
            // экземплятор сервера SimplePM_Server
            GenerateEnabledLangsList();

            ///////////////////////////////////////////////////
            // ОБРАБОТКА АРГУМЕНТОВ
            ///////////////////////////////////////////////////

            new SimplePM_Commander().SplitArguments(args);

            ///////////////////////////////////////////////////
            // ОСНОВНОЙ ЦИКЛ СЕРВЕРА
            ///////////////////////////////////////////////////

            while (true)
            {

                //Отлавливаем все ошибки
                try
                {
                
                    //Получаем дескриптор соединения с базой данных
                    MySqlConnection conn = StartMysqlConnection(sConfig);

                    //Защита от перегрузки сервера
                    if (_customersCount < _maxCustomersCount)
                        GetSubIdAndRunCompile(conn);

                }
                //В случае ошибки передаём информацию о ней логгеру событий
                catch (Exception ex) { logger.Error(ex); }

                //Ожидание для уменьшения нагрузки на сервер
                Thread.Sleep(SleepTime);

            }

            ///////////////////////////////////////////////////

        }

        ///////////////////////////////////////////////////
        /// ПЕРВОНАЧАЛЬНАЯ НАСТРОЙКА ОКНА КОНСОЛИ
        ///////////////////////////////////////////////////

        public static void GenerateProgramHeader()
        {

            //Установка заголовка приложения
            Console.Title = "SimplePM_Server";

            //Выводим на экран информацию о приложении
            Console.WriteLine(Properties.Resources.consoleHeader);

            //Отключаем показ курсора и возможность ввода
            Console.CursorVisible = false;

        }

        ///////////////////////////////////////////////////
        /// ОБРАБОТКА ЗАПРОСОВ НА ПРОВЕРКУ РЕШЕНИЙ
        ///////////////////////////////////////////////////

        public static void GetSubIdAndRunCompile(MySqlConnection connection)
        {
            
            //Создаём новую задачу, без неё - никак!
            new Task(() =>
            {

                //Формируем запрос на выборку
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

                //Объявляем словарь, который будет содержать информацию о запросе
                Dictionary<string, string> submissionInfo = new Dictionary<string, string>();

                //Создаём запрос на выборку из базы данных
                MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);

                //Производим выборку полученных результатов из временной таблицы
                MySqlDataReader dataReader = cmdSelect.ExecuteReader();

                //Получаем все поля запроса на тестирование
                while (dataReader.Read())
                {
                    submissionInfo["submissionId"] = dataReader["submissionId"].ToString(); //идентификатор
                    submissionInfo["classworkId"] = dataReader["classworkId"].ToString(); //идентификатор урока
                    submissionInfo["olympId"] = dataReader["olympId"].ToString(); //идентификатор олимпиады
                    submissionInfo["codeLang"] = dataReader["codeLang"].ToString(); //язык исходного кода
                    submissionInfo["userId"] = dataReader["userId"].ToString(); //идентификатор пользователя
                    submissionInfo["problemId"] = dataReader["problemId"].ToString(); //идентификатор задачи
                    submissionInfo["testType"] = dataReader["testType"].ToString(); //тип теста
                    submissionInfo["problemCode"] = HttpUtility.HtmlDecode(dataReader["problemCode"].ToString()); //код программы
                    submissionInfo["customTest"] = HttpUtility.HtmlDecode(dataReader["customTest"].ToString()); //собственный тест пользователя
                }

                //Закрываем чтение временной таблицы
                dataReader.Close();

                //Производим проверку на успешное получение данных о запросе
                if (submissionInfo.Count > 0)
                {

                    //Получаем сложность поставленной задачи
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

                    //Устанавливаем статус запроса на "в обработке"
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

                    //Зовём официанта-шляпочника
                    //уж он знает, что делать в таких вот ситуациях
                    SimplePM_Officiant officiant = new SimplePM_Officiant(connection, sConfig, submissionInfo);
                    officiant.ServeSubmission();

                    //Уменьшаем количество текущих соединений
                    //чтобы другие соединения были возможны.
                    _customersCount--;

                }

                //Закрываем соединение с БД
                connection.Close();

            }).Start();

        }

        ///////////////////////////////////////////////////
        /// Функция инициализирует соединение с базой
        /// данных MySQL используя данные аутенфикации,
        /// расположенные в конфигурационном файле сервера
        /// /return Указатель на соединение с БД
        ///////////////////////////////////////////////////

        public static MySqlConnection StartMysqlConnection(IniData sConfig)
        {

            //Подключаемся к базе данных на удалённом
            //MySQL сервере и получаем дескриптор подключения к ней
            MySqlConnection db = new MySqlConnection(
                $@"
                    server={sConfig["Database"]["db_host"]};
                    uid={sConfig["Database"]["db_user"]};
                    pwd={sConfig["Database"]["db_pass"]};
                    database={sConfig["Database"]["db_name"]};
                    Charset={sConfig["Database"]["db_chst"]}
                "
            );
            db.Open();

            //Возвращаем дескриптор подключения к базе данных
            return db;

        }

        ///////////////////////////////////////////////////

    }

}
