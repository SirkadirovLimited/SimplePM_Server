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

        private SingleTestResult GenerateTestResult()
        {
            
            logger.Trace("ProgramExecutor for <" + _programPath + ">: GenerateTestResult() [started]");
            
            /*
             * Проверяем на наличие файла output.
             *
             * В случае если он существует, производим
             * чтение данных из него и представляем их
             * в виде выходных данных приложения.
             */
            
            // Генерируем результат тестирования программы
            var result = new SingleTestResult
            {

                // Выходные данные из стандартного потока
                ErrorOutput = _programErrorOutput,
                Output = ReadOutputData(),

                // Результаты предварительного тестирования
                ExitCode = _programProcess.ExitCode,
                Result = _testingResult,

                // Информация об использовании ресурсов
                UsedMemory = UsedMemory,
                UsedProcessorTime = UsedProcessorTime

            };
            
            /*
             * Освобождаем все связанные с процессом ресурсы.
             */

            _programProcess.Close();
            _programProcess.Dispose();

            logger.Trace("ProgramExecutor for <" + _programPath + ">: GenerateTestResult() [finished]");
            
            // Возвращаем сгенерированный результат
            return result;

        }

    }
    
}