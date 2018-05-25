/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server
 * A part of SimplePM programming contests management system.
 *
 * Copyright 2017 Yurij Kadirov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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
using CompilerBase;
using System.Diagnostics;

namespace CompilerPlugin
{
    
    public class Compiler : ICompilerPlugin
    {

        public string PluginName => "Java";
        public string AuthorName => "Kadirov Yurij";
        public string SupportUri => "https://spm.sirkadirov.com/";

        public CompilerResult StartCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation)
        {

            // Инициализируем объект CompilerRefs
            var cRefs = new CompilerRefs();

            // Запуск компилятора с заранее определёнными аргументами
            var result = cRefs.RunCompiler(
                (string)languageConfiguration.compiler_path,
                ((string)languageConfiguration.compiler_arguments).Replace("{%fileLocation%}", fileLocation)
            );

            /*
             * Выполняем  все  необходимые действия
             * в  блоке  обработки  исключений  для
             * исключения возможности возникновения
             * непредвиденных исключений.
             */

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
                    (string)languageConfiguration.default_class_name + ".class"
                );

                // Проверяем на существование главного класса
                if (!File.Exists(result.ExeFullname))
                    throw new FileNotFoundException();

                // Ошибок не найдено!
                result.HasErrors = false;

            }
            catch (Exception)
            {

                /*
                 * В случае любой ошибки считаем что она
                 * произошла по прямой вине пользователя
                 */

                result.HasErrors = true;

            }

            // Возвращаем результат компиляции
            return cRefs.ReturnCompilerResult(result);

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {
            
            /*
             * Выполняем  все  необходимые действия
             * в  блоке  обработки  исключений  для
             * исключения возможности возникновения
             * непредвиденных исключений.
             */
            
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
                startInfo.FileName = (string)languageConfiguration.runtime_path;
                
                // Аргументы запуска данной программы
                startInfo.Arguments = "-d64 -cp . " +
                                      '"' +
                                      Path.GetFileNameWithoutExtension(fileInfo.Name) +
                                      '"';

            }
            catch
            {

                /*
                 * В случае возникновения каких-либо
                 * ошибок  сигнализируем  об этом  с
                 * помощью return false.
                 */

                return false;

            }
            
            /*
             * Возвращаем родителю информацию о
             * том, что  запашиваемая  операция
             * была  выполнена  самым  успешным
             * образом.
             */

            return true;

        }
        
    }

}
