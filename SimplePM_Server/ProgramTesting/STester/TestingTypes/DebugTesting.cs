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

using System;
using System.IO;
using System.Text;
using CompilerPlugin;
using NLog;
using MySql.Data.MySqlClient;

using ProgramTestingAdditions;
using SimplePM_Exceptions;
using SimplePM_Server.ProgramTesting.SRunner;
using SimplePM_Server.Workers;
using SimplePM_Server.Workers.Recourse;

namespace SimplePM_Server.ProgramTesting.STester
{
    
    public class DebugTesting : TestingType
    {
        
        private static readonly Logger logger = LogManager.GetLogger("SimplePM_Server.ProgramTesting.STester.DebugTesting");

        public DebugTesting(
            ref MySqlConnection conn,
            string exeFilePath,
            ref SubmissionInfo.SubmissionInfo submissionInfo
        ) : base(ref conn, exeFilePath, ref submissionInfo) {  }

        public override ProgramTestingResult RunTesting()
        {

            /*
             * Переменная  хранит  полный путь
             * к запускаемому файлу авторского
             * решения задачи.
             */

            var authorSolutionExePath = GetAuthorSolutionExePath(
                out var authorLanguageConfiguration,
                out var authorCompilerPlugin
            );

            /*
             * Передаём       новосозданным      переменным
             * информацию  о  лимитах для пользовательского
             * процесса (пользовательского решения задачи).
             */

            GetDebugProgramLimits(
                out var memoryLimit, // переменная, хранящая значение лимита по памяти
                out var timeLimit // переменная, хранящая значение лимита по процессорному времени
            );
            
            /*
             * Проводим нетестовый запуск авторского решения
             * и получаем всё необходимое  для  тестирования
             * пользовательской программы.
             */

            var authorTestingResult = new ProgramExecutor(
                authorLanguageConfiguration,
                authorCompilerPlugin,
                authorSolutionExePath,
                "--author-solution",
                memoryLimit,
                timeLimit,
                submissionInfo.CustomTest,
                0,
                submissionInfo.ProblemInformation.AdaptProgramOutput
            ).RunTesting();

            /*
             * Проверяем,    успешно  ли  проведен  запуск
             * авторского   решения     задачи.   В случае
             * обнаружения  каких-либо  ошибок выбрасываем
             * исключение  AuthorSolutionRunningException,
             * которое информирует улавливатель исключений
             * о необходимости  предоставления  информации
             * об  ошибке  в  лог-файлах  сервера и прочих
             * местах, где это важно и необходимо.
             */
            
            if (authorTestingResult.Result != SingleTestResult.PossibleResult.MiddleSuccessResult)
                throw new AuthorSolutionRunningException();
            
            /*
             * Получаем ссылку на объект, который
             * хранит информацию  о  конфигурации
             * компиляционного модуля для данного
             * языка программирования.
             */
            
            var userLanguageConfiguration = SCompiler.GetCompilerConfig(
                submissionInfo.CodeLang
            );

            /*
             * Получаем     ссылку     на     объект,
             * созданный    на    основании   класса,
             * который,   в   свою  очередь,   создан
             * по подобию интерфейса ICompilerPlugin.
             */
            
            var userCompilerPlugin = SCompiler.FindCompilerPlugin(
                (string)(userLanguageConfiguration.module_name)
            );

            /*
             * Проводим тестовый запуск пользовательского
             * решения поставленной задачи и получаем всё
             * необходимое для тестирования программы.
             */

            var userTestingResult = new ProgramExecutor(
                userLanguageConfiguration,
                userCompilerPlugin,
                exeFilePath,
                "--user-solution",
                memoryLimit,
                timeLimit,
                submissionInfo.CustomTest,
                Encoding.UTF8.GetString(authorTestingResult.Output).Length * 2,
                submissionInfo.ProblemInformation.AdaptProgramOutput
            ).RunTesting();
            
            /*
             * Если   результат    тестирования   не   полностью
             * известен, осуществляем проверку по дополнительным
             * тестам  и  выдвигаем  остаточный  результат debug
             * тестирования пользовательского решения задачи.
             */
            
            if (userTestingResult.Result == SingleTestResult.PossibleResult.MiddleSuccessResult)
            {

                // TODO: Implement checkers
                
                userTestingResult.Result =
                    Convert.ToBase64String(userTestingResult.Output) == Convert.ToBase64String(authorTestingResult.Output)
                    ? SingleTestResult.PossibleResult.FullSuccessResult
                    : SingleTestResult.PossibleResult.FullNoSuccessResult;
                
            }

            /*
             * Производим удаление директории
             * авторского решения поставленно
             * й задачи для экономии места на
             * диске.
             */
            
            Directory.Delete(
                new FileInfo(authorSolutionExePath).Directory?.FullName
                    ?? throw new AuthorSolutionNotFoundException(
                        "",
                        new DirectoryNotFoundException()
                    ),
                true
            );
            
            /*
             * Формируем результат тестирования пользовательского
             * решения поставленной задачи,  добавляем информацию
             * о  единственном  тесте,   который   был   проведен
             * непосредственно    при    тестировании     данного
             * пользовательского решения поставленной задачи.
             */

            var programTestingResult = new ProgramTestingResult(1)
            {

                TestingResults =
                {
                    [0] = userTestingResult
                }

            };

            /*
             * Возвращаем сформированный результат
             * тестирования      пользовательского
             * решения поставленной задачи.
             */

            return programTestingResult;

        }
        
        private string GetAuthorSolutionExePath(
            out dynamic authorLanguageConfiguration,
            out ICompilerPlugin authorCompilerPlugin
        )
        {
            
            /*
             * Получаем ссылку на объект, который
             * хранит информацию  о  конфигурации
             * компиляционного модуля для данного
             * языка программирования.
             */
            
            authorLanguageConfiguration = SCompiler.GetCompilerConfig(
                submissionInfo.ProblemInformation.AuthorSolutionCodeLanguage
            );

            /*
             * Получаем ссылку на объект,
             * созданный на основании класса,
             * который, в свою очередь, создан
             * по подобию интерфейса ICompilerPlugin.
             */

            authorCompilerPlugin = SCompiler.FindCompilerPlugin(
                (string)(authorLanguageConfiguration.module_name)
            );
            
            /*
             * Компиляция авторского решения
             * поставленной задачи с последующим
             * возвращением результатов компиляции.
             */
            
            // Определяем расширение файла
            var authorFileExt = "." + (string)(authorLanguageConfiguration.source_ext);

            // Получаем случайный путь к директории авторского решения
            var tmpAuthorDir = Path.Combine(
                (string)(SWorker._serverConfiguration.path.temp),
                "author",
                Guid.NewGuid().ToString()
            );

            /*
             * Создаём   папку   текущего
             * авторского решения задачи.
             */

            Directory.CreateDirectory(tmpAuthorDir);

            /*
             * Определяем путь хранения
             * файла   исходного   кода
             * авторского решения.
             */

            var tmpAuthorSrcLocation = Path.Combine(
                tmpAuthorDir,
                "sa" + authorFileExt
            );

            /*
             * Для обеспечения безопасности синхронизируем потоки
             */

            lock (new object())
            {

                /*
                 * Записываем исходный код авторского
                 * решения в заданный временный файл.
                 */

                File.WriteAllBytes(
                    tmpAuthorSrcLocation,
                    submissionInfo.ProblemInformation.AuthorSolutionCode
                );

                // Устанавливаем его аттрибуты
                SWaiter.SetSourceFileAttributes(tmpAuthorSrcLocation);
                
            }
            
            // Выполняем компиляцию авторского решения данной задачи
            var cResult = SCompiler.ChooseCompilerAndRun(
                ref authorLanguageConfiguration,
                ref authorCompilerPlugin,
                "a",
                tmpAuthorSrcLocation
            );

            /*
             * В случае возникновения ошибки при компиляции
             * авторского решения аварийно завершаем работу
             * функции и выбрасываем исключение, содержащее
             * информацию о файле и причине ошибки при  его
             * открытии.
             */

            if (cResult.HasErrors)
                throw new FileLoadException(cResult.ExeFullname);
            
            /*
             * Возвращаем  полный  путь к исполняемому
             * файлу авторского решения данной задачи.
             */

            return cResult.ExeFullname;

        }
        
        private void GetDebugProgramLimits(out int memoryLimit, out int timeLimit)
        {

            /*
             * Формируем и выполняем запрос к
             * базе  данных  системы, который
             * позволит нам узнать информацию
             * о лимитах для пользовательской
             * программы.
             */

            // Формируем SQL запрос
            const string querySelect = @"
                SELECT 
                    `memoryLimit`, 
                    `timeLimit` 
                FROM 
                    `spm_problems_tests` 
                WHERE 
                    `problemId` = @problemId 
                ORDER BY 
                    `id` ASC 
                LIMIT 
                    1
                ;
            ";

            var cmdSelect = new MySqlCommand(querySelect, conn);

            cmdSelect.Parameters.AddWithValue(
                "@problemId",
                submissionInfo.ProblemInformation.ProblemId
            );

            // Чтение результатов запроса
            var dataReader = cmdSelect.ExecuteReader();
            
            // Читаем полученные данные
            if (dataReader.Read())
            {

                // Получаем лимит по используемой программой памяти
                memoryLimit = int.Parse(dataReader["memoryLimit"].ToString());

                // Получаем лимит по используемом программой процессорному времени
                timeLimit = int.Parse(dataReader["timeLimit"].ToString());

                // Закрываем data reader
                dataReader.Close();

            }
            else
            {

                // Закрываем data reader
                dataReader.Close();

                /*
                 * Выбрасываем исключение, которое означает
                 * присутствие ошибки при попытке получения
                 * информации с базы данных системы.
                 */

                throw new DatabaseQueryException();

            }

        }
        
    }
    
}