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
using System.Collections.Generic;
using System.Text;
using NLog;
using MySql.Data.MySqlClient;

using ProgramTestingAdditions;
using SimplePM_Server.ProgramTesting.SRunner;
using SimplePM_Server.Workers.Recourse;

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
                submissionInfo.CodeLang
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

                // Запускаем пользовательское решение поставленной задачи
                var currentTestResult = new ProgramExecutor(
                    languageConfiguration,
                    compilerPlugin,
                    exeFilePath,
                    "",
                    currentTest.MemoryLimit,
                    currentTest.ProcessorTimeLimit,
                    currentTest.InputData,
                    Encoding.UTF8.GetString(currentTest.OutputData).Length * 2,
                    submissionInfo.ProblemInformation.AdaptProgramOutput
                ).RunTesting();

                // Производим допроверку пользовательского решения на данном тесте
                if (currentTestResult.Result == SingleTestResult.PossibleResult.MiddleSuccessResult)
                {

                    // TODO: Implement checkers
                    // TODO: Одинаковые части кода с DebugTesting!
                    
                    // Сравнение выходных потоков и вынесение  результата по данному тесту
                    currentTestResult.Result = 
                        Convert.ToBase64String(currentTestResult.Output) == Convert.ToBase64String(currentTest.OutputData)
                            ? SingleTestResult.PossibleResult.FullSuccessResult
                            : SingleTestResult.PossibleResult.FullNoSuccessResult;

                }

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
                var testInfo = new ReleaseTestInfo.ReleaseTestInfo(

                    // Уникальный идентификатор теста
                    int.Parse(
                        dataReader["id"].ToString()
                    ),

                    // Входные данные
                    (byte[])dataReader["input"],

                    // Выходные данные
                    (byte[])dataReader["output"],

                    // Лимит используемой памяти
                    long.Parse(
                       dataReader["memoryLimit"].ToString()
                    ),

                    // Лимит используемого процессорного времени
                    int.Parse(
                        dataReader["timeLimit"].ToString()
                    )

                );
                
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