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

using System.Diagnostics;
using CompilerBase;

namespace CompilerPlugin
{
    
    public class Compiler : ICompilerPlugin
    {

        public string PluginName => "NoCompiler";
        public string AuthorName => "Kadirov Yurij";
        public string SupportUri => "https://spm.sirkadirov.com/";

        public CompilerResult StartCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation)
        {

            // Делаем преждевременные выводы
            // прям как некоторые девушки
            // (по крайней мере на данный момент)

            var result = new CompilerResult
            {

                // ошибок нет - но вы держитесь
                HasErrors = false,

                // что дали - то и скинул
                ExeFullname = fileLocation,

                // хз зачем, но надо
                CompilerMessage = Properties.Resources.noCompilerRequired

            };

            // Возвращаем результат фальш-компиляции
            return result;

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

                // Устанавливаем имя запускаемой программы
                startInfo.FileName = (string)languageConfiguration.runtime_path;
                
                // Аргументы запуска данной программы
                startInfo.Arguments = ((string)languageConfiguration.runtime_arguments).Replace("{%fileLocation%}", filePath);
                
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
