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
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.STester
{
    
    public abstract class TestingType
    {

        internal MySqlConnection conn { get; }
        internal string exeFilePath { get; }
        internal SubmissionInfo.SubmissionInfo submissionInfo { get; }

        protected TestingType(
            ref MySqlConnection conn,
            string exeFilePath,
            ref SubmissionInfo.SubmissionInfo submissionInfo
        )
        {

            this.conn = conn;
            this.exeFilePath = exeFilePath;
            this.submissionInfo = submissionInfo;

        }

        public abstract ProgramTestingResult RunTesting();

        internal void MakeFinalTestResult(ref SingleTestResult singleTestResult, byte[] rightOutputData)
        {
            
            // Действуем лишь в случае необходимости вынесения дополнительных итогов
            if (singleTestResult.Result == SingleTestResult.PossibleResult.MiddleSuccessResult)
            {

                // TODO: Implement checkers [SERVER-28]
                
                // Сравнение выходных потоков и вынесение  результата по данному тесту
                singleTestResult.Result = 
                    singleTestResult.Output.SequenceEqual(rightOutputData)
                        ? SingleTestResult.PossibleResult.FullSuccessResult
                        : SingleTestResult.PossibleResult.FullNoSuccessResult;
                
            }
            
        }

    }
    
}