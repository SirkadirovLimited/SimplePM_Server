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

        private void FormatTestResult()
        {

            /*
             * Проверка на использованную память
             */

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

            checker = !_testingResultReceived &&
                _programProcess.ExitCode != 0 &&
                _programProcess.ExitCode != -1;

            if (checker)
            {

                _testingResultReceived = true;
                _testingResult = SingleTestResult.PossibleResult.RuntimeErrorResult;

            }

            /*
             * Проверка на наличие текста в выходном потоке ошибок
             */

            if (!_testingResultReceived)
            {

                // Читаем выходной поток ошибок
                _programErrorOutput = _programProcess.StandardError.ReadToEnd();

                // Проверка на наличие ошибок
                if (_programErrorOutput.Length > 0)
                {

                    _testingResultReceived = true;
                    _testingResult = SingleTestResult.PossibleResult.ErrorOutputNotNullResult;

                }

            }

            /*
             * Если всё хорошо, возвращаем временный результат
             */

            if (!_testingResultReceived)
            {

                _testingResultReceived = true;
                _testingResult = SingleTestResult.PossibleResult.MiddleSuccessResult;

            }

        }

    }
    
}