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

        public SingleTestResult RunTesting()
        {

            logger.Trace("ProgramExecutor for <" + _programPath + ">: RunTesting() [started]");
            
            try
            {

                // Инициализация всего необходимого
                Init();

                // Запись входных данных в файл
                WriteInputFile();

                /*
                 * Продолжаем тестирование лишь в случае
                 * отсутствия предопределённого результата
                 * тестирования.
                 */

                if (!_testingResultReceived)
                {

                    // Запускаем пользовательский процесс
                    _programProcess.Start();

                    // Сигнализируем о готовности чтения выходного потока
                    _programProcess.BeginOutputReadLine();

                    // Записываем входные данные
                    WriteInputString();

                    // Запускаем слежение за процессорным временем
                    new Task(StartProcessorTimeLimitChecker).Start();

                    // Запускаем слежение за используемой памятью
                    new Task(StartMemoryLimitChecker).Start();

                    /*
                     * Ожидаем завершения пользовательского процесса.
                     * Если этого не произошло, предпринимаем
                     * необходимые действия по потношению к нему.
                     */

                    if (!_programProcess.WaitForExit(_programRuntimeLimit))
                    {

                        try
                        {

                            // Насильно "убиваем" пользовательский процесс
                            _programProcess.Kill();

                        }
                        catch { /* Нет необходимости обработки */ }
                        
                        // Указываем, что результат тестирования уже получен
                        _testingResultReceived = true;
                        
                        // Устанавливаем неудачный результат тестирования
                        _testingResult = SingleTestResult.PossibleResult.WaitErrorResult;
                        
                    }

                    // Формируем промежуточный результат тестирования
                    FormatTestResult();

                }
                
            }
            catch (Exception ex)
            {

                // Записываем информацию об ошибке в лог-файл
                logger.Trace("ProgramExecutor for <" + _programPath + ">: " + ex);

                /*
                 * Создаём псевдорезультаты тестирования пользовательской программы
                 */

                _testingResultReceived = true;
                _testingResult = SingleTestResult.PossibleResult.ErrorOutputNotNullResult;

                /*
                 * Записываем информацию об исключении в
                 * выходной поток ошибок пользовательской
                 * программы.
                 */

                _programErrorOutput = ex.ToString();

            }
            
            logger.Trace("ProgramExecutor for <" + _programPath + ">: RunTesting() [finished]");
            
            // Возвращаем промежуточный результат тестирования
            return GenerateTestResult();

        }

    }
    
}