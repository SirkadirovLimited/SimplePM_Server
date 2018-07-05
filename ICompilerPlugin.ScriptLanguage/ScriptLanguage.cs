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

using Plugin;
using System.Diagnostics;

namespace CompilerPlugin
{
    
    // ReSharper disable once UnusedMember.Global
    public class Compiler : ICompilerPlugin
    {

        public PluginInfo PluginInformation => new PluginInfo(
            "ScriptLanguage",
            "Yurij Kadirov (Sirkadirov)",
            "https://spm.sirkadirov.com/"
        );

        public CompilationResult RunCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation)
        {

            // Возвращаем пустой результат компиляции
            return new CompilationResult
            {

                HasErrors = false,
                ExeFullname = fileLocation,
                CompilerOutput = ""

            };

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {

            try
            {

                // Устанавливаем имя запускаемой программы
                startInfo.FileName = (string)languageConfiguration.runtime_path;
                
                // Аргументы запуска данной программы
                startInfo.Arguments = ((string)languageConfiguration.runtime_arguments).Replace("{%fileLocation%}", filePath);
                
            }
            catch
            {

                // Сигнализируем о наличии ошибок при выполнении операции
                return false;

            }

            // Сигнализируем об успешности выполнения операции
            return true;

        }
        
    }

}
