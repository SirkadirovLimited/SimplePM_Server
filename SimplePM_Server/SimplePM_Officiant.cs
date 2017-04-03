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

            string fileLocation = sConfig["Program"]["tempPath"] + submissionInfo["submissionId"] + "." + Submission.getExtByLang(codeLang);

            StreamWriter codeWriter = File.CreateText(fileLocation);

            //throw new Exception(submissionInfo["problemCode"]);
            codeWriter.WriteLine(submissionInfo["problemCode"]);
            codeWriter.Flush();
            codeWriter.Close();

            SimplePM_Compiler compiler = new SimplePM_Compiler(sConfig, ulong.Parse(submissionInfo["submissionId"]), fileLocation);
            SimplePM_Compiler.CompilerResult cResult;

            switch (Submission.getCodeLanguageByName(submissionInfo["codeLang"]))
            {
                case Submission.SubmissionLanguage.freepascal:
                    cResult = compiler.startFreepascalCompiler();
                    break;
                case Submission.SubmissionLanguage.unset:
                    Console.WriteLine("[ERROR] Language not supported on submission '" + submissionInfo["submissionId"] + "'");
                    break;
            }

        }
    }
}
