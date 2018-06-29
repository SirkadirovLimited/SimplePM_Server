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

        private void WriteInputString()
        {

            try
            {

                // Записываем входные данные во входной поток
                _programProcess.StandardInput.Write(
                    Encoding.UTF8.GetString(
                        _programInputBytes
                    )
                );

                // Очищаем буферы
                _programProcess.StandardInput.Flush();

                // Закрываем входной поток
                _programProcess.StandardInput.Close();

            }
            catch (Exception)
            {

                /*
                 * Если что-то пошло не так,
                 * как планировалось ранее.
                 */

                try
                {

                    // Убиваем процесс
                    _programProcess.Kill();

                }
                catch (Exception)
                {

                    /* Deal with it */

                }
                finally
                {

                    // Указываем, что результат тестирования получен
                    _testingResultReceived = true;

                    // Указываем результат тестирования
                    _testingResult = SingleTestResult.PossibleResult.InputErrorResult;

                }

            }

        }

        private void WriteInputFile()
        {

            try
            {
                
                /*
                 * Получаем полный путь к файлу с входными данными
                 */

                var inputFilePath = Path.Combine(
                    new FileInfo(_programPath).DirectoryName ?? throw new DirectoryNotFoundException(),
                    "input.txt"
                );

                /*
                 * Выполняем действия над файлом в синхронизируемом
                 * блоке команд для обеспечения  безопасности и для
                 * снжения нагрузки на накопитель.
                 */

                lock (new object())
                {

                    // Записываем данные в файл input.txt
                    File.WriteAllBytes(
                        inputFilePath,
                        _programInputBytes
                    );

                    // Указываем аттрибуты этого файла
                    File.SetAttributes(
                        inputFilePath,
                        FileAttributes.Temporary | FileAttributes.NotContentIndexed
                    );

                }
                
            }
            catch (Exception)
            {

                // Указываем, что результат тестирования получен
                _testingResultReceived = true;

                // Указываем результат тестирования
                _testingResult = SingleTestResult.PossibleResult.InputErrorResult;

            }

        }

    }
    
}