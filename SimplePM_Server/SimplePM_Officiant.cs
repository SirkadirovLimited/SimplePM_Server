using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SimplePM_Server
{
    class SimplePM_Officiant
    {
        private MySqlConnection connection;
        private Dictionary<string, string> submissionInfo;

        public SimplePM_Officiant(MySqlConnection connection, Dictionary<string, string> submissionInfo)
        {
            this.connection = connection;
            this.submissionInfo = submissionInfo;
        }

        public void serveSubmission()
        {
            SimplePM_Compiler compiler = new SimplePM_Compiler(connection, submissionInfo);

            switch (Submission.getCodeLanguageByName(submissionInfo["codeLang"]))
            {
                case Submission.SubmissionLanguage.freepascal:
                    
                    break;
                case Submission.SubmissionLanguage.unset:
                    Console.WriteLine("[ERROR] Language not supported on submission '" + submissionInfo["submissionId"] + "'");
                    break;
            }

        }
    }
}
