/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

using System;
using System.Text;
using CompilerBase;
using ProgramTesting;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace SimplePM_Server.SimplePM_Tester
{

    internal partial class SimplePM_Tester
    {
        
        /*
         * Метод отвечает за выполнение релизного
         * тестирования пользовательского решения
         * поставленной задачи.
         */

        public ProgramTestingResult Release(
            ref dynamic compilerConfiguration,
            ref ICompilerPlugin compilerPlugin
        )
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

                var currentTestResult = new ProgramTester(
                    ref compilerConfiguration,
                    ref compilerPlugin,
                    exeFileUrl,
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

                if (currentTestResult.Result == TestResult.MiddleSuccessResult)
                {

                    /*
                     * Сравнение выходных потоков
                     * и вынесение  результата по
                     * данному тесту.
                     */
                    currentTestResult.Result = 
                        Convert.ToBase64String(currentTestResult.Output) == Convert.ToBase64String(currentTest.OutputData)
                            ? TestResult.FullSuccessResult
                            : TestResult.FullNoSuccessResult;

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

        /*
         * Метод получает и обрабатывает информацию
         * о релизных тестах для данной задачи.
         */

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
            var cmdSelect = new MySqlCommand(querySelect, connection);

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