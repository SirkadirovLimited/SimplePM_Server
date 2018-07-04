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
            "TypicalCompiler",
            "Yurij Kadirov (Sirkadirov)",
            "https://spm.sirkadirov.com/"
        );

        public CompilationResult RunCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation)
        {

            // Будущее местонахождение исполняемого файла
            var exeLocation = StandardCompilationMethods.GenerateExeFileLocation(
                fileLocation,
                submissionId
            );

            // Запуск компилятора с заранее определёнными аргументами
            var result = StandardCompilationMethods.RunCompiler(
                (string)languageConfiguration.compiler_path,
                ((string)languageConfiguration.compiler_arguments)
                .Replace("{%fileLocation%}", fileLocation)
                .Replace("{%exeLocation%}", exeLocation)
            );

            // Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

            // Возвращаем результат компиляции
            return StandardCompilationMethods.ReturnCompilerResult(result);

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {

            /*
             * Устанавливаем имя исполняемого файла
             * и аргументы запуска программы.
             */
            
            startInfo.FileName = filePath;
            startInfo.Arguments = "";

            // Сигнализируем об успешности выполнения операции
            return true;

        }
        
    }

}
