//Основа
using System;
using System.Collections.Generic;
//Подключение к БД
using MySql.Data.MySqlClient;
//Конфигурационный файл
using IniParser.Model;
//Работа с файлами
using System.IO;

namespace SimplePM_Server
{
    class SimplePM_Officiant
    {
        //Объявляем необходимые переменные
        private MySqlConnection connection; //дескриптор соединения с БД
        private Dictionary<string, string> submissionInfo; //словарь информации о запросе
        private IniData sConfig; //дескриптор конфигурационного файла

        public SimplePM_Officiant(MySqlConnection connection, IniData sConfig, Dictionary<string, string> submissionInfo)
        {
            this.connection = connection;
            this.sConfig = sConfig;
            this.submissionInfo = submissionInfo;
        }

        /// <summary>
        /// Процедура, которая отвечает за обработку пользовательского запроса
        /// </summary>
        public void serveSubmission()
        {
            //Определяем язык написания пользовательской программы
            Submission.SubmissionLanguage codeLang = Submission.getCodeLanguageByName(submissionInfo["codeLang"]);

            //Сводим на нет все попытки "подлома"
            switch (codeLang)
            {
                case Submission.SubmissionLanguage.freepascal:
                    submissionInfo["problemCode"].Replace("uses", "");
                    break;
                default:
                    return;
            }

            //Определяем расширение файла
            string fileExt = "." + Submission.getExtByLang(codeLang);
            //Определяем полный путь к файлу
            string fileLocation = sConfig["Program"]["tempPath"] + submissionInfo["submissionId"] + fileExt;

            //Создаём файл исходного кода
            StreamWriter codeWriter = File.CreateText(fileLocation);

            //Устанавливаем его аттрибуты
            File.SetAttributes(fileLocation, FileAttributes.Temporary | FileAttributes.NotContentIndexed);
            
            //Записываем в него исходный код, очищаем буфер и закрываем поток записи
            codeWriter.WriteLine(submissionInfo["problemCode"]);
            codeWriter.Flush();
            codeWriter.Close();

            //Объявляем экземпляр класса компиляции
            SimplePM_Compiler compiler = new SimplePM_Compiler(sConfig, ulong.Parse(submissionInfo["submissionId"]), fileExt);
            //Объявляем переменную результата компиляции
            SimplePM_Compiler.CompilerResult cResult;

            //Запускаем определённый компилятор в зависимости от языка решения задачи
            switch (codeLang)
            {
                case Submission.SubmissionLanguage.freepascal:
                    //Запускаем компилятор
                    cResult = compiler.startFreepascalCompiler();
                    break;
                default:
                    return;
            }

            //Записываем в базу данных сообщение компилятора
            string queryUpdate = "UPDATE `spm_submissions` SET `compiler_text` = '" + cResult.compilerMessage + "' WHERE `submissionId` = '" + submissionInfo["submissionId"] + "' LIMIT 1;";
            new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();

            //Проверяем на наличие ошибок компиляции
            if (cResult.hasErrors)
            {
                //Ошибка компиляции, записываем это в БД
                queryUpdate = "UPDATE `spm_submissions` SET `status` = 'ready', `hasError` = true WHERE `submissionId` = '" + submissionInfo["submissionId"] + "' LIMIT 1;";
                new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
            }
            else
            {
                //Выполняем различные действия в зависимости от теста
                switch (submissionInfo["testType"])
                {
                    //Проверка синтаксиса
                    case "syntax":
                        queryUpdate = "UPDATE `spm_submissions` SET `status` = 'ready' WHERE `submissionId` = '" + submissionInfo["submissionId"].ToString() + "' LIMIT 1;";
                        new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
                        break;
                    //Отладка программы по пользовательскому тесту
                    case "debug":
                        //Запускаем тестирование программы
                        new SimplePM_Tester(
                            connection, //дескриптор соединения с БД
                            cResult.exe_fullname, //полный путь к исполняемому файлу
                            ulong.Parse(submissionInfo["problemId"].ToString()), //идентификатор задачи
                            ulong.Parse(submissionInfo["submissionId"].ToString()), //идентификатор запроса на отправку
                            float.Parse(submissionInfo["difficulty"]), //сложность поставленной задачи
                            ulong.Parse(submissionInfo["userId"]), //идентификатор пользователя
                            sConfig, //дескриптор конфигурационного файла сервера
                            submissionInfo["customTest"] //собственный тест пользователя
                        ).DebugTest();
                        break;
                    //Отправка решения задачи
                    case "release":
                        //Запускаем тестирование программы
                        new SimplePM_Tester(
                            connection, //дескриптор соединения с БД
                            cResult.exe_fullname, //полный путь к исполняемому файлу
                            ulong.Parse( submissionInfo["problemId"].ToString() ), //идентификатор задачи
                            ulong.Parse( submissionInfo["submissionId"].ToString() ), //идентификатор запроса на отправку
                            float.Parse( submissionInfo["difficulty"] ), //сложность поставленной задачи
                            ulong.Parse(submissionInfo["userId"]) //идентификатор пользователя
                        ).ReleaseTest();
                        break;
                }
            }

            //Очищаем папку экзешников от мусора
            try
            {
                File.Delete(cResult.exe_fullname);
                File.Delete(fileLocation);
            }
            catch (Exception) { }
        }
    }
}
