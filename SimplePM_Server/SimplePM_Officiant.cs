using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using IniParser.Model;
using System.IO;

namespace SimplePM_Server
{
    class SimplePM_Officiant
    {
        private MySqlConnection connection;
        private Dictionary<string, string> submissionInfo;
        private IniData sConfig;

        public SimplePM_Officiant(MySqlConnection connection, IniData sConfig, Dictionary<string, string> submissionInfo)
        {
            this.connection = connection;
            this.sConfig = sConfig;
            this.submissionInfo = submissionInfo;
        }

        public void serveSubmission()
        {
            Submission.SubmissionLanguage codeLang = Submission.getCodeLanguageByName(submissionInfo["codeLang"]);

            string fileExt = "." + Submission.getExtByLang(codeLang);
            string fileLocation = sConfig["Program"]["tempPath"] + submissionInfo["submissionId"] + fileExt;

            StreamWriter codeWriter = File.CreateText(fileLocation);

            File.SetAttributes(fileLocation, FileAttributes.Temporary | FileAttributes.NotContentIndexed);
            
            codeWriter.WriteLine(submissionInfo["problemCode"]);
            codeWriter.Flush();
            codeWriter.Close();

            SimplePM_Compiler compiler = new SimplePM_Compiler(sConfig, ulong.Parse(submissionInfo["submissionId"]), fileExt);
            SimplePM_Compiler.CompilerResult cResult;

            //Запускаем определённый компилятор в зависимости от языка решения задачи
            switch (Submission.getCodeLanguageByName(submissionInfo["codeLang"]))
            {
                case Submission.SubmissionLanguage.freepascal:
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
                    case "syntax":
                        queryUpdate = "UPDATE `spm_submissions` SET `status` = 'ready' WHERE `submissionId` = '" + submissionInfo["submissionId"].ToString() + "' LIMIT 1;";
                        new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
                        break;
                    case "debug":

                        break;
                    case "release":
                        SimplePM_Tester releaseTester = new SimplePM_Tester(
                            connection,
                            cResult.exe_fullname,
                            ulong.Parse( submissionInfo["problemId"].ToString() ),
                            ulong.Parse( submissionInfo["submissionId"].ToString() ),
                            float.Parse( submissionInfo["difficulty"] ),
                            ulong.Parse(submissionInfo["userId"])
                        );
                        releaseTester.ReleaseTest();
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
