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

                    // Возвращаем результат выполнения операции с некоторыми изменениями
                    return (current + aggTmpString + splitter.ToString()).Trim(' ', '\r', '\n', splitter);

                });

            }
            catch
            {

                // В исключительной ситуации возвращаем пустую строку
                resultString = string.Empty;

            }

            // Возвращаем результат выполнения операции
            return resultString;

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