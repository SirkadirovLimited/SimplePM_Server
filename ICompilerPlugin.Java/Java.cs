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
using Plugin;
using System.IO;
using System.Diagnostics;

namespace CompilerPlugin
{
    
    // ReSharper disable once UnusedMember.Global
    public class Compiler : ICompilerPlugin
    {

        public PluginInfo PluginInformation => new PluginInfo(
            "Java",
            "Yurij Kadirov (Sirkadirov)",
            "https://spm.sirkadirov.com/"
        );

        public CompilationResult RunCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation)
        {

            // Запуск компилятора с заранее определёнными аргументами
            var result = StandardCompilationMethods.RunCompiler(
                (string)(languageConfiguration.compiler_path),
                (string)(languageConfiguration.compiler_arguments).Replace("{%fileLocation%}", fileLocation)
            );

            try
            {

                // Получаем информацию о файле исходного кода
                var fileInfo = new FileInfo(fileLocation);

                // Указываем полный путь к главному исполняемому файлу
                result.ExeFullname = Path.Combine(
                    fileInfo.DirectoryName ?? throw new DirectoryNotFoundException(
                        "inner",
                        new FileNotFoundException(
                            "ICompilerPlugin.Java",
                            fileLocation
                        )
                    ),
                    (string)(languageConfiguration.default_class_name) + ".class"
                );

                // Проверяем на существование главного класса
                if (!File.Exists(result.ExeFullname))
                    throw new FileNotFoundException();

                // Ошибок не найдено!
                result.HasErrors = false;

            }
            catch (Exception)
            {

                // Будем считать, что исключение выброшено по вине пользователя
                result.HasErrors = true;

            }

            // Возвращаем результат компиляции
            return StandardCompilationMethods.ReturnCompilerResult(result);

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {
            
            try
            {
                
                // Получаем информацию о файле
                var fileInfo = new FileInfo(filePath);

                // Устанавливаем рабочую папку процесса
                startInfo.WorkingDirectory = fileInfo.DirectoryName
                                             ?? throw new DirectoryNotFoundException(
                                                 "inner",
                                                 new FileNotFoundException(
                                                     "ICompilerPlugin.Java",
                                                     filePath
                                                 )
                                             );

                // Устанавливаем имя запускаемой программы
                startInfo.FileName = (string)(languageConfiguration.runtime_path);
                
                // Аргументы запуска данной программы
                startInfo.Arguments = "-d64 -cp . " +
                                      '"' +
                                      Path.GetFileNameWithoutExtension(fileInfo.Name) +
                                      '"';

            }
            catch
            {

                // Сигнализируем о наличии ошибок в процессе операции
                return false;

            }
            
            // Сигнализируем об успешном выполнении операции
            return true;

        }
        
    }

}
