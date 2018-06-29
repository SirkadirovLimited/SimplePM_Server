using System;
using System.Threading.Tasks;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        private void StartMemoryLimitChecker()
        {

            new Task(() => {

                try
                {

                    while (!_programProcess.HasExited)
                    {

                        // Удаляем весь кэш, связанный с компонентом
                        _programProcess.Refresh();

                        // Получаем текущее значение свойства
                        UsedMemory = _programProcess.PeakWorkingSet64;

                        /*
                         * Проверяем  на  превышение  лимита
                         * и в случае обнаружения, "убиваем"
                         * процесс.
                         */
                        
                        if (_programMemoryLimit > 0 && UsedMemory > _programMemoryLimit)
                        {

                            // Убиваем процесс
                            _programProcess.Kill();

                            /*
                             * Записываем   преждевременный   результат
                             * тестирования пользовательской программы.
                             */

                            _testingResultReceived = true;
                            _testingResult = SingleTestResult.PossibleResult.MemoryLimitResult;

                        }
                        
                    }

                }
                catch (Exception)
                {

                    /* Deal with it */

                }

            }).Start();

        }

        private void StartProcessorTimeLimitChecker()
        {

            new Task(() => {

                try
                {

                    while (!_programProcess.HasExited)
                    {

                        // Удаляем весь кэш, связанный с компонентом
                        _programProcess.Refresh();

                        // Получаем текущее значение свойства
                        UsedProcessorTime = Convert.ToInt32(
                            Math.Round(
                                _programProcess.TotalProcessorTime.TotalMilliseconds
                            )
                        );

                        /*
                         * Проверяем  на  превышение  лимита
                         * и в случае обнаружения, "убиваем"
                         * процесс.
                         */

                        if (_programProcessorTimeLimit > 0 && UsedProcessorTime > _programProcessorTimeLimit)
                        {

                            // Убиваем процесс
                            _programProcess.Kill();

                            /*
                             * Записываем   преждевременный   результат
                             * тестирования пользовательской программы.
                             */

                            _testingResultReceived = true;
                            _testingResult = SingleTestResult.PossibleResult.TimeLimitResult;

                        }

                    }

                }
                catch (Exception)
                {

                    /* Deal with it */

                }

            }).Start();

        }

    }
    
}