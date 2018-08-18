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

namespace SProgramRunner
{
    
    public partial class SRunner
    {

        private void WriteInputData_StandardInputStream()
        {
            
            throw new NotImplementedException();
            
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

                // Write exception information to program error data param (testing result section).
                _programRunningResult.ProgramErrorData = ex.ToString();
                
                // Now we have a new program running result - Input error result.
                _programRunningResult.Result = TestingResult.InputErrorResult;

            }

        }
        
    }
    
}