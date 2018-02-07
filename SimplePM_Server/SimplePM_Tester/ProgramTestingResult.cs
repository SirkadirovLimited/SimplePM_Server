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
/*! \file */

using System;
using System.Linq;

namespace SimplePM_Server.SimplePM_Tester
{

    /*!
    * \brief
    * Класс, который является хостом для результатов
    * тестирования пользовательского решения поставленной
    * задачи по программированию.
    */

    internal class ProgramTestingResult
    {

        ///////////////////////////////////////////////////
        /// Раздел объявления необходимых переменных.
        ///////////////////////////////////////////////////

        public Test[] TestingResults; //!< Массив, содержащий информацию о тестировании пользовательского решения

        ///////////////////////////////////////////////////
        /// Метод является переопределением стандартного
        /// конструктора текущего класса.
        ///////////////////////////////////////////////////

        public ProgramTestingResult(int testsCount)
        {

            // Инициализируем массив результатов тестирования
            TestingResults = new Test[testsCount];

        }

        ///////////////////////////////////////////////////
        /// Метод возвращает строку, которая содержит
        /// информацию о результатах тестирования
        /// пользовательской программы.
        ///////////////////////////////////////////////////

        public string GetResultAsLine(char splitter)
        {

            /*
             * Объявляем временную переменную, в которой
             * будем хранить результат работы метода
             **/
            string resultLine;

            try
            {

                // С помощью LINQ запроса формируем строку результата
                resultLine = TestingResults.Aggregate("", (current, test) => current + (test.Result + splitter));

                /*
                 * Удаляем все ненужные символы
                 **/
                resultLine = resultLine.Trim('\n', '\r');

            }
            catch (Exception)
            {

                // Если что-то сломалось, возвращаем пустоту
                resultLine = "";

            }

            // Возвращаем сформированную строку
            return resultLine;

        }

        ///////////////////////////////////////////////////
        /// Метод возвращает строку, которая содержит
        /// информацию о кодах выхода пользовательского
        /// процесса на всех тестах.
        ///////////////////////////////////////////////////

        public string GetExitCodesAsLine(char splitter)
        {

            /*
             * Объявляем временную переменную, в которой
             * будем хранить результат работы метода
             **/
            string resultLine;

            try
            {

                // С помощью LINQ запроса формируем строку результата
                resultLine = TestingResults.Aggregate("", (current, test) => current + (test.ExitCode + splitter));

                /*
                 * Удаляем все ненужные символы
                 **/
                resultLine = resultLine.Trim('\n', '\r');

            }
            catch (Exception)
            {

                // Если что-то сломалось, возвращаем пустоту
                resultLine = "";

            }

            // Возвращаем сформированную строку
            return resultLine;

        }

        ///////////////////////////////////////////////////
        /// Метод возвращает строку, которая содержит
        /// информацию об использованной процессом памяти
        /// во время каждого теста.
        ///////////////////////////////////////////////////

        public string GetUsedMemoryAsLine(char splitter)
        {

            /*
             * Объявляем временную переменную, в которой
             * будем хранить результат работы метода
             **/
            string resultLine;

            try
            {

                // С помощью LINQ запроса формируем строку результата
                resultLine = TestingResults.Aggregate("", (current, test) => current + (test.UsedMemory + splitter));

                /*
                 * Удаляем все ненужные символы
                 **/
                resultLine = resultLine.Trim('\n', '\r');

            }
            catch (Exception)
            {

                // Если что-то сломалось, возвращаем пустоту
                resultLine = "";

            }

            // Возвращаем сформированную строку
            return resultLine;

        }

        ///////////////////////////////////////////////////
        /// Метод возвращает строку, которая содержит
        /// информацию об использованной процессом
        /// процессорного времени во время каждого теста.
        ///////////////////////////////////////////////////

        public string GetUsedProcessorTimeAsLine(char splitter)
        {

            /*
             * Объявляем временную переменную, в которой
             * будем хранить результат работы метода
             **/
            string resultLine;

            try
            {

                // С помощью LINQ запроса формируем строку результата
                resultLine = TestingResults.Aggregate("", (current, test) => current + (test.UsedProcessorTime + splitter));

                /*
                 * Удаляем все ненужные символы
                 **/
                resultLine = resultLine.Trim(splitter);

            }
            catch (Exception)
            {

                // Если что-то сломалось, возвращаем пустоту
                resultLine = "";

            }

            // Возвращаем сформированную строку
            return resultLine;

        }

        ///////////////////////////////////////////////////
        /// Метод возвращает строку исключений тестируемой
        /// программы, которая собрана со всех тестов
        ///////////////////////////////////////////////////

        public string GetErrorOutputAsLine()
        {

            /*
             * Объявляем временную переменную, в которой
             * будем хранить результат работы метода
             **/
            string resultLine;

            try
            {

                // С помощью LINQ запроса формируем строку результата
                resultLine = TestingResults.Aggregate("", (current, test) => current + (test.ErrorOutput + "\r\n"));

                /*
                 * Удаляем все ненужные символы
                 **/
                resultLine = resultLine.Trim('\n', '\r');

            }
            catch (Exception)
            {

                // Если что-то сломалось, возвращаем пустоту
                resultLine = "";

            }

            // Возвращаем сформированную строку
            return resultLine;

        }

        ///////////////////////////////////////////////////
        /// Метод возвращает булевое значение, которое
        /// гласит о том, успешно ли пройдены все тесты,
        /// или нет. Ошибок, вроде, не должно возникать.
        ///////////////////////////////////////////////////

        public bool IsSuccessful()
        {

            /*
             * Выполняем LINQ запрос на выборку и проверку, после
             * чего возвращаем результат выполнения метода.
             **/
            return TestingResults.Aggregate(true, (current, test) => current && test.IsSuccessful);

        }

        ///////////////////////////////////////////////////

    }
    
}