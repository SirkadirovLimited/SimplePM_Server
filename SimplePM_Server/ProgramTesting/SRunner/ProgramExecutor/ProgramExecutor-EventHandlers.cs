using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CompilerPlugin;
using NLog;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        private void ProgramProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            /*
             * Если результат тестирования уже
             * имеется, не стоит ничего делать
             *
             * Если данные не получены, так же
             * не стоит ничего делать.
             */
            
            if (e.Data == null || _testingResultReceived)
                return;
            
            /*
             * Проверка на превышение лимита вывода
             */

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

            /*
             * В ином случае дозаписываем данные
             * в соответственную переменную.
             */

            var adaptedString = (_adaptOutput)
                ? e.Data.Trim()
                : e.Data;

            _programOutput += adaptedString + '\n';

        }

    }
    
}