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

using System.Diagnostics;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        /*
         * Обработка поступающих выходных данных приложения
         */
        
        private void ProgramProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            // Различные проверки безопасности
            if (e.Data == null || _testingResultReceived)
                return;
            
            // Проверка на превышение лимитов вывода данных
            if (_outputCharsLimit > 0 && _programOutput.Length + e.Data.Length > _outputCharsLimit)
            {

                // Указываем, что результаты проверки уже есть
                _testingResultReceived = true;

                // Указываем результат тестирования
                _testingResult = SingleTestResult.PossibleResult.OutputErrorResult;

                // Добавляем сообщение пояснения
                _programOutput = "=== OUTPUT CHARS LIMIT REACHED ===";

                // Завершаем выполнение метода
                return;

            }

            // Производим дозапись выходных данных
            var adaptedString = (_adaptOutput)
                ? e.Data.TrimEnd()
                : e.Data;

            // Записываем символ окончания строки
            _programOutput += adaptedString + '\n';

        }

    }
    
}