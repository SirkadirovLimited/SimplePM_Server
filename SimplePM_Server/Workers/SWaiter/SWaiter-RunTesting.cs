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
using System.Text;
using CompilerPlugin;
using ProgramTestingAdditions;
using SimplePM_Server.ProgramTesting.STester;

namespace SimplePM_Server.Workers
{
    
    public partial class SWaiter
    {
        
        private SolutionTestingResult RunTesting(
            CompilationResult cResult
        )
        {

            // Добавляем в лог-файл сведения о действиях
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

            // Временная перемення для хранения результатов тестирования
            SolutionTestingResult tmpResult = null;

            try
            {

                // В зависимости от типа тестирования выполняем специфические операции
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

                // Записываем информацию об исключении в лог-файл
                logger.Error(ex);

                /*
                 * Создаём псевдорезультат тестирования,
                 * который будет содержать информацию о
                 * возникшем исключении.
                 */

                tmpResult = new SolutionTestingResult(1)
                {

                    // Создаём псевдотест
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

            // Возвращаем результаты тестирования
            return tmpResult;

        }
        
    }
    
}