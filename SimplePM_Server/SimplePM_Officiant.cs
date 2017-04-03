using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using IniParser;
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
            
            codeWriter.WriteLine(submissionInfo["problemCode"]);
            codeWriter.Flush();
            codeWriter.Close();

            SimplePM_Compiler compiler = new SimplePM_Compiler(sConfig, ulong.Parse(submissionInfo["submissionId"]), fileExt);
            SimplePM_Compiler.CompilerResult cResult;

            switch (Submission.getCodeLanguageByName(submissionInfo["codeLang"]))
            {
                case Submission.SubmissionLanguage.freepascal:
                    cResult = compiler.startFreepascalCompiler();
                    break;
                default:
                    return;
            }

            string queryUpdate = "UPDATE `spm_submissions` SET `compiler_text` = '" + cResult.compilerMessage + "' WHERE `submissionId` = '" + submissionInfo["submissionId"] + "' LIMIT 1;";
            new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();

            if (cResult.hasErrors)
            {
                queryUpdate = "UPDATE `spm_submissions` SET `status` = 'error' WHERE `submissionId` = '" + submissionInfo["submissionId"] + "' LIMIT 1;";
                new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
            }
            else
            {
                switch (submissionInfo["testType"])
                {
                    case "syntax":
                        queryUpdate = "UPDATE `spm_submissions` SET `status` = 'ready' WHERE `submissionId` = '" + submissionInfo["submissionId"] + "' LIMIT 1;";
                        new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
                        break;
                    case "debug":

                        break;
                    case "release":

                        break;
                }
            }

            File.Delete(cResult.exe_fullname);
            File.Delete(fileLocation);

        }
    }
}
