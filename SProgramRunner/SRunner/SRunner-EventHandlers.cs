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

using System.IO;
using System.Text;
using System.Diagnostics;

namespace SProgramRunner
{
    
    public partial class SRunner
    {

        private void ProcessOnExited()
        {

            // Is output data read from file or not checker
            var isOutputReadFromFile = false;
            
            // Try to read from file when this is preferred way to read output data
            if (_testingRequestStuct.IOConfig.PreferReadFromOutputFile)
            {
                
                // We need to read output from file
                isOutputReadFromFile = ReadOutputDataFromFile();
                
            }

            // Adapt STDOUT output only if we read from it, not from file
            if (!isOutputReadFromFile)
            {

                // Canceloutput reading process and get output data
                _programRunningResult.ProgramOutputData = _outputStreamReader.KillAndGet();
                
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

            // If output data limit enabled, check for it.
            if (_testingRequestStuct.IOConfig.OutputCharsLimit > 0)
            {

                // If output characters limit reached
                if (Encoding.UTF8.GetCharCount(_programRunningResult.ProgramOutputData) >
                    _testingRequestStuct.IOConfig.OutputCharsLimit)
                {
                    
                    // Output data limit reached, so we need to set new testing result
                    SetNewTestingResult(TestingResult.OutputErrorResult, false);
                    
                }
                
            }
            
            //========================================================================================================//
            // INLINE METHOD, THAT READS OUTPUT DATA FROM FILE (WHEN THIS ACTION IS REALLY NEEDED)                    //
            //========================================================================================================//
            
            bool ReadOutputDataFromFile()
            {

                // Form full path to file, that (maybe) contains output data
                var programOutputFilePath = Path.Combine(
                    _testingRequestStuct.RuntimeInfo.WorkingDirectory,
                    _testingRequestStuct.IOConfig.OutputFileName
                );

                // Check if formed file with output data exists
                if (!File.Exists(programOutputFilePath))
                    return false;

                try
                {

                    // Try to read data from pre-defined and existing output file
                    _programRunningResult.ProgramOutputData = File.ReadAllBytes(programOutputFilePath);

                }
                catch
                {
                    /* Additional operations not required */
                }

                // Signal that we read from file, not from STDOUT
                return true;

            }
            
            //========================================================================================================//

        }
        
    }
    
}