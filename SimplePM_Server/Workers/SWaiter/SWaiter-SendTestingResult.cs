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

            /*
             * Указываем в лог-файле о скором
             * завершении  обработки  данного
             * запроса на тестирование.
             */

            logger.Trace(
                "#" +
                _submissionInfo.SubmissionId +
                ": Result is being sent to MySQL server..."
            );
            
            /*
             * Создаём команду для MySQL сервера
             * на     основе     сформированного
             * запроса к базе данных.
             */

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
                Encoding.UTF8.GetBytes(ptResult.GetErrorOutputAsLine())
            ); // Вывод ошибок решения

            updateSqlCommand.Parameters.AddWithValue(
                "@param_output",
                ptResult.TestingResults[ptResult.TestingResults.Length - 1].Output
            ); // Вывод решения

            updateSqlCommand.Parameters.AddWithValue(
                "@param_exitcodes",
                ptResult.GetExitCodesAsLine('|')
            ); // Коды выхода решения
            
            updateSqlCommand.Parameters.AddWithValue(
                "@param_result",
                ptResult.GetResultAsLine('|')
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