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
        private MySqlConnection connection;
        private Dictionary<string, string> submissionInfo;

        public SimplePM_Compiler(MySqlConnection connection, Dictionary<string,string> submissionInfo)
        {
            this.connection = connection;
            this.submissionInfo = submissionInfo;
        }

        public class CompilerResult
        {
            public bool hasErrors = false;
            public string exe_fullname = null;
            public string compilerMessage = null;
        }

        public CompilerResult freepascalCompiler()
        {
            return null;
        }
    }
}
