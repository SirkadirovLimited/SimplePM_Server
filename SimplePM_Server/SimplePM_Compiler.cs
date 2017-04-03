using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePM_Server
{
    class SimplePM_Compiler
    {
        private string submissionId, problemCode;

        public SimplePM_Compiler(string submissionId, string problemCode)
        {
            this.submissionId = submissionId;
            this.problemCode = problemCode;
        }

        public class CompilerResult
        {
            public bool hasErrors = false;
            public string exe_fullname = null;
            public string compilerMessage = null;
        }

        public CompilerResult startFreepascalCompiler()
        {
            return null;
        }
    }
}
