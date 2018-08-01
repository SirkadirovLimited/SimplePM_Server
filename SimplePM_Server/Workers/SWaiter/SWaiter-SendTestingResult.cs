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
using System.Text;
using CompilerPlugin;
using MySql.Data.MySqlClient;
using ProgramTestingAdditions;

namespace SimplePM_Server.Workers
{
    
    public partial class SWaiter
    {
        
        private void SendTestingResult(ref ProgramTestingResult ptResult, CompilationResult cResult)
        {

            logger.Trace(
                "#" +
                _submissionInfo.SubmissionId +
                ": Result is being sent to MySQL server..."
            );
            
            // Команда на обновление БД
            var updateSqlCommand = new MySqlCommand(
                Resources.submission_result_query,
                _connection
            );

            /*
             * Указываем параметры выше сформированного
             * запроса к базе данных.
             */
            
            updateSqlCommand.Parameters.AddWithValue(
                "@param_submissionId",
                _submissionInfo.SubmissionId
            ); // Идентификатор запроса

            updateSqlCommand.Parameters.AddWithValue(
                "@param_testType",
                _submissionInfo.TestType
            ); // Тип тестирования

            updateSqlCommand.Parameters.AddWithValue(
                "@param_hasError",
                Convert.ToInt32(cResult.HasErrors)
            ); // Сигнал об ошибке при компиляции

            updateSqlCommand.Parameters.AddWithValue(
                "@param_compiler_text",
                cResult.CompilerOutput
            ); // Вывод компилятора
            
            updateSqlCommand.Parameters.AddWithValue(
                "@param_errorOutput",
                Encoding.UTF8.GetBytes(
                    ptResult.GetTestingResultInfoString(ProgramTestingResult.TestingResultInfo.ErrorOutput, '\n')
                )
            ); // Вывод ошибок решения

            updateSqlCommand.Parameters.AddWithValue(
                "@param_output",
                ptResult.TestingResults[ptResult.TestingResults.Length - 1].Output
            ); // Вывод решения

            updateSqlCommand.Parameters.AddWithValue(
                "@param_exitcodes",
                ptResult.GetTestingResultInfoString(ProgramTestingResult.TestingResultInfo.ExitCodes)
            ); // Коды выхода решения
            
            updateSqlCommand.Parameters.AddWithValue(
                "@param_result",
                ptResult.GetTestingResultInfoString(ProgramTestingResult.TestingResultInfo.TestsResults)
            ); // Потестовые результаты решения
            
            updateSqlCommand.Parameters.AddWithValue(
                "@param_rating",
                GetSolutionRating(ref ptResult)
            ); // Полученный рейтинг за решение

            // Выполняем запрос к базе данных
            updateSqlCommand.ExecuteNonQuery();

        }
        
    }
    
}