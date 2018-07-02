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

using NLog;
using System;
using System.IO;
using System.Text;
using CompilerPlugin;
using ServerExceptions;
using MySql.Data.MySqlClient;
using ProgramTestingAdditions;
using SimplePM_Server.Workers;
using SimplePM_Server.Workers.Recourse;
using SimplePM_Server.ProgramTesting.SRunner;

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

            // Получаем путь к авторскому решению поставленной задачи
            var authorSolutionExePath = GetAuthorSolutionExePath(
                out var authorLanguageConfiguration,
                out var authorCompilerPlugin
            );

            // Получаем лимиты для пользовательского решения
            GetDebugProgramLimits(
                out var memoryLimit, // переменная, хранящая значение лимита по памяти
                out var timeLimit // переменная, хранящая значение лимита по процессорному времени
            );
            
            // Запуск авторского решения поставленной задачи
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

            // Осуществляем проверки на наличие ошибок
            if (authorTestingResult.Result != SingleTestResult.PossibleResult.MiddleSuccessResult)
                throw new AuthorSolutionRunningException();
            
            // Определяем конфигурацию компиляционного плагина
            var userLanguageConfiguration = SCompiler.GetCompilerConfig(
                submissionInfo.CodeLang
            );
            
            // Получаем экземпляр компиляционного плагина
            var userCompilerPlugin = SCompiler.FindCompilerPlugin(
                (string)(userLanguageConfiguration.module_name)
            );

            // Запуск пользовательского решения поставленной задачи
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
            
            // В некоторых случаях необходимо произвести "допроверку"
            if (userTestingResult.Result == SingleTestResult.PossibleResult.MiddleSuccessResult)
            {

                // TODO: Implement checkers
                // TODO: Одинаковые части кода с ReleaseTesting!
                
                /*
                 * Сравнение выходных потоков
                 * и вынесение  результата по
                 * данному тесту.
                 */
                
                userTestingResult.Result =
                    Convert.ToBase64String(userTestingResult.Output) == Convert.ToBase64String(authorTestingResult.Output)
                    ? SingleTestResult.PossibleResult.FullSuccessResult
                    : SingleTestResult.PossibleResult.FullNoSuccessResult;
                
            }

            // Удаляем директорию с авторским решением
            Directory.Delete(
                new FileInfo(authorSolutionExePath).Directory?.FullName
                    ?? throw new AuthorSolutionNotFoundException(
                        "",
                        new DirectoryNotFoundException()
                    ),
                true
            );
            
            // Формируем результат тестирования пользовательского решения
            var programTestingResult = new ProgramTestingResult(1)
            {

                TestingResults =
                {
                    [0] = userTestingResult
                }

            };

            /*
             * Возвращаем сформированный результат тестирования
             * пользовательского решения поставленной задачи.
             */

            return programTestingResult;

        }
        
        private string GetAuthorSolutionExePath(
            out dynamic authorLanguageConfiguration,
            out ICompilerPlugin authorCompilerPlugin
        )
        {
            
            // Определяем конфигурацию компиляционного плагина
            authorLanguageConfiguration = SCompiler.GetCompilerConfig(
                submissionInfo.ProblemInformation.AuthorSolutionCodeLanguage
            );

            // Получаем экземпляр компиляционного плагина
            authorCompilerPlugin = SCompiler.FindCompilerPlugin(
                (string)(authorLanguageConfiguration.module_name)
            );
            
            /*
             * Компиляция авторского решения с последующим
             * возвращением результатов компиляции.
             */
            
            // Получаем случайный путь к директории авторского решения
            var tmpAuthorDir = Path.Combine(
                (string)(SWorker._serverConfiguration.path.temp),
                "author",
                Guid.NewGuid().ToString()
            );

            // Создаём папку текущего авторского решения задачи
            Directory.CreateDirectory(tmpAuthorDir);

            // Определяем путь хранения файла исходного кода авторского решения
            var tmpAuthorSrcLocation = Path.Combine(
                tmpAuthorDir,
                "sa" + ("." + (string)(authorLanguageConfiguration.source_ext))
            );

            lock (new object())
            {

                // Производим запись в файл исходного кода
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

            // Производим проверку на наличие ошибок
            if (cResult.HasErrors)
                throw new FileLoadException(cResult.ExeFullname);
            
            // Возвращаем полный путь к исполняемому файлу
            return cResult.ExeFullname;

        }
        
        private void GetDebugProgramLimits(out int memoryLimit, out int timeLimit)
        {

            // Создаём команду для СУБД
            var cmdSelect = new MySqlCommand(Resources.get_debug_limits, conn);

            // Добавляем параметры запроса
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

                // Выбрасываем исключение
                throw new DatabaseQueryException();

            }

        }
        
    }
    
}