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