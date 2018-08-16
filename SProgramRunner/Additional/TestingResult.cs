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
    /// This public static classtores default running
    /// (or testing) result codes. Use it when you need
    /// to check running result of specified process,
    /// which tested with <code>SProgramRunner</code>.
    /// </summary>
    public static class TestingResult
    {
        
        /// <summary>
        /// Middle succes result is not full success, but partially success.
        /// You need to check output for <code>FullSuccessResult</code> or
        /// <code>FullFailResult</code>.
        /// </summary>
        public const char MiddleSuccessResult = '*';
        
        /// <summary>
        /// Full success, output is right, no errors while testing.
        /// </summary>
        public const char FullSuccessResult = '+';
        
        /// <summary>
        /// Output data not right, but no errors during runtime and no limits reached.
        /// </summary>
        public const char FullFailResult = '-';
        
        /// <summary>
        /// Processor time limit reached during run time.
        /// </summary>
        public const char TimeLimitResult = 'T';
        
        /// <summary>
        /// Peak working set limit reached durng run time.
        /// </summary>
        public const char MemoryLimitResult = 'M';
        
        /// <summary>
        /// Runtime error occured or exitcode not equals <code>0</code>.
        /// Only exitcode <code>0</code> is successful.
        /// </summary>
        public const char RuntimeErrorResult = 'R';
        
        /// <summary>
        /// Standard error stream (<code>STDERR</code>) is not null.
        /// See error output for more information.
        /// </summary>
        public const char ErrorOutputNotNullResult = 'E';
        
        /// <summary>
        /// An error occured during input file (or stream) writing process.
        /// </summary>
        public const char InputErrorResult = 'I';
        
        /// <summary>
        /// Output data limit reached or something wrong with output stream.
        /// </summary>
        public const char OutputErrorResult = 'O';
        
        /// <summary>
        /// SProgramRunner error occured during secified process run time.
        /// </summary>
        public const char ServerErrorResult = 'S';
        
        /// <summary>
        /// Real running time limit reached.
        /// </summary>
        public const char WaitErrorResult = 'W';
        
    }
    
}