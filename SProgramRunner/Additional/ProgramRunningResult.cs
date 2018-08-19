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

namespace SProgramRunner
{
    
    /// <summary>
    /// Definjes all information about running result
    /// of specified in <code>TestingRequestStruct</code>
    /// struct process.
    /// </summary>
    /// <seealso cref="TestingRequestStuct"/>
    public class ProgramRunningResult
    {

        /// <summary>
        /// Process runtime resources usage statistics.
        /// </summary>
        /// <seealso cref="ProcessResourcesUsageStatsStruct"/>
        public ProcessResourcesUsageStatsStruct ProcessResourcesUsageStats;

        /// <summary>
        /// Output data of specified process as bytes array
        /// </summary>
        public byte[] ProgramOutputData { get; set; }

        /// <summary>
        /// Data from process standard error output (<code>STDERR</code>).
        /// </summary>
        public string ProgramErrorData { get; set; }

        /// <summary>
        /// Exit code of the specified process.
        /// </summary>
        public int ProgramExitCode { get; set; }

        /// <summary>
        /// Result on current run.
        /// </summary>
        /// <see cref="TestingResult"/>
        public char Result { get; set; }
        
        /// <summary>
        /// Check if current run is middle successful.
        /// </summary>
        /// <remarks>
        /// Analog of manual check for <code>Result</code> equals to <code>TestingResult.MiddleSuccessResult</code>.
        /// </remarks>
        /// <see cref="TestingResult"/>
        /// <seealso cref="TestingResult.MiddleSuccessResult"/>
        public bool IsMiddleSuccessful => (
            Result == TestingResult.MiddleSuccessResult
        );
        
        /// <summary>
        /// Check if current run is fully succesful.
        /// </summary>
        /// <remarks>
        /// Analog of manual check for <code>Result</code> equals to <code>TestingResult.FullSuccessResult</code>.
        /// </remarks>
        /// <see cref="TestingResult"/>
        /// <seealso cref="TestingResult.FullSuccessResult"/>
        public bool IsFullySuccessful => (
            Result == TestingResult.FullSuccessResult
        );

    }
    
}