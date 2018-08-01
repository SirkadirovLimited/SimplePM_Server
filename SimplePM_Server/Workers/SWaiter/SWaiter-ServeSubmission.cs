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