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

        private SingleTestResult GenerateTestResult()
        {
            
            /*
             * Генерируем результат тестирования
             * пользовательской   программы   на
             * текущем тесте.
             */

            var result = new SingleTestResult
            {

                // Выходные данные из стандартного потока
                ErrorOutput = _programErrorOutput,
                Output = Encoding.UTF8.GetBytes(
                    (_adaptOutput)
                        ? _programOutput.TrimEnd('\r', '\n')
                        : _programOutput
                ),

                // Результаты предварительного тестирования
                ExitCode = _programProcess.ExitCode,
                Result = _testingResult,

                // Информация об использовании ресурсов
                UsedMemory = UsedMemory,
                UsedProcessorTime = UsedProcessorTime

            };
            
            /*
             * Освобождаем все связанные
             * с процессом ресурсы.
             */

            _programProcess.Close();
            _programProcess.Dispose();

            /*
             * Возвращаем сгенерированный выше результат
             */

            return result;

        }

    }
    
}