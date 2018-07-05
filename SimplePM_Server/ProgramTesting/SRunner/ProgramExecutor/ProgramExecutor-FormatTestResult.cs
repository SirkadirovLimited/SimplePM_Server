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