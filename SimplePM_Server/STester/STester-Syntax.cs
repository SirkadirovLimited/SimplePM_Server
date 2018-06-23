/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server
 * A part of SimplePM programming contests management system.
 *
 * Copyright 2017 Yurij Kadirov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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

using System.Text;
using ProgramTesting;

namespace SimplePM_Server.STester
{
    
    internal partial class STester
    {

        /*
         * Метод отвечает за выполнение проверки
         * синтаксиса пользовательского решения.
         */

        public ProgramTestingResult Syntax()
        {

            /*
             * Генерируем результат тестирования
             * и возвращаем его  как объект типа
             * ProgramTestingResult.
             */

            return new ProgramTestingResult(1)
            {

                TestingResults =
                {

                    [0] = new TestResult
                    {

                        // Выходные данные заполняем кракозябрами
                        Output = Encoding.UTF8.GetBytes("NULL"),

                        // Выходные данные исключений устанавливаем в null
                        ErrorOutput = null,

                        // Код выхода - стандартный
                        ExitCode = 0,

                        // Результатом будет промежуточный успешный
                        Result = TestResult.MiddleSuccessResult,

                        // Использованная память
                        UsedMemory = 0,

                        // Использованное процессорное время
                        UsedProcessorTime = 0

                    }

                }

            };

        }

    }

}