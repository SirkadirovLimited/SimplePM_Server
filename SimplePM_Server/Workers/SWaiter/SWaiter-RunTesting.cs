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
using ProgramTestingAdditions;
using SimplePM_Server.ProgramTesting.STester;

namespace SimplePM_Server.Workers
{
    
    public partial class SWaiter
    {
        
        private ProgramTestingResult RunTesting(
            CompilationResult cResult
        )
        {

            /*
             * Записываем   в   лог-файл  информацию  о  том,
             * что в данный  момент  производим  тестирование
             * пользовательского решения поставленной задачи.
             */

            logger.Trace(
                "#" + _submissionInfo.SubmissionId +
                ": Running solution testing subsystem (" +
                _submissionInfo.TestType +
                " mode)..."
            );

            /*
             * Если компиляция пользовательского решения
             * поставленной задачи произошла с ошибками,
             * изменяем тип тестирования на syntax.
             */

            if (cResult.HasErrors)
                _submissionInfo.TestType = "syntax";

            /*
             * Объявляем временную переменную,
             * которая будет хранить результат
             * выполнения тестирования пользо-
             * вательской программы.
             */

            ProgramTestingResult tmpResult = null;

            /*
             * Выполняем тестирование в блоке обработки
             * исключительных систуаций,  и,  в  случае
             * возникновения такой  ситуации записываем
             * данные о ней в лог-файл  и  искусственно
             * создаём     результаты      тестирования
             * пользовательского решения задачи.
             */

            try
            {

                /*
                 * В зависимости от выбранного пользователем
                 * типа тестирования выполняем специфические
                 * операции по отношению к решению задачи.
                 */

                switch (_submissionInfo.TestType)
                {

                    /* Проверка синтаксиса */
                    case "syntax":

                        tmpResult = new SyntaxTesting(
                            ref _connection,
                            cResult.ExeFullname,
                            ref _submissionInfo
                        ).RunTesting();

                        break;

                    /* Debug-тестирование */
                    case "debug":

                        tmpResult = new DebugTesting(
                            ref _connection,
                            cResult.ExeFullname,
                            ref _submissionInfo
                        ).RunTesting();

                        break;

                    /* Release-тестирование */
                    case "release":

                        tmpResult = new ReleaseTesting(
                            ref _connection,
                            cResult.ExeFullname,
                            ref _submissionInfo
                        ).RunTesting();

                        break;

                }

            }
            catch (Exception ex)
            {

                // Записываем информацию об исключении в лог
                logger.Error(ex);

                /*
                 * Создаём псевдорезультат тестирования,
                 * который будет содержать информацию о
                 * возникшем во время тестирования поль-
                 * зовательского решения задачи исключе-
                 * нии.
                 */

                tmpResult = new ProgramTestingResult(1)
                {

                    /*
                     * Создаём превдотест и добавляем его
                     * в массив результатов тестирования.
                     */
                    TestingResults =
                    {

                        [0] = new SingleTestResult
                        {
                            
                            // За код выхода принимаем номер исклбчения
                            ExitCode = ex.HResult,

                            // За выходной поток ошибок принимаем исключение
                            ErrorOutput = ex.ToString(),

                            // Заполняем выходные данные информацией (not null)
                            Output = Encoding.UTF8.GetBytes("An exception occured during testing!"),

                            // Устанавливаем результат тестирования
                            Result = SingleTestResult.PossibleResult.ErrorOutputNotNullResult,

                            /*
                             * Указываем, что память и процессорное
                             * время не были использованы.
                             */

                            UsedMemory = 0,
                            UsedProcessorTime = 0

                        }

                    }

                };

            }

            /*
             * Возвращаем результат выполнения
             * тестирования  пользовательского
             * решения данной задачи.
             */

            return tmpResult;

        }
        
    }
    
}