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
using Plugin;
using System.IO;
using System.Diagnostics;
using SProgramRunner;

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
        
        public TestingRequestStuct.ProcessRuntimeInfo SetRunningMethod(ref dynamic languageConfiguration, string filePath, string arguments)
        {
            
            return new TestingRequestStuct.ProcessRuntimeInfo
            {
                
                FileName = (string)(languageConfiguration.runtime_path),
                Arguments = "-d64 -cp . \"" + Path.GetFileNameWithoutExtension(new FileInfo(filePath).Name) + '"' + (!string.IsNullOrWhiteSpace(arguments) ? " " + arguments : ""),
                WorkingDirectory = new FileInfo(filePath).DirectoryName
                
            };

        }
        
    }

}
