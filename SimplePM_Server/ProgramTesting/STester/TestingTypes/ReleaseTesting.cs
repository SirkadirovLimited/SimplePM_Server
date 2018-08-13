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

using NLog;
using System.Text;
using MySql.Data.MySqlClient;
using ProgramTestingAdditions;
using System.Collections.Generic;
using SimplePM_Server.Workers.Recourse;
using SimplePM_Server.ProgramTesting.SRunner;

namespace SimplePM_Server.ProgramTesting.STester
{
    
    public class ReleaseTesting : TestingType
    {
        
        private static readonly Logger logger = LogManager.GetLogger("SimplePM_Server.ProgramTesting.STester.ReleaseTesting");

        public ReleaseTesting(
            ref MySqlConnection conn,
            string exeFilePath,
            ref SubmissionInfo.SubmissionInfo submissionInfo
        ) : base(ref conn, exeFilePath, ref submissionInfo) {  }

        public override ProgramTestingResult RunTesting()
        {

            logger.Trace("#" + submissionInfo.SubmissionId + ": ReleaseTesting.RunTesting() [started]");
            
            // Получаем информацию о всех тестах для данной задачи
            var ReleaseTestsInfo = GetTestsInfo();

            // Запоминаем количество тестов
            var testsCount = ReleaseTestsInfo.Count;

            // Создаём объект, где будем хранить результаты тестирования
            var programTestingResult = new ProgramTestingResult(testsCount);
            
            // Определяем конфигурацию компиляционного плагина
            var languageConfiguration = SCompiler.GetCompilerConfig(
                submissionInfo.UserSolution.ProgrammingLanguage
            );
            
            // Получаем экземпляр компиляционного плагина
            var compilerPlugin = SCompiler.FindCompilerPlugin(
                (string)(languageConfiguration.module_name)
            );
            
            // Производим тестирования пользовательского решения поставленной задачи по всем тестам
            for (var currentTestIndex = 0; currentTestIndex < testsCount; currentTestIndex++)
            {

                // Получаем информацию о текущем тесте
                var currentTest = ReleaseTestsInfo.Dequeue();

                // Запускаем тестирование пользовательской программы на данном тесте
                SingleTestResult currentTestResult = new ProgramExecutor(
                    languageConfiguration,
                    compilerPlugin,
                    exeFilePath,
                    "--user-solution",
                    currentTest.MemoryLimit,
                    currentTest.ProcessorTimeLimit,
                    currentTest.InputData,
                    Encoding.UTF8.GetString(currentTest.OutputData).Length * 2,
                    submissionInfo.ProblemInformation.AdaptProgramOutput
                ).RunTesting();

                // Выносим финальный вердикт по тесту
                MakeFinalTestResult(ref currentTestResult, currentTest.OutputData);

                // Записываем результат на текущем тесте в массив
                programTestingResult.TestingResults[currentTestIndex] = currentTestResult;

            }

            logger.Trace("#" + submissionInfo.SubmissionId + ": ReleaseTesting.RunTesting() [finished]");
            
            // Возвращаем результаты тестирования пользовательского решения
            return programTestingResult;

        }
        
        private Queue<ReleaseTestInfo.ReleaseTestInfo> GetTestsInfo()
        {
            
            logger.Trace("GetTestsInfo for submission #" + submissionInfo.SubmissionId + " [started]");
            
            // Формируем SQL-запрос
            const string querySelect = @"
                SELECT 
                    * 
                FROM 
                    `spm_problems_tests`
                WHERE 
                    `problemId` = @problemId 
                ORDER BY 
                    `id` ASC
                ;
            ";

            // Создаём запрос на выборку из БД
            var cmdSelect = new MySqlCommand(querySelect, conn);

            // Добавляем параметры запроса
            cmdSelect.Parameters.AddWithValue(
                "@problemId",
                submissionInfo.ProblemInformation.ProblemId
            );

            // Получаем результат выполнения запроса
            var dataReader = cmdSelect.ExecuteReader();

            // Создаём очередь тестов
            var testsQueue = new Queue<ReleaseTestInfo.ReleaseTestInfo>();

            // Обрабатываем полученные тесты для данной задачи
            while (dataReader.Read())
            {

                // Создаём новый тест
                var testInfo = new ReleaseTestInfo.ReleaseTestInfo
                {

                    // Уникальный идентификатор теста
                    Id = int.Parse(
                        dataReader["id"].ToString()
                    ),

                    // Входные данные
                    InputData = (byte[]) dataReader["input"],

                    // Выходные данные
                    OutputData = (byte[]) dataReader["output"],

                    // Лимит используемой памяти
                    MemoryLimit = long.Parse(
                        dataReader["memoryLimit"].ToString()
                    ),

                    // Лимит используемого процессорного времени
                    ProcessorTimeLimit = int.Parse(
                        dataReader["timeLimit"].ToString()
                    )

                };
                
                // Добавляем текущий тест в конец очереди обработки
                testsQueue.Enqueue(testInfo);

            }

            // Завершаем чтение потока
            dataReader.Close();

            logger.Trace("GetTestsInfo for submission #" + submissionInfo.SubmissionId + " [finished]");
            
            // Возвращаем все тесты для данной задачи в виде очереди
            return testsQueue;

        }
        
    }
    
}