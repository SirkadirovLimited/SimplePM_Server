/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 * 
 * SimplePM Server is a part of software product "Automated
 * verification system for programming tasks "SimplePM".
 * 
 * Copyright (C) 2016-2018 Yurij Kadirov
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 * 
 * GNU Affero General Public License applied only to source code of
 * this program. More licensing information hosted on project's website.
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
            FileName = ""
            
        };

        private long UsedMemory;
        private int UsedProcessorTime;

    }
    
}