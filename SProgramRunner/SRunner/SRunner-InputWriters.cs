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
using System.IO;
using System.Text;

namespace SProgramRunner
{
    
    public partial class SRunner
    {

        private void WriteInputData_StandardInputStream()
        {

            try
            {

                // Write input data to program's STDIN
                _process.StandardInput.Write(
                    Encoding.UTF8.GetString(_testingRequestStuct.IOConfig.ProgramInput)
                );

                // Clear all buffers and write pending data to stream
                _process.StandardInput.Flush();
                
                // Close program's standard input stream
                _process.StandardInput.Close();

            }
            catch (Exception ex)
            {
                
                // Call method, that specifies new testing result, writes exception and kills associated process.
                SetNewTestingResult(ex, TestingResult.InputErrorResult);
                
            }
            
        }

        private void WriteInputData_File()
        {

            // Write input data to specified file only when this feature enabled.
            if (!_testingRequestStuct.IOConfig.WriteInputToFile)
                return;

            try
            {

                // Form full path to a new file, that will be input data file.
                var inputDataFilePath = Path.Combine(
                    _testingRequestStuct.RuntimeInfo.WorkingDirectory,
                    _testingRequestStuct.IOConfig.InputFileName
                );

                // Create file at specified path
                File.Create(inputDataFilePath).Close();
                
                // Write program input data to newly created file
                File.WriteAllBytes(inputDataFilePath, _testingRequestStuct.IOConfig.ProgramInput);

            }
            catch (Exception ex)
            {

                // Call method, that specifies new testing result and writes exception.
                SetNewTestingResult(ex, TestingResult.InputErrorResult, false);

            }

        }
        
    }
    
}