/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server is a part of software product "Automated
 * vefification system for programming tasks "SimplePM".
 *
 * Copyright 2018 Yurij Kadirov
 *
 * Source code of the product licensed under the Apache License,
 * Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Visit website for more details: https://spm.sirkadirov.com/
 */

using NLog;
using System.Text;
using MySql.Data.MySqlClient;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.STester
{
    
    public class SyntaxTesting : TestingType
    {
        
        private static readonly Logger logger = LogManager.GetLogger("SimplePM_Server.ProgramTesting.STester.SyntaxTesting");

        public SyntaxTesting(
            ref MySqlConnection conn,
            string exeFilePath,
            ref SubmissionInfo.SubmissionInfo submissionInfo
        ) : base(ref conn, exeFilePath, ref submissionInfo) {  }

        public override ProgramTestingResult RunTesting()
        {
            
            return new ProgramTestingResult(1)
            {

                TestingResults =
                {

                    [0] = new SingleTestResult
                    {

                        // Выходные данные заполняем кракозябрами
                        Output = Encoding.UTF8.GetBytes("NULL"),

                        // Выходные данные исключений устанавливаем в null
                        ErrorOutput = null,

                        // Код выхода - стандартный
                        ExitCode = 0,

                        // Результатом будет промежуточный успешный
                        Result = SingleTestResult.PossibleResult.MiddleSuccessResult,

                        // Использованная память
                        UsedMemory = 0,

                        // Использованное процессорное время
                        UsedProcessorTime = 0

                    }

                }

            };
            
        }
        
    }
    
}