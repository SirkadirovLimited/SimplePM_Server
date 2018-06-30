﻿/*
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

            /*
             * Получаем информацию о тестах
             * для     release-тестирования
             * пользовательского    решения
             * посталвенной задачи.
             */

            var ReleaseTestsInfo = GetTestsInfo();

            /*
             * Сохраняем первоначальное количество
             * тестов в отдельной переменной, дабы
             * не затерять   эту   информацию  при
             * видоизменении очереди.
             */

            var testsCount = ReleaseTestsInfo.Count;

            /*
             * Создаём объект,  который будет
             * хранить  результаты  релизного
             * тестирования пользовательского
             * решения  поставленной  задачи.
             */

            var programTestingResult = new ProgramTestingResult(testsCount);

            /*
             * Получаем ссылку на объект, который
             * хранит информацию  о  конфигурации
             * компиляционного модуля для данного
             * языка программирования.
             */
            
            var languageConfiguration = SCompiler.GetCompilerConfig(
                submissionInfo.CodeLang
            );
            
            /*
             * Получаем     ссылку     на     объект,
             * созданный    на    основании   класса,
             * который,   в   свою  очередь,   создан
             * по подобию интерфейса ICompilerPlugin.
             */
            
            var compilerPlugin = SCompiler.FindCompilerPlugin(
                (string)(languageConfiguration.module_name)
            );
            
            /*
             * В цикле  тестируем  пользовательское
             * решение   поставленной   задачи   на
             * заранее  подготовленных   тестах   и
             * записываем  результаты  проверки  по
             * каждому тесту в специально созданный
             * для этих нужд массив.
             */

            for (var currentTestIndex = 0; currentTestIndex < testsCount; currentTestIndex++)
            {

                /*
                 * Делаем выборку  данных о текущем
                 * релизном тесте из очереди тестов
                 */

                var currentTest = ReleaseTestsInfo.Dequeue();

                /*
                 * Запускаем  тестирование  пользовательского
                 * решения поставленной задачи  на  указанном
                 * тесте и получаем  промежуточные результаты
                 * тестирования, заносим их в соответственную
                 * переменную,  созданную специально для этих
                 * нужд.
                 */

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

                /*
                 * Если временный  результат  успешен,
                 * проводим   финальную   перепроверку
                 * результата тестирования  на  данном
                 * тесте и выносим финальный результат
                 * данного теста.
                 */

                if (currentTestResult.Result == SingleTestResult.PossibleResult.MiddleSuccessResult)
                {

                    /*
                     * Сравнение выходных потоков
                     * и вынесение  результата по
                     * данному тесту.
                     */
                    currentTestResult.Result = 
                        Convert.ToBase64String(currentTestResult.Output) == Convert.ToBase64String(currentTest.OutputData)
                            ? SingleTestResult.PossibleResult.FullSuccessResult
                            : SingleTestResult.PossibleResult.FullNoSuccessResult;

                }

                /*
                 * Заносим результат проведения
                 * текущего теста в специальный
                 * массив.
                 */
                
                programTestingResult.TestingResults[currentTestIndex] = currentTestResult;

            }

            /*
             * Возвращаем информацию  о  тестировании
             * пользовательского решения поставленной
             * задачи.
             */

            return programTestingResult;

        }
        
        private Queue<ReleaseTestInfo.ReleaseTestInfo> GetTestsInfo()
        {

            /*
             * Производим выборку всех данных,
             * необходимых для формирования
             * релизных тестов пользовательского
             * решения поставленной задачи
             */
            
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

            /*
             * Для хранения и использования
             * информации о релизных тестах
             * пользовательского    решения
             * поставленной  задачи создаём
             * специальную очередь, которая
             * будет хранить объекты содер-
             * жащие  информацию о релизных
             * тестах для заданной  задачи.
             */
            
            var testsQueue = new Queue<ReleaseTestInfo.ReleaseTestInfo>();

            /*
             * В цикле занимаемся обработкой
             * каждого    релизного    теста
             * пользовательских      решений
             * и вносим  его  в  специальную
             * очередь тестов.
             */

            while (dataReader.Read())
            {

                /*
                 * Создаём новый объект, содержащий
                 * информацию об одном из  релизных
                 * тестов пользовательских решений.
                 */

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

            /*
             * Возвращаем специально
             * созданную     очередь
             * тестов  для  заданной
             * задачи.
             */
            
            return testsQueue;

        }
        
    }
    
}