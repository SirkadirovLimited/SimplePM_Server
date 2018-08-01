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

using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        /*
         * Метод обрабатывает имеющиеся данные
         * и предоставляет данные о результатах
         * тестирования.
         */
        
        private void FormatTestResult()
        {

            logger.Trace("ProgramExecutor for <" + _programPath + ">: FormatTestResult() [started]");
            
            /*
             * Проверка на использованную память
             */

            logger.Trace("ProgramExecutor for <" + _programPath + ">: memory check");
            
            var checker = !_testingResultReceived && _programMemoryLimit > 0 &&
                           UsedMemory > _programMemoryLimit;

            if (checker)
            {

                _testingResultReceived = true;
                _testingResult = SingleTestResult.PossibleResult.MemoryLimitResult;

            }

            /*
             * Проверка достижения лимита по процессорному времени
             */

            logger.Trace("ProgramExecutor for <" + _programPath + ">: processor time limit check");
            
            checker = !_testingResultReceived && _programProcessorTimeLimit > 0 &&
                      UsedProcessorTime > _programProcessorTimeLimit;

            if (checker)
            {

                _testingResultReceived = true;
                _testingResult = SingleTestResult.PossibleResult.TimeLimitResult;

            }

            /*
             * Проверка на обнаружение Runtime-ошибок
             */
            
            logger.Trace("ProgramExecutor for <" + _programPath + ">: runtime errors check");
            
            checker = !_testingResultReceived &&
                _programProcess.ExitCode != 0 &&
                _programProcess.ExitCode != -1;

            if (checker)
            {

                _testingResultReceived = true;
                _testingResult = SingleTestResult.PossibleResult.RuntimeErrorResult;

            }

            // Проверка на наличие текста в выходном потоке ошибок
            if (!_testingResultReceived)
            {

                logger.Trace("ProgramExecutor for <" + _programPath + ">: stderr check");
                
                // Читаем выходной поток ошибок
                _programErrorOutput = _programProcess.StandardError.ReadToEnd();

                // Проверка на наличие ошибок
                if (_programErrorOutput.Length > 0)
                {

                    _testingResultReceived = true;
                    _testingResult = SingleTestResult.PossibleResult.ErrorOutputNotNullResult;

                }

            }

            // Если всё хорошо, возвращаем временный результат
            if (!_testingResultReceived)
            {

                _testingResultReceived = true;
                _testingResult = SingleTestResult.PossibleResult.MiddleSuccessResult;

            }
            
            logger.Trace("ProgramExecutor for <" + _programPath + ">: FormatTestResult() [finished]");
           
        }

    }
    
}