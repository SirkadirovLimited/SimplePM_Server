/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server is a part of software product "Automated
 * vefification system for programming tasks "SimplePM".
 *
 * Copyright 2018 Yurij Kadirov
 *
 * Source code of the product licensed under the Apache License,
 * Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Visit website for more details: https://spm.sirkadirov.com/
 */

using NLog;
using System.Text;
using CompilerPlugin;
using System.Diagnostics;
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