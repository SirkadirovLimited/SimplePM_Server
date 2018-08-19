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
using System.Threading;

namespace SProgramRunner
{
    
    public partial class SRunner
    {

        public ProgramRunningResult Execute()
        {

            try
            {

                // Run program testing
                RunTesting();
                
            }
            catch (Exception ex)
            {

                // Inform user about catched exception
                TestingExceptionCatched(ex, TestingResult.ServerErrorResult);

            }
            
            throw new NotImplementedException();
            
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
            
            // Begin asynchronous read of program's output
            _process.BeginOutputReadLine();

            // Write program input data to STDIN
            WriteInputData_StandardInputStream();
            
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
            
        }
        
    }
    
}