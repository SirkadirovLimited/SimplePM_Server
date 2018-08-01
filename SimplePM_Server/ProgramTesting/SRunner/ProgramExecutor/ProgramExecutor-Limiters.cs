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
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        private void StartMemoryLimitChecker()
        {

            logger.Trace("ProgramExecutor for <" + _programPath + ">: StartMemoryLimitChecker() [started]");
            
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
            catch { /* Дополнительных действий не предусмотрено */ }

            logger.Trace("ProgramExecutor for <" + _programPath + ">: StartMemoryLimitChecker() [finished]");
            
        }

        private void StartProcessorTimeLimitChecker()
        {

            logger.Trace("ProgramExecutor for <" + _programPath + ">: StartProcessorTimeLimitChecker() [started]");
            
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
            catch { /* Дополнительных действий не предусмотрено */ }

            logger.Trace("ProgramExecutor for <" + _programPath + ">: StartProcessorTimeLimitChecker() [finished]");
            
        }

    }
    
}