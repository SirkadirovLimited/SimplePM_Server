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

using System.IO;
using SimplePM_Server.Workers.Recourse;

namespace SimplePM_Server.Workers
{
    
    public partial class SWaiter
    {
        
        public void ServeSubmission()
        {
            
            // Запись информации о начале обработки запроса на проверку в лог-файл
            logger.Trace("Serving submission #" + _submissionInfo.SubmissionId + " started!");

            // Определяем конфигурацию компиляционного плагина
            var compilerConfiguration = SCompiler.GetCompilerConfig(
                _submissionInfo.UserSolution.ProgrammingLanguage
            );

            // Получаем экземпляр компиляционного плагина
            var compilerPlugin = SCompiler.FindCompilerPlugin(
                (string)(compilerConfiguration.module_name)
            );
            
            // Получаем полный путь к файлу исходного кода
            CreateSourceFile(ref compilerConfiguration, out var fileLocation);
            
            // Производим компиляцию пользовательского решения
            var cResult = SCompiler.ChooseCompilerAndRun(
                ref compilerConfiguration,
                ref compilerPlugin,
                _submissionInfo.SubmissionId.ToString(),
                fileLocation
            );
            
            // Выполняем тестирование пользовательского решения
            var testingResult = RunTesting(cResult);
            
            // Производим отправку результатов тестирования
            SendTestingResult(ref testingResult, cResult);

            // Удаляем все временные файлы, связанные с данной отправкой
            ClearCache(fileLocation);

            // Запись сведений об окончании отправки в лог-файл
            logger.Trace(
                "#" +
                _submissionInfo.SubmissionId +
                ": Submission testing completed!"
            );

        }

        private void CreateSourceFile(ref dynamic compilerConfiguration, out string fileLocation)
        {
            
            // Генерируем случайный путь для файла исходного кода
            fileLocation = RandomGenSourceFileLocation(
                _submissionInfo.SubmissionId.ToString(),
                "." + compilerConfiguration.source_ext
            );
            
            // Запись данных в файл исходного кода
            File.WriteAllBytes(fileLocation, _submissionInfo.UserSolution.SourceCode);

            // Установка аттрибутов файла исходного кода
            SetSourceFileAttributes(fileLocation);
            
        }
        
    }
    
}