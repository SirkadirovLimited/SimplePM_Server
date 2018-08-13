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

using System;
using System.IO;
using System.Text;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        /*
         * Метод занимается тем, что записывает
         * входные данные программы в её входной
         * (как ни странно) поток.
         */
        
        private void WriteInputString()
        {

            logger.Trace("ProgramExecutor for <" + _programPath + ">: WriteInputString() [started]");
            
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
            catch (Exception ex)
            {

                logger.Error("ProgramExecutor for <" + _programPath + ">: " + ex);
                
                try
                {

                    // Убиваем процесс
                    _programProcess.Kill();

                }
                catch (Exception) { /* Выполнение дополнительных действий не предусмотрено */ }
                finally
                {

                    // Указываем, что результат тестирования получен
                    _testingResultReceived = true;

                    // Указываем результат тестирования
                    _testingResult = SingleTestResult.PossibleResult.InputErrorResult;

                }

            }
            
            logger.Trace("ProgramExecutor for <" + _programPath + ">: WriteInputString() [finished]");

        }

        /*
         * Метод занимается тем, что записывает
         * входные данные программы в файл, из
         * которого эта программа (возможно) их
         * будет читать.
         */
        
        private void WriteInputFile()
        {

            logger.Trace("ProgramExecutor for <" + _programPath + ">: WriteInputFile() [started]");
            
            try
            {
                
                // Получаем полный путь к файлу с входными данными
                var inputFilePath = Path.Combine(
                    new FileInfo(_programPath).DirectoryName ?? throw new DirectoryNotFoundException(),
                    "input"
                );

                lock (new object())
                {

                    // Создаём файл
                    File.Create(inputFilePath, 4096, FileOptions.None).Close();
                    
                    // Указываем аттрибуты этого файла
                    File.SetAttributes(
                        inputFilePath,
                        FileAttributes.Normal
                    );
                    
                    // Записываем данные в файл input
                    File.WriteAllBytes(
                        inputFilePath,
                        _programInputBytes
                    );

                }
                
            }
            catch (Exception ex)
            {

                // Записываем информацию об ошибке в лог-файл
                logger.Error("ProgramExecutor for <" + _programPath + ">: " + ex);
                
                // Указываем, что результат тестирования получен
                _testingResultReceived = true;

                // Указываем результат тестирования
                _testingResult = SingleTestResult.PossibleResult.InputErrorResult;

            }
            
            logger.Trace("ProgramExecutor for <" + _programPath + ">: WriteInputFile() [finished]");

        }
        
        private byte[] ReadOutputData()
        {
            
            // Получаем полный путь к файлу с выходными данными
            var outputFilePath = Path.Combine(
                new FileInfo(_programPath).DirectoryName ?? throw new DirectoryNotFoundException(),
                "output"
            );

            try
            {

                //Ели файл не существует, используем стандартный выходной поток
                if (!File.Exists(outputFilePath))
                    throw new Exception(); // Выбрасываем исключение

                // Осуществляем чтение данных из файла и возвращаем их
                return File.ReadAllBytes(outputFilePath);

            }
            catch
            {
                
                logger.Fatal("#" + _programPath + ((_adaptOutput)
                    ? _programOutput.TrimEnd('\r', '\n')
                    : _programOutput));
                
                // В случае ошибки или если файл не найден, используем STDOUT
                return Encoding.UTF8.GetBytes(
                    (_adaptOutput)
                        ? _programOutput.TrimEnd('\r', '\n')
                        : _programOutput
                );

            }
            
        }

    }
    
}