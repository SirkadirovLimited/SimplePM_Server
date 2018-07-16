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
using System.IO;
using System.Diagnostics;

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
            var generationResult = StandardCompilationMethods.RunCompiler(
                (string)(languageConfiguration.dotnet_path),
                ((string)(languageConfiguration.generator_arguments))
                    .Replace("{%submission_id%}", submissionId)
                    .Replace("{%directory_full_path%}", directoryFullPath),
                directoryFullPath
            );

            // Удаляем стандартный файл исходного кода
            File.Delete(Path.Combine(directoryFullPath, "Program." + (string)(languageConfiguration.source_ext)));
            
            // Делаем так, чтобы сборщик "подцепил" наш файл исходного кода
            File.Move(fileLocation, Path.Combine(directoryFullPath, "Program." + (string)(languageConfiguration.source_ext)));
            
            // Выполняем сборку проекта средствами .NET Core's MSBuild
            var buildResult = StandardCompilationMethods.RunCompiler(
                (string)(languageConfiguration.dotnet_path),
                ((string)(languageConfiguration.compiler_arguments))
                    .Replace("{%project_path%}", submissionId + ".csproj")
                    .Replace("{%directory_full_path%}", directoryFullPath),
                directoryFullPath
            );

            // Формируем полные выходные данные приложения
            buildResult.CompilerOutput = generationResult.CompilerOutput + "\r\n" + buildResult.CompilerOutput;
            
            // Формируем полный путь к исполнительному файлу
            buildResult.ExeFullname = Path.Combine(directoryFullPath, submissionId + ".dll");
            
            // Возвращаем результат компиляции
            return StandardCompilationMethods.ReturnCompilerResult(buildResult);

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {
            
            try
            {
                
                // Устанавливаем имя запускаемой программы
                startInfo.FileName = (string)(languageConfiguration.runtime_path);
                
                // Аргументы запуска данной программы
                startInfo.Arguments = "-d64 -cp . \"" + Path.GetFileNameWithoutExtension(new FileInfo(filePath).Name) + '"';

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