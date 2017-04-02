using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//MySQL client
using MySql.Data.MySqlClient;
//INI Parser
using IniParser;
using IniParser.Model;
//For errors
using System.Windows.Forms;

namespace SimplePM_Server
{
    class Gov
    {
        public const int _MAJOR_VER_ = 0;
        public const int _MINOR_VER_ = 1;
        public const int _PATCH_VER_ = 0;
        public const string _RELEASE_TYPE_ = "prealpha";
        public const int _RELEASE_ID_ = 0;

        public static string db_host = "37.57.143.185";
        public static string db_user = "Sirkadirov";
        public static string db_pass = "Dam900000zaua";
        public static string db_name = "simplepm";

        public static int _customersCount = 0;
        public static int _maxCustomersCount = 10;
        
        public static IniData sConfig;

        private static int sleepTime = 1000;

        static void Main(string[] args)
        {
            //Генерирую "шапку" консоли сервера
            generateProgramHeader();
            try
            {
                FileIniDataParser iniParser = new FileIniDataParser();
                sConfig = iniParser.ReadFile("server_config.ini", Encoding.UTF8);
            }
            catch (Exception)
            {
                MessageBox.Show("SimplePM_Server configuration file (server_config.ini) is not found in the same directory as SimplePM_Server.exe!" +
                    "\nUse SimplePM_Server Configuration Tool to generate new configuration file!" +
                    "\n",
                    "Configuration file not found!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Environment.Exit(-2);
            }

            //Добавляю поток управления
            new Thread(new ThreadStart(cmdCatcher)).Start();
            
            //Добавляю основной поток
            new Thread(() => {
                MySqlConnection conn = startMysqlConnection();
                while (true)
                {
                    try
                    {
                        if (_customersCount < _maxCustomersCount)
                            getSubIdAndRunCompile(conn);
                    }
                    catch (Exception) { }
                    Thread.Sleep(sleepTime);
                }
            }).Start();
        }

        private static void generateProgramHeader()
        {
            Console.Title = "SimplePM_Server";

            Console.WriteLine("█ SimplePM_Server v" + _MAJOR_VER_ + "." + _MINOR_VER_ + "." + _PATCH_VER_ + "-" + _RELEASE_TYPE_ + _RELEASE_ID_);
            Console.WriteLine("█ Copyright (C) 2017, Kadirov Yurij. All rights are reserved.");
            Console.WriteLine("█ Official website: www.sirkadirov.com");
            Console.WriteLine("█ Support email: admin@sirkadirov.com");
            Console.WriteLine("█ Type [h] for help");

            Console.CursorSize = 100;
            Console.CursorVisible = false;
        }

        public static void cmdCatcher()
        {
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.H:
                        Console.WriteLine("Help requested.");
                        break;
                }
            }
        }

        public static void getSubIdAndRunCompile(MySqlConnection connection)
        {
            new Thread(() => {

                string querySelect = "SELECT * FROM `spm_submissions` WHERE `status` = 'waiting' ORDER BY `submissionId` ASC LIMIT 1;";

                Dictionary<string, string> submissionInfo = new Dictionary<string, string>();

                MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);

                MySqlDataReader dataReader = cmdSelect.ExecuteReader();

                while (dataReader.Read())
                {
                    submissionInfo["submissionId"] = dataReader["submissionId"].ToString();
                    submissionInfo["codeLang"] = dataReader["codeLang"].ToString();
                    submissionInfo["userId"] = dataReader["userId"].ToString();
                    submissionInfo["problemId"] = dataReader["problemId"].ToString();
                    submissionInfo["testType"] = dataReader["testType"].ToString();
                    submissionInfo["problemCode"] = dataReader["problemCode"].ToString();
                    submissionInfo["customTest"] = dataReader["customTest"].ToString();
                }

                dataReader.Close();

                if (submissionInfo.Count > 0)
                {
                    string queryUpdate = "UPDATE `spm_submissions` SET `status` = 'processing' WHERE `submissionId` = '" + submissionInfo["submissionId"] + "' LIMIT 1;";
                    MySqlCommand cmdUpdate = new MySqlCommand(queryUpdate, connection);
                    cmdUpdate.ExecuteNonQuery();
                    _customersCount++;

                    SimplePM_Officiant officiant = new SimplePM_Officiant(connection, submissionInfo);
                    officiant.serveSubmission();
                }

            }).Start();
        }

        public static MySqlConnection startMysqlConnection()
        {
            try
            {
                MySqlConnection db = new MySqlConnection("server=" + db_host + ";uid=" + db_user + ";pwd=" + db_pass + ";database=" + db_name + ";Charset=utf8;");
                db.Open();

                Console.WriteLine("MySQL server version: " + db.ServerVersion);
                Console.WriteLine("Database name: " + db.Database);
                Console.WriteLine("MySQl connection timeout: " + db.ConnectionTimeout);

                return db;
            }
            catch (MySqlException)
            {
                Environment.Exit(-1);
                return null;
            }
        }
    }
}
