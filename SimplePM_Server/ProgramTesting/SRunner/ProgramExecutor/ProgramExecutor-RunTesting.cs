using System;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        public SingleTestResult RunTesting()
        {

            /*
             * Защищаемся от всех возможных угроз
             */
            
            try
            {

                // Инициализация всего необходимого
                Init();

                // Запись входных данных в файл
                WriteInputFile();

                /*
                 * Продолжаем тестирование лишь  в  случае
                 * отсутствия предопределённого результата
                 * тестирования.
                 */

                if (!_testingResultReceived)
                {

                    // Запускаем пользовательский процесс
                    _programProcess.Start();

                    // Сигнализируем о готовности чтения выходного потока
                    _programProcess.BeginOutputReadLine();

                    /*
                     * Записываем  входные
                     * данные  во  входной
                     * поток.
                     */

                    WriteInputString();

                    /*
                     * Вызываем метод, запускающий
                     * слежение  за   процессорным
                     * временем.
                     */
                    
                    StartProcessorTimeLimitChecker();

                    /*
                     * Вызываем метод, запускающий
                     * слежение за памятью.
                     */

                    StartMemoryLimitChecker();

                    /*
                     * Ожидаем  завершения  пользовательского
                     * процесса.
                     * Если этого не произошло, предпринимаем
                     * необходимые действия  по  потношению к
                     * нему.
                     */

                    if (!_programProcess.WaitForExit(_programRuntimeLimit))
                    {

                        /*
                         * Обрабатываем возможные исключения
                         */
                        
                        try
                        {

                            // Насильно "убиваем" пользовательский процесс
                            _programProcess.Kill();

                        }
                        catch
                        {
                            
                            /* Нет необходимости обработки */
                            
                        }
                        
                        // Указываем, что результат тестирования уже получен
                        _testingResultReceived = true;
                        
                        // Устанавливаем неудачный результат тестирования
                        _testingResult = SingleTestResult.PossibleResult.WaitErrorResult;
                        
                    }

                    /*
                     * Формируем     промежуточный
                     * результат      тестирования
                     * пользовательской программы.
                     */

                    FormatTestResult();

                }
                
            }
            catch (Exception ex)
            {

                /*
                 * Записываем информацию об ошибке в лог-файл
                 */

                logger.Error("An exception catched while trying to test user's submission: " + ex);

                /*
                 * Создаём псевдорезультаты
                 * тестирования   пользова-
                 * тельской программы.
                 */

                _testingResultReceived = true;
                _testingResult = SingleTestResult.PossibleResult.ErrorOutputNotNullResult;

                /*
                 * Записываем    информацию  об  исключении
                 * в выходной поток ошибок пользовательской
                 * программы.
                 */

                _programErrorOutput = ex.ToString();

            }
            
            /*
             * Возвращаем промежуточный
             * результат тестирования.
             */

            return GenerateTestResult();

        }

    }
    
}