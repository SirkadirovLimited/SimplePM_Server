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
using PlatformChecker;
using System.Diagnostics;

namespace CompilerPlugin
{
    
    public class Compiler : ICompilerPlugin
    {

        public string PluginName => "Mono";
        public string AuthorName => "Yurij Kadirov";
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
                ((string)languageConfiguration.compiler_arguments).Replace("{%fileLocation%}", fileLocation)
            );

            // Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

            // Фикс для GNU/Linux-based систем
            if (!Platform.IsWindows)
                result.ExeFullname += ".exe";

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
                
                /*
                 * В зависимости от установленной на машине
                 * операционной системы, выполняем специфи-
                 * ческие действия для указания верного ме-
                 * тода  запуска  пользовательского решения
                 * поставленной задачи.
                 */

                if (Platform.IsLovelyLinux || Platform.IsUglyMac)
                {

                    // Указываем имя запускаемой программы (полный путь к ней)
                    startInfo.FileName = languageConfiguration.runtime_path;

                    // Указываем аргументы запуска
                    startInfo.Arguments = '"' + filePath + '"';

                }
                else
                {

                    // Указываем имя запускаемой программы (полный путь к ней)
                    startInfo.FileName = filePath;

                    // Указываем аргументы запуска
                    startInfo.Arguments = "";

                }

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
