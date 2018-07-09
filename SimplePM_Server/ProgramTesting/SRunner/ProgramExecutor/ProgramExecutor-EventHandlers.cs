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