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
using ProgramTestingAdditions;
using SimplePM_Server.Workers.Recourse;

namespace SimplePM_Server.Workers
{
    
    public partial class SWaiter
    {
        
        public void ServeSubmission()
        {
            
            /*
             * Записываем в лог-файл информацию о том,
             * что начата обработка данного запроса на
             * тестирование.
             */

            logger.Trace("Serving submission #" + _submissionInfo.SubmissionId + " started!");

            /*
             * Определяем   соответствующую  данному   запросу
             * на тестирование конфигурацию модуля компиляции.
             */

            var compilerConfiguration = SCompiler.GetCompilerConfig(
                _submissionInfo.CodeLang
            );

            var compilerPlugin = SCompiler.FindCompilerPlugin(
                (string)(compilerConfiguration.module_name)
            );

            /*
             * Определяем   расширение    файла
             * исходного кода пользовательского
             * решения поставленной задачи.
             */

            var fileExt = "." + compilerConfiguration.source_ext;
            
            /*
             * Случайным   образом   генерируем    путь
             * к файлу исходного кода пользовательского
             * решения поставленной задачи.
             */

            var fileLocation = RandomGenSourceFileLocation(
                _submissionInfo.SubmissionId.ToString(),
                fileExt
            );
            
            /*
             * Записываем  в  него   исходный   код,
             * очищаем  буфер   и   закрываем  поток
             * записи в данный файл.
             * При  этом,   осуществляем  побайтовую
             * запись в файл, дабы не повредить его.
             */

            File.WriteAllBytes(
                fileLocation,
                _submissionInfo.ProblemCode
            );

            /*
             * Устанавливаем   аттрибуты   данного   файла
             * таким образом, дабы  исключить  возможность
             * индексирования  его содержимого и остальные
             * не приятные для нас ситуации, которые могут
             * привести к непредвиденным последствиям.
             */

            SetSourceFileAttributes(fileLocation);

            /*
             * Вызываем функцию  запуска  компилятора для
             * данного языка программирования.
             * Функция возвращает информацию о результате
             * компиляции пользовательской программы.
             */

            var cResult = SCompiler.ChooseCompilerAndRun(
                ref compilerConfiguration,
                ref compilerPlugin,
                _submissionInfo.SubmissionId.ToString(),
                fileLocation
            );
            
            /*
             * Запускаем тестирование пользовательской
             * программы  по  указанному  его  типу  и
             * парметрам,   после  чего  получаем  его
             * результат.
             */

            ProgramTestingResult testingResult = RunTesting(
                cResult
            );
            
            /*
             * Отправляем результат тестирования
             * пользовательского решения постав-
             * ленной  задачи  на  сервер БД для
             * последующей обработки.
             */

            SendTestingResult(ref testingResult, cResult);

            /*
             * Вызываем метод,  который несёт
             * ответственность  за   удаление
             * всех временных файлов запросов
             * на  тестирование,  а  также за
             * вызов сборщика мусора.
             */
            
            ClearCache(fileLocation);

            /*
             * Записываем в лог-файл  информацию о том,
             * что    тестирование    пользовательского
             * решения завершено, но нам всё равно как.
             */
            
            logger.Trace(
                "#" +
                _submissionInfo.SubmissionId +
                ": Submission testing completed!"
            );

        }
        
    }
    
}