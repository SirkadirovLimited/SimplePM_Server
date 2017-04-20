/*
 * Copyright (C) 2017, Kadirov Yurij.
 * Licensed under CC BY-NC-SA 4.0 license.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: http://spm.sirkadirov.com/
 */

//Основа
using System;
using System.Collections.Generic;
using System.Text;
//Многопоточность
using System.Threading;
//Подключение к MySQL серверу
using MySql.Data.MySqlClient;
//Парсер конфигурационного файла
using IniParser;
using IniParser.Model;
//Для безопасности
using System.Web;

namespace SimplePM_Server
{
    class Gov
    {
        //Текущее количество подсоединённых пользователей
        public static int _customersCount = 0;
        public static int _maxCustomersCount = 10;
        
        //Объявляем дескриптор конфигурационного файла
        public static IniData sConfig;

        //Устанавливаем время ожидания
        private static int sleepTime = 1000;

        /// <summary>
        /// Процедура, отвечающая за установку и настройку "отлавливателя исключений"
        /// </summary>
        private static void setExceptionHandler()
        {
            //Когда подключён дебагер - не тревожить, в инном случае жаловаться пользователю
            NBug.Settings.ReleaseMode = true;
            
            //Устанавливаем место хранения архивов, содержащих информацию об ошибке
            NBug.Settings.StoragePath = NBug.Enums.StoragePath.CurrentDirectory;
            //Устанавливаем полный показ сведений об исключении
            NBug.Settings.UIMode = NBug.Enums.UIMode.Full;
            //Устанавливаем провайдера форм
            NBug.Settings.UIProvider = NBug.Enums.UIProvider.WinForms;

            //Устанавливаем обработчик необработанных исключений
            AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
        }

        /// <summary>
        /// Процедура отображает информацию о компиляторах
        /// </summary>
        /// <param name="sConfig">Дескриптор конфигурационного файла</param>
        public static void viewCompilersInfo(IniData sConfig)
        {
            Console.WriteLine("\n█ Compilers information █");
            //Free Pascal
            Console.WriteLine("Free Pascal: " + sConfig["Compilers"]["freepascal_enabled"] + ", <" + sConfig["Compilers"]["freepascal_location"] + ">");
            //CSharp
            Console.WriteLine("CSharp: " + sConfig["Compilers"]["csharp_enabled"] + ", <" + sConfig["Compilers"]["csharp_location"] + ">");
            //Cpp
            Console.WriteLine("CPP: " + sConfig["Compilers"]["cpp_enabled"] + ", <" + sConfig["Compilers"]["cpp_location"] + ">");
            //C
            Console.WriteLine("C: " + sConfig["Compilers"]["c_enabled"] + ", <" + sConfig["Compilers"]["c_location"] + ">");
            //Python
            Console.WriteLine("Python: " + sConfig["Compilers"]["python_enabled"] + ", <" + sConfig["Compilers"]["python_location"] + ">");
            //Lua
            Console.WriteLine("Lua: " + sConfig["Compilers"]["lua_enabled"] + ", <" + sConfig["Compilers"]["lua_location"] + ">");
        }

        static void Main(string[] args)
        {
            //Устанавливаем "улавливатель исключений"
            setExceptionHandler();

            //Генерирую "шапку" консоли сервера
            generateProgramHeader();
            
            //Открываем конфигурационный файл для чтения
            FileIniDataParser iniParser = new FileIniDataParser();
            sConfig = iniParser.ReadFile("server_config.ini", Encoding.UTF8);

            //Показать информацию о компиляторах
            viewCompilersInfo(sConfig);
            
            //Добавляю основной поток
            new Thread(() => {
                //Получаем дескриптор соединения с базой данных
                MySqlConnection conn = startMysqlConnection(sConfig);
                while (true)
                {
                    try
                    {
                        //Защита от перегрузки сервера
                        if (_customersCount < _maxCustomersCount)
                            getSubIdAndRunCompile(conn);
                    }
                    catch (Exception) { }
                    //Ожидание нескольких мс чтобы повторить запрос заново
                    Thread.Sleep(sleepTime);
                }
            }).Start();
        }

        /// <summary>
        /// Процедура отвечает за первоначальную настройку окна консоли SimplePM_Server
        /// </summary>
        private static void generateProgramHeader()
        {
            Console.Title = "SimplePM_Server";
            
            Console.WriteLine("█ SimplePM_Server");
            Console.WriteLine("█ Copyright (C) 2017, Kadirov Yurij. All rights are reserved.");
            Console.WriteLine("█ Official website: www.sirkadirov.com");
            Console.WriteLine("█ Support email: admin@sirkadirov.com");

            Console.CursorSize = 100;
            Console.CursorVisible = false;
        }

        /// <summary>
        /// Процедура отвечает за получение информации о запросе на тестирование пользовательского решения программы и начало её компиляции
        /// </summary>
        /// <param name="connection">Дескриптор соединения с базой данных</param>
        public static void getSubIdAndRunCompile(MySqlConnection connection)
        {
            new Thread(() =>
            {

                string querySelect = "SELECT * FROM `spm_submissions` WHERE `status` = 'waiting' ORDER BY `submissionId` ASC LIMIT 1;";

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
                    string queryGetDifficulty = "SELECT `difficulty` FROM `spm_problems` WHERE `id` = '" + submissionInfo["problemId"] + "' LIMIT 1;";
                    MySqlCommand cmdGetProblemDifficulty = new MySqlCommand(queryGetDifficulty, connection);
                    submissionInfo["difficulty"] = cmdGetProblemDifficulty.ExecuteScalar().ToString();

                    //Устанавливаем статус запроса на "в обработке"
                    string queryUpdate = "UPDATE `spm_submissions` SET `status` = 'processing' WHERE `submissionId` = '" + submissionInfo["submissionId"] + "' LIMIT 1;";
                    new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
                    _customersCount++;

                    //Зовём официанта-шляпочника
                    //уж он знает, что делать в таких вот ситуациях
                    SimplePM_Officiant officiant = new SimplePM_Officiant(connection, sConfig, submissionInfo);
                    officiant.serveSubmission();

                    //Уменьшаем количество текущих соединений
                    //чтобы другие соединения были возможны.
                    _customersCount--;
                }

            }).Start();
        }

        public static MySqlConnection startMysqlConnection(IniData sConfig)
        {
            //Подключаемся к базе данных на удалённом
            //MySQL сервере и получаем дескриптор подключения к ней
            MySqlConnection db = new MySqlConnection(
                "server=" + sConfig["Database"]["db_host"] +
                ";uid=" + sConfig["Database"]["db_user"] +
                ";pwd=" + sConfig["Database"]["db_pass"] +
                ";database=" + sConfig["Database"]["db_name"] +
                ";Charset=" + sConfig["Database"]["db_chst"] + ";"
            );
            db.Open();
            //Отображаем краткую (и никому не нужную) информацию о подключении
            Console.WriteLine("\n█ Database connection established successfully! █");
            Console.WriteLine("MySQL server version: " + db.ServerVersion);
            Console.WriteLine("Database name: " + db.Database);
            Console.WriteLine("MySQl connection timeout: " + db.ConnectionTimeout);
            Console.WriteLine();

            //Возвращаем дескриптор подключения к базе данных
            return db;
        }
    }
}
