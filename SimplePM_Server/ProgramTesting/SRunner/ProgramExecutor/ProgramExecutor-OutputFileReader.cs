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

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

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