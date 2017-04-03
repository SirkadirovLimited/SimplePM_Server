using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using IniParser;
using IniParser.Model;
using System.IO;

namespace SimplePM_Server
{
    class SimplePM_Compiler
    {
        private ulong submissionId;
        private string fileLocation;
        private IniData sConfig;

        public SimplePM_Compiler(IniData sConfig, ulong submissionId, string fileLocation)
        {
            if (submissionId <= 0)
                throw new ArgumentNullException("submissionId", "Submission ID invalid!");
            if (string.IsNullOrEmpty(fileLocation) || string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
                throw new ArgumentNullException("fileLocation", "File not found!");

            this.sConfig = sConfig;
            this.submissionId = submissionId;
            this.fileLocation = fileLocation;
        }

        public class CompilerResult
        {
            public bool hasErrors = false;
            public string exe_fullname = null;
            public string compilerMessage = null;
        }

        public CompilerResult startFreepascalCompiler()
        {
            Process fpcProc = new Process();

            ProcessStartInfo pStartInfo = new ProcessStartInfo(sConfig["Compilers"]["freepascal_location"], fileLocation + " -va");
            pStartInfo.ErrorDialog = false;
            pStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.UseShellExecute = false;

            fpcProc.StartInfo = pStartInfo;
            fpcProc.Start();

            Console.WriteLine("<!--START--!>");
            StreamReader reader = fpcProc.StandardOutput;
            Console.WriteLine(reader.ReadToEnd());
            Console.WriteLine("<!--END--!>");

            fpcProc.WaitForExit();
            
            return null;
        }
    }
}
