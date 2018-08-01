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