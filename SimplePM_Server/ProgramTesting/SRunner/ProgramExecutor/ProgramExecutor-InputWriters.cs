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

using System;
using System.IO;
using System.Text;
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