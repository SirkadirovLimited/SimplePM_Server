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

using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SProgramRunner
{
    
    public partial class SRunner
    {

        private readonly TestingRequestStuct _testingRequestStuct;
        private ProgramRunningResult _programRunningResult;
        private Process _process;
        
        public SRunner(TestingRequestStuct testingRequestStuct)
        {

            // Store testing request data localy
            _testingRequestStuct = testingRequestStuct;
            
            // Init process object with required info
            _process = new Process
            {
                
                StartInfo = new ProcessStartInfo
                {
                    
                    /*
                     * Specify basic process information,
                     * such as path to program, its arguments
                     * and working directory.
                     */
                    
                    FileName = _testingRequestStuct.RuntimeInfo.FileName,
                    Arguments = _testingRequestStuct.RuntimeInfo.Arguments,
                    WorkingDirectory = _testingRequestStuct.RuntimeInfo.WorkingDirectory,
                    
                    /*
                     * To rewrite input for specified process
                     * and read all output data from standard
                     * output and standard error output we need
                     * to redirect all that outputs.
                     */
                    
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    
                    /*
                     * Set default encodings to all standard streams.
                     * This required for compatibility.
                     */
                    
                    StandardInputEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    
                    /*
                     * We need full control on starting process,
                     * so we don't need to use shell execute and
                     * create that process GUI to work with it.
                     */
                    
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    UseShellExecute = false
                    
                },
                
                // We need to catch all events, that associated with process
                EnableRaisingEvents = true
                
            };
            
            // Set RunAs feature information with lovely inline void :)
            SetRunAsInformation();

            _programRunningResult = new ProgramRunningResult
            {
                
                // Reset process resources usage statistics
                ProcessResourcesUsageStats = new ProcessResourcesUsageStatsStruct
                {
                
                    UsedProcessorTime = 0,
                    PeakUsedWorkingSet = 0,
                    WorkingDirectoryDiskUsage = 0,
                    RealRunTime = new TimeSpan(0, 0, 0, 0)
                
                },
                
                /*
                 * Set output data of all types to null (reset).
                 */
                
                ProgramOutputData = null,
                ProgramErrorData = null,
                
                // Default exit code - '-1'
                ProgramExitCode = -1,
                
                // Indicates that testing is in progress or corrupted
                Result = TestingResult.NoTestingResult
                
            };
            
            void SetRunAsInformation()
            {

                // Continue only if this feature enabled
                if (!_testingRequestStuct.RunAsInfo.Enable)
                    return;

                // Set username
                _process.StartInfo.UserName = _testingRequestStuct.RunAsInfo.UserName;

                /*
                 * Some features currently work only on Windows.
                 * I'm so sorry about it...
                 */
                
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {

                    // We don't need to load user profile
                    _process.StartInfo.LoadUserProfile = false;

                    // Give user's password required to log in
                    _process.StartInfo.Password = _testingRequestStuct.RunAsInfo.UserPassword;

                }

            }

        }
        
    }
    
}