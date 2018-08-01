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

using Plugin;
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
