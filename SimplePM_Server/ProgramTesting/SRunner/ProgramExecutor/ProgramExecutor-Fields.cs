using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CompilerPlugin;
using NLog;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        private static readonly Logger logger = LogManager.GetLogger("SimplePM_Server.ProgramTesting.SRunner.ProgramExecutor");

        private dynamic _compilerConfiguration;
        private ICompilerPlugin _compilerPlugin;

        private readonly string _programPath, _programArguments;
        private readonly byte[] _programInputBytes;

        private readonly long _programMemoryLimit;
        private readonly int _programProcessorTimeLimit, _programRuntimeLimit, _outputCharsLimit;

        private readonly bool _adaptOutput;

        private string _programOutput = "", _programErrorOutput;

        private Process _programProcess;
        
        private bool _testingResultReceived;
        private char _testingResult = SingleTestResult.PossibleResult.ServerErrorResult;
        
        private ProcessStartInfo programStartInfo = new ProcessStartInfo
        {

            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,

            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding  = Encoding.UTF8,
            
            UseShellExecute = false,
            LoadUserProfile = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            ErrorDialog = false,

            Arguments = "",
            FileName = "",
            
        };

        private long UsedMemory;
        private int UsedProcessorTime;

    }
    
}