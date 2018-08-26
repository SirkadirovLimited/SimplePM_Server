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

using System.Security;
using System.Diagnostics;

namespace SProgramRunner
{
    
    /// <summary>
    /// Use when you need to run your program
    /// with <code>SProgramRunner</code>.
    /// </summary>
    /// <seealso cref="SProgramRunner"/>
    public struct TestingRequestStuct
    {
        
        //============================================================================================================//
        
        /// <summary>
        /// Information about program.
        /// </summary>
        /// <seealso cref="ProcessRuntimeInfo"/>
        public ProcessRuntimeInfo RuntimeInfo { get; set; }
        
        /// <summary>
        /// Information about program struct.
        /// </summary>
        public struct ProcessRuntimeInfo
        {
            
            /// <summary>
            /// Full (or Path-based) path to program.
            /// </summary>
            public string FileName { get; set; }
            
            /// <summary>
            /// Full (or non-recommended relative) path
            /// to program's working directory.
            /// </summary>
            /// <remarks>
            /// All path
            /// parameters in <code>TestingRequestStruct</code>
            /// are relative to it, but this not true for
            /// <code>FileName</code> field.
            /// </remarks>
            /// <seealso cref="FileName"/>
            public string WorkingDirectory { get; set; }
            
            /// <summary>
            /// Program running arguments
            /// </summary>
            public string Arguments { get; set; }

        }
        
        //============================================================================================================//

        /// <summary>
        /// Run progam as another user information.
        /// </summary>
        /// <seealso cref="ProcessRunAsInfo"/>
        public ProcessRunAsInfo RunAsInfo { get; set; }
        
        /// <summary>
        /// Run program as another user information struct.
        /// </summary>
        public struct ProcessRunAsInfo
        {

            /// <summary>
            /// Enable or not run as other user feature.
            /// </summary>
            public bool Enable { get; set; }

            /// <summary>
            /// Run program as user with specified username.
            /// If Run As feature enabled, this mustn't be empty!
            /// </summary>
            /// <remarks>
            /// Can be null if <code>Enabled</code> is <code>false</code>.
            /// </remarks>
            /// <seealso cref="Enable"/>
            public string UserName { get; set; }

            /// <summary>
            /// Password of user with username specified in
            /// <code>UserName</code> field of this struct.
            /// </summary>
            /// <remarks>
            /// - Can be null if <code>Enabled</code> is <code>false</code>.
            /// - Not applicable if running on Unix!
            /// </remarks>
            /// <seealso cref="Enable"/>
            /// <seealso cref="UserName"/>
            public SecureString UserPassword { get; set; }

        }
        
        //============================================================================================================//

        /// <summary>
        /// Set process limits information.
        /// </summary>
        /// <seealso cref="ProcessLimitsInfo"/>
        public ProcessLimitsInfo LimitsInfo { get; set; }
        
        /// <summary>
        /// Use if when you need to limit process resources
        /// and/or if you want to add custom actions at
        /// process limiting resources stage.
        /// </summary>
        public struct ProcessLimitsInfo
        {
            
            /// <summary>
            /// Enable or not process limits feature.
            /// </summary>
            /// <value>true or false</value>
            public bool Enable { get; set; }

            /// <summary>
            /// Processor using time limit in milliseconds.
            /// </summary>
            /// <value>Set value <code>-1</code> to unlimited.</value>
            /// <remarks>
            /// - Used only when <code>Enabled</code> equals to <code>true</code>.
            /// - Non-hard limit, cross-platform!
            /// </remarks>
            /// <seealso cref="Enable"/>
            /// <seealso cref="System.Diagnostics.Process"/>
            public int ProcessorTimeLimit { get; set; }

            /// <summary>
            /// Process memory limit in bytes
            /// </summary>
            /// <value>Set value <code>-1</code> to unlimited.</value>
            /// <remarks>
            /// - Used only when <code>Enabled</code> equals to <code>true</code>.
            /// - Non-hard limit, cross-platform!
            /// </remarks>
            /// <seealso cref="Enable"/>
            /// <seealso cref="System.Diagnostics.Process"/>
            public long ProcessWorkingSetLimit { get; set; }

            /// <summary>
            /// Set process real working time limit.
            /// </summary>
            /// <value>Set value <code>-1</code> to unlimited.</value>
            /// <remarks>
            /// - Used only when <code>Enabled</code> equals to <code>true</code>.
            /// - Non-ard limit, cross-platform!
            /// - Process can work more time than specified here.
            ///   Get real working time by using one of parameters
            ///   in testing results objects, returned by <code>SRunner</code>.
            /// </remarks>
            /// <seealso cref="Enable"/>
            /// <seealso cref="System.Diagnostics.Process"/>
            public int ProcessRealWorkingTimeLimit { get; set; }
            
            /// <summary>
            /// Useful when you want to do custom actions
            /// for limiting process resources or environment.
            /// Also can be used for defining custom actions at
            /// process limiting stage.
            /// </summary>
            /// <remarks>
            /// - Used only when <code>Enabled</code> equals to <code>true</code>.
            /// - Runs after all standard limiting actions
            /// - Useful when starts a <code>Thread</code> or a <code>Task</code>.
            /// - You need to think about cross-platform compatibility!
            /// - Reserved for future use
            /// </remarks>
            /// <seealso cref="Enable"/>
            /// <seealso cref="System.Threading"/>
            /// <seealso cref="System.Threading.Tasks"/>
            /// <param name="proc">Reference to a running process</param>
            /// TODO
            public delegate void CutomLimitingAction(ref Process proc);

        }
        
        //============================================================================================================//

        /// <summary>
        /// Set input and output configuration for specified process.
        /// </summary>
        /// <seealso cref="ProcessIOConfig"/>
        public ProcessIOConfig IOConfig { get; set; }
        
        /// <summary>
        /// Input and output configuration for specified process.
        /// </summary>
        public struct ProcessIOConfig
        {

            /// <summary>
            /// Set this variable to inject custom input data to starting process.
            /// </summary>
            public byte[] ProgramInput { get; set; }
            
            /// <summary>
            /// Set to <code>true</code> if you would like that SProgramRunner
            /// create file in specified program's directory, that contains
            /// duplicate of program's input stream. Otherwise set to <code>false</code>.
            /// </summary>
            /// <seealso cref="InputFileName"/>
            public bool WriteInputToFile { get; set; }
            
            /// <summary>
            /// Name of input file.
            /// Require field <code>WriteInputToFile</code> set to <code>true</code>.
            /// </summary>
            /// <seealso cref="WriteInputToFile"/>
            public string InputFileName { get; set; }
            
            /// <summary>
            /// Prefer (enable) to read output from specified
            /// in field <code>OutputFileName</code> file, that
            /// reading from program's standard output stream.
            /// </summary>
            /// <seealso cref="OutputFileName"/>
            public bool PreferReadFromOutputFile { get; set; }

            /// <summary>
            /// Specify output file name, in which you think
            /// specified program will store result of it's
            /// work. If specified file not found, we'll use
            /// data from <code>stdout</code>.
            /// </summary>
            public string OutputFileName { get; set; }
            
            /// <summary>
            /// Limit number of characters that program can write
            /// to <code>stdout</code> or/and to specified output file.
            /// </summary>
            /// <value>Set value <code>-1</code> to unlimited.</value>
            /// <seealso cref="OutputFileName"/>
            public int OutputCharsLimit { get; set; }

        }
        
        //============================================================================================================//
        
    }
    
}