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

namespace ProgramTestingAdditions
{

    /// <summary>
    /// Класс, содержащий свойства и методы, необходимые для обработки
    /// результатов тестирования пользовательской программы.
    /// </summary>
    public class ProgramTestingResult
    {

        /// <summary>
        /// Массив результатов тестирования
        /// </summary>
        public SingleTestResult[] TestingResults { get; set; }

        /// <summary>
        /// Количество тестов
        /// </summary>
        public int TestsCount => TestingResults.Length;

        /// <summary>
        /// Количество успешно пройденных тестов.
        /// </summary>
        /// <returns>Возвращает количество успешно пройденных тестов.</returns>
        public int PassedTestsCount()
        {

            /*
             * Вычисляем сумму IsSuccessful значений
             * всех элементов массива TestingResults
             * и возвращаем полученное значение.
             */

            return TestingResults.Sum(test => Convert.ToInt32(test.IsSuccessful));

        }

        /// <summary>
        /// Главный конструктор класса результатов тестирования.
        /// Инициализирует массив результатов <c>SingleTestResult</c>
        /// </summary>
        /// <param name="testsCount">Количество тестов</param>
        public ProgramTestingResult(int testsCount)
        {

            // Инициализируем массив результатов тестирования
            TestingResults = new SingleTestResult[testsCount];

        }

        /// <summary>
        /// Перечисление для работы метода <c>GetTestingResultInfoString(...)</c>
        /// </summary>
        public enum TestingResultInfo
        {
            
            TestsResults,
            
            UsedMemory,
            UsedProcessorTime,
            
            ErrorOutput,
            ExitCodes
            
        }

        /// <summary>
        /// Метод возвращает запрашиваемые данные о результатах
        /// тестирования в виде строки с разделителями.
        /// </summary>
        /// <param name="queriedInfo">Запрашиваемая информация (единственный параметр)</param>
        /// <param name="splitter">Разделитель</param>
        /// <returns>
        /// В случае успеха возвращает запрашиваемые данные
        /// в виде строки с разделителями, иначе возвращает
        /// пустую строку.
        /// </returns>
        public string GetTestingResultInfoString(TestingResultInfo queriedInfo, char splitter = '|')
        {

            // Объявляем переменную результата выполнения операции
            string resultString;

            try
            {

                // Получаем результат выполнения операции
                resultString = TestingResults.Aggregate("", (current, test) =>
                {

                    // Объявляем временную переменную результата выполнения операции
                    string aggTmpString;
                    
                    // В зависимости от запрашиваемой информации получаем те или иные значения
                    switch (queriedInfo)
                    {
                        
                        // Результат по текущему тесту
                        case TestingResultInfo.TestsResults:
                            aggTmpString = test.Result.ToString();
                            break;
                        
                        // Использованная память
                        case TestingResultInfo.UsedMemory:
                            aggTmpString = test.UsedMemory.ToString();
                            break;
                        
                        // Использованное процессорное время
                        case TestingResultInfo.UsedProcessorTime:
                            aggTmpString = test.UsedProcessorTime.ToString();
                            break;
                        
                        // Данные с потока stderr
                        case TestingResultInfo.ErrorOutput:
                            aggTmpString = test.ErrorOutput;
                            break;
                        
                        // Выходной код программы на тесте
                        case TestingResultInfo.ExitCodes:
                            aggTmpString = test.ExitCode.ToString();
                            break;
                        
                        // Обработка исключительной ситуации
                        default:
                            aggTmpString = string.Empty;
                            break;
                        
                    }

                    // Возвращаем результат выполнения операции
                    return current + aggTmpString + splitter.ToString();

                });

            }
            catch
            {

                // В исключительной ситуации возвращаем пустую строку
                resultString = string.Empty;

            }

            // Возвращаем результат выполнения операции с некоторыми изменениями
            return resultString.Trim(' ', '\r', '\n');

        }
        
        /// <summary>
        /// Метод осуществляет проверку на успешность прохождения пользовательской программой тестирования.
        /// </summary>
        /// <exception cref="ArgumentNullException">Что-то не так с данными результата тестирования</exception>
        /// <returns>Возвращает значение, которое сигнализирует об успешности прохождения всех тестов.</returns>
        public bool IsSuccessful()
        {

            // Обрабатываем данные и возвращаем результат
            return TestingResults.Aggregate(
                true,
                (current, test) => current && test.IsSuccessful
            );

        }

    }

}