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
using System.IO;
using SProgramRunner;

namespace CompilerPlugin
{
    
    // ReSharper disable once UnusedMember.Global
    public class Compiler : ICompilerPlugin
    {

        public PluginInfo PluginInformation => new PluginInfo(
            "DotNetCore",
            "Yurij Kadirov (Sirkadirov)",
            "https://spm.sirkadirov.com/"
        );

        public CompilationResult RunCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation)
        {

            // Получаем путь к директории с исходным кодом
            var directoryFullPath = new FileInfo(fileLocation).DirectoryName ?? throw new FileNotFoundException();
            
            // Создаём новый .NET Core проект
            StandardCompilationMethods.RunCompiler(
                (string)(languageConfiguration.dotnet_path),
                ((string)(languageConfiguration.generator_arguments))
                    .Replace("{%submission_id%}", submissionId),
                directoryFullPath
            );

            // Удаляем стандартный файл исходного кода
            File.Delete(Path.Combine(directoryFullPath, "Program." + (string)(languageConfiguration.source_ext)));
            
            // Делаем так, чтобы сборщик "подцепил" наш файл исходного кода
            File.Move(fileLocation, Path.Combine(directoryFullPath, "Program." + (string)(languageConfiguration.source_ext)));
            
            // Выполняем сборку проекта средствами .NET Core's MSBuild
            var buildResult = StandardCompilationMethods.RunCompiler(
                (string)(languageConfiguration.dotnet_path),
                ((string)(languageConfiguration.compiler_arguments)),
                directoryFullPath
            );
            
            // Формируем полный путь к исполнительному файлу
            buildResult.ExeFullname = Path.Combine(directoryFullPath, submissionId + ".dll");
            
            // Возвращаем результат компиляции
            return StandardCompilationMethods.ReturnCompilerResult(buildResult);

        }
        
        public TestingRequestStuct.ProcessRuntimeInfo SetRunningMethod(ref dynamic languageConfiguration, string filePath, string arguments)
        {

            return new TestingRequestStuct.ProcessRuntimeInfo
            {
                
                FileName = (string)(languageConfiguration.dotnet_path),
                Arguments = filePath + (!string.IsNullOrWhiteSpace(arguments) ? " " + arguments : ""),
                WorkingDirectory = new FileInfo(filePath).DirectoryName
                
            };

        }
        
    }

}