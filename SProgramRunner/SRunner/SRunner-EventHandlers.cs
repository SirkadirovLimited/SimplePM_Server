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
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SProgramRunner
{
    
    public partial class SRunner
    {

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            //========================================================================================================//
            // PREPARE OUTPUT DATA BEFORE MAIN APPEND PROCESS                                                         //
            //========================================================================================================//
            
            // Ignore if output data is null
            if (e.Data == null)
                return;

            // Adapt program's output if requested
            var dataLine = (_testingRequestStuct.IOConfig.AdaptOutput)
                ? e.Data.TrimEnd()
                : e.Data;

            //========================================================================================================//
            // OUTPUT DATA LIMIT REACHED CHECKER SECTION                                                              //
            //========================================================================================================//
            
            // Define output data limit reached checker
            var outputCharsLimitReached_checker = _testingRequestStuct.IOConfig.OutputCharsLimit > 0 &&
                                                    (
                                                        Encoding.UTF8.GetCharCount(
                                                            _programRunningResult.ProgramOutputData
                                                        ) + dataLine.Length >
                                                            _testingRequestStuct.IOConfig.OutputCharsLimit
                                                    );
            
            // Aggregate checking results
            if (outputCharsLimitReached_checker)
            {
                
                // Set testing results
                TestingExceptionCatched(
                    new ProgramRunnerExceptions.OutputDataLimitReachedException(),
                    TestingResult.OutputErrorResult
                );
                
            }
            
            //========================================================================================================//
            // APPEND NEW LINE TO PROGRAM'S OUTPUT PARAMETER                                                          //
            //========================================================================================================//

            // Some work with encodings due to types incompatibility
            _programRunningResult.ProgramOutputData = Encoding.UTF8.GetBytes(
                    new StringBuilder(
                        Encoding.UTF8.GetString(
                            _programRunningResult.ProgramOutputData
                        )
                    ).AppendLine(dataLine).ToString()
            );

            //========================================================================================================//

        }
        
        private void ProcessOnExited(object sender, EventArgs e)
        {

            //========================================================================================================//
            
            // If program output data is not null, adapt output
            if (_programRunningResult.ProgramOutputData != null)
            {

                // Adapt output if only this required by IO configuration section of testing request.
                if (_testingRequestStuct.IOConfig.AdaptOutput)
                {
                    
                    // Some convertations due to types incompatibility
                    _programRunningResult.ProgramOutputData = Encoding.UTF8.GetBytes(
                        Encoding.UTF8.GetString(
                            _programRunningResult.ProgramOutputData
                        ).TrimEnd()
                    );
                    
                }
                
            }
            else
            {
                
                // Otherwise we need to read output from file
                ReadOutputDataFromFile();
                
            }
            
            //========================================================================================================//
            // INLINE METHOD, THAT READS OUTPUT DATA FROM FILE (WHEN THIS ACTION IS REALLY NEEDED)                    //
            //========================================================================================================//
            
            void ReadOutputDataFromFile()
            {

                // Check if we need to read output data from specified outptu file
                if (!_testingRequestStuct.IOConfig.PreferReadFromOutputFile)
                    return;

                // Form full path to file, that (maybe) contains output data
                var programOutputFilePath = Path.Combine(
                    _testingRequestStuct.RuntimeInfo.WorkingDirectory,
                    _testingRequestStuct.IOConfig.OutputFileName
                );

                // Check if formed file with output data exists
                if (!File.Exists(programOutputFilePath))
                    return;
                
                try
                {

                    // Try to read data from pre-defined and existing output file
                    _programRunningResult.ProgramOutputData = File.ReadAllBytes(programOutputFilePath);

                }
                catch { /* Additional operations not required */ }

            }
            
            //========================================================================================================//

        }
        
    }
    
}