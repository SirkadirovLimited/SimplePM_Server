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
using System.Threading.Tasks;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        private void StartMemoryLimitChecker()
        {

            //TODO: Создавать задачу при вызове, а не тут
            new Task(() => {

                try
                {

                    while (!_programProcess.HasExited)
                    {

                        // Удаляем весь кэш, связанный с компонентом
                        _programProcess.Refresh();

                        // Получаем текущее значение свойства
                        UsedMemory = _programProcess.PeakWorkingSet64;

                        /*
                         * Проверяем  на  превышение  лимита
                         * и в случае обнаружения, "убиваем"
                         * процесс.
                         */
                        
                        if (_programMemoryLimit > 0 && UsedMemory > _programMemoryLimit)
                        {

                            // Убиваем процесс
                            _programProcess.Kill();

                            /*
                             * Записываем   преждевременный   результат
                             * тестирования пользовательской программы.
                             */

                            _testingResultReceived = true;
                            _testingResult = SingleTestResult.PossibleResult.MemoryLimitResult;

                        }
                        
                    }

                }
                catch (Exception)
                {

                    /* Deal with it */

                }

            }).Start();

        }

        private void StartProcessorTimeLimitChecker()
        {

            //TODO: Создавать задачу при вызове, а не тут
            new Task(() => {

                try
                {

                    while (!_programProcess.HasExited)
                    {

                        // Удаляем весь кэш, связанный с компонентом
                        _programProcess.Refresh();

                        // Получаем текущее значение свойства
                        UsedProcessorTime = Convert.ToInt32(
                            Math.Round(
                                _programProcess.TotalProcessorTime.TotalMilliseconds
                            )
                        );

                        /*
                         * Проверяем  на  превышение  лимита
                         * и в случае обнаружения, "убиваем"
                         * процесс.
                         */

                        if (_programProcessorTimeLimit > 0 && UsedProcessorTime > _programProcessorTimeLimit)
                        {

                            // Убиваем процесс
                            _programProcess.Kill();

                            /*
                             * Записываем   преждевременный   результат
                             * тестирования пользовательской программы.
                             */

                            _testingResultReceived = true;
                            _testingResult = SingleTestResult.PossibleResult.TimeLimitResult;

                        }

                    }

                }
                catch (Exception)
                {

                    /* Deal with it */

                }

            }).Start();

        }

    }
    
}