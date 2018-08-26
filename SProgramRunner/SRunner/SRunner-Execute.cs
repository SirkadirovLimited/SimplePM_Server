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
using System.Threading;

namespace SProgramRunner
{
    
    public partial class SRunner
    {

        public ProgramRunningResult Execute()
        {

            // Init new ProgramRunningResult object with default values
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
                
                ProgramOutputData = Encoding.UTF8.GetBytes(""),
                ProgramErrorData = null,
                
                // Default exit code - '-1'
                ProgramExitCode = -1,
                
                // Indicates that testing is in progress or corrupted
                Result = TestingResult.NoTestingResult
                
            };
            
            try
            {

                // Run program testing
                RunTesting();
                
            }
            catch (Exception ex)
            {

                // Inform user about catched exception
                SetNewTestingResult(TestingResult.ServerErrorResult, true, ex);

            }
            
            // Execute additonal checkers
            ExecuteAdditonalCheckers();

            // Return formed program testing (running) result
            return _programRunningResult;
            
            //========================================================================================================//
            // INLINE METHOD, THAT DEFINES AND EXECUTES ADDITIONAL FINAL TESTING CHECKERS                             //
            //========================================================================================================//
            
            void ExecuteAdditonalCheckers()
            {

                /*
                 * Get some information about past from process
                 * object, so we can give more information to user.
                 */
                
                // Get process exit code
                _programRunningResult.ProgramExitCode = _process.ExitCode;

                // Get process real running time as TimeSpan
                _programRunningResult.ProcessResourcesUsageStats.RealRunTime = _process.ExitTime - _process.StartTime;
                
                /*
                 * Checkers for TestingResult.ErrorOutputNotNullResult
                 */
                
                var realErrorOutputData = _process.StandardError.ReadToEnd();

                if (!IsTestingResultReceived && string.IsNullOrWhiteSpace(_programRunningResult.ProgramErrorData) &&
                    !string.IsNullOrWhiteSpace(realErrorOutputData))
                {
                    
                    // Set new testing result
                    SetNewTestingResult(TestingResult.ErrorOutputNotNullResult, false);
                    
                    // Set program's error output data property
                    _programRunningResult.ProgramErrorData = realErrorOutputData;
                    
                }
                
                /*
                 * Checkers for TestingResult.RuntimeErrorResult
                 */
                
                if (!IsTestingResultReceived && _process.ExitCode != 0)
                    SetNewTestingResult(TestingResult.RuntimeErrorResult, false);
                
                /*
                 * Checkers for TestingResult.MiddleSuccessResult
                 */

                if (!IsTestingResultReceived)
                    SetNewTestingResult(TestingResult.MiddleSuccessResult);
                
            }
            
            //========================================================================================================//

        }

        private void RunTesting()
        {
            
            //========================================================================================================//
            // WRITE PROGRAM INPUT DATA TO A FILE, THEN DO SOME STUFF                                                 //
            //========================================================================================================//
            
            // Write program's input data to an input file
            WriteInputData_File();

            // Check for some errors occured during writing process
            if (IsTestingResultReceived)
                return;
            
            //========================================================================================================//

            // Start program process
            _process.Start();

            // Write program input data to STDIN
            WriteInputData_StandardInputStream();
            
            // Initialize advanced output stream reader
            _outputStreamReader = new AdvancedStreamReader(_process.StandardOutput.BaseStream);
            
            // Start output stream reader
            _outputStreamReader.Run();
            
            // Start limits checker task
            new Thread(ExecuteLimitsChecker).Start();

            //========================================================================================================//
            // WAIT FOR CHILD PROCESS TO END (OR KILl IT MANUALlY)                                                    //
            //========================================================================================================//
            
            // Check for process real working time limit
            var checkIsWaitChldLimitOccured = (
                _testingRequestStuct.LimitsInfo.Enable && // is this feature enabled
                _testingRequestStuct.LimitsInfo.ProcessRealWorkingTimeLimit != -1 && // is limit required
                !_process.WaitForExit(_testingRequestStuct.LimitsInfo.ProcessRealWorkingTimeLimit) // is limit reached
            );

            // Kill process if timeout reached
            if (checkIsWaitChldLimitOccured)
            {

                try
                {

                    // Kill process
                    _process.Kill();

                }
                catch
                {
                    /* Dead End */
                }

                // Set testing result as "wait error result".
                _programRunningResult.Result = TestingResult.WaitErrorResult;

            }
            //...or wait unlimited time for process to end
            else
                _process.WaitForExit();

            //========================================================================================================//

            // Call method that does some work to finalize testing on process exit
            ProcessOnExited();

        }

    }
    
}