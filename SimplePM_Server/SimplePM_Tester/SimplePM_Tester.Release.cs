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
using System.Collections.Generic;
using System.Text;
using System.Web;
using MySql.Data.MySqlClient;
using ReleaseTestInfo;

namespace SimplePM_Server.SimplePM_Tester
{

    internal partial class SimplePM_Tester
    {
        

        public ProgramTestingResult Release()
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
                    ref sConfig,
                    ref sCompilersConfig,
                    ref _compilerPlugins,
                    submissionInfo.CodeLang,
                    exeFileUrl,
                    "",
                    currentTest.MemoryLimit,
                    currentTest.ProcessorTimeLimit,
                    Encoding.UTF8.GetString(currentTest.InputData),
                    currentTest.OutputData.Length * 2,
                    submissionInfo.AdaptProgramOutput
                ).RunTesting();

                /*
                 * Заносим результат проведения
                 * текущего теста в специальный
                 * массив.
                 */
                programTestingResult.TestingResults[currentTestIndex] = currentTestResult;

            }

            throw new NotImplementedException();

        }

        private Queue<ReleaseTest> GetTestsInfo()
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

            // Отправляем запрос на сервер БД
            var cmdSelect = new MySqlCommand(querySelect, connection);
            cmdSelect.Parameters.AddWithValue("@problemId", submissionInfo.ProblemId);

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
            var testsQueue = new Queue<ReleaseTest>();

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
                var testInfo = new ReleaseTest(

                    // Уникальный идентификатор теста
                    int.Parse(
                        HttpUtility.HtmlDecode(dataReader["id"].ToString())
                    ),

                    // Входные данные
                    (byte[])dataReader["input"],

                    // Выходные данные
                    (byte[])dataReader["output"],

                    // Лимит используемой памяти
                    long.Parse(
                        HttpUtility.HtmlDecode(dataReader["memoryLimit"].ToString())
                    ),

                    // Лимит используемого процессорного времени
                    int.Parse(
                        HttpUtility.HtmlDecode(dataReader["timeLimit"].ToString())
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