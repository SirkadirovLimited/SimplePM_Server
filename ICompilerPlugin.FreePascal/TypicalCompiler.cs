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

using CompilerBase;
using System.Diagnostics;

namespace CompilerPlugin
{
    
    public class Compiler : ICompilerPlugin
    {

        public string PluginName => "TypicalCompiler";
        public string AuthorName => "Kadirov Yurij";
        public string SupportUri => "https://spm.sirkadirov.com/";

        public CompilerResult StartCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation)
        {

            // Инициализируем объект CompilerRefs
            var cRefs = new CompilerRefs();

            // Будущее местонахождение исполняемого файла
            var exeLocation = cRefs.GenerateExeFileLocation(
                fileLocation,
                submissionId
            );

            // Запуск компилятора с заранее определёнными аргументами
            var result = cRefs.RunCompiler(
                (string)languageConfiguration.compiler_path,
                ((string)languageConfiguration.compiler_arguments)
                    .Replace("{%fileLocation%}", fileLocation)
                        .Replace("{%exeLocation%}", exeLocation)
            );

            // Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

            // Возвращаем результат компиляции
            return cRefs.ReturnCompilerResult(result);

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {

            // Устанавливаем имя запускаемой программы
            startInfo.FileName = filePath;

            // Устанавливаем аргументы запуска данной программы
            startInfo.Arguments = "";

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
