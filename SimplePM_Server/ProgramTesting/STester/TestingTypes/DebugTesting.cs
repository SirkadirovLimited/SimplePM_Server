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

using NLog;
using System;
using System.IO;
using System.Text;
using CompilerPlugin;
using ServerExceptions;
using MySql.Data.MySqlClient;
using SimplePM_Server.Workers;
using SimplePM_Server.Workers.Recourse;
using SProgramRunner;

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

        public override SolutionTestingResult RunTesting()
        {

            logger.Trace("#" + submissionInfo.SubmissionId + ": DebugTesting.RunTesting() [started]");
            
            // Получаем лимиты для пользовательского решения
            GetDebugProgramLimits(
                out var memoryLimit, // переменная, хранящая значение лимита по памяти
                out var timeLimit // переменная, хранящая значение лимита по процессорному времени
            );

            var processLimitsInfo = new TestingRequestStuct.ProcessLimitsInfo
            {

                Enable = true,

                ProcessorTimeLimit = timeLimit,
                ProcessWorkingSetLimit = memoryLimit,
                ProcessRealWorkingTimeLimit = timeLimit * 5

            };
            
            //========================================================================================================//
            
            // Получаем путь к авторскому решению поставленной задачи
            var authorSolutionExePath = GetAuthorSolutionExePath(
                out var authorLanguageConfiguration,
                out var authorCompilerPlugin
            );
            
            // Run author solution
            var authorTestingResult = new SRunner(
                
                new TestingRequestStuct
                {
                    
                    RuntimeInfo = authorCompilerPlugin.SetRunningMethod(
                        ref authorLanguageConfiguration,
                        authorSolutionExePath,
                        "--author-solution"
                    ),
                    
                    RunAsInfo = defaultRunAsInfo,
                    
                    LimitsInfo = processLimitsInfo,
                    
                    IOConfig = new TestingRequestStuct.ProcessIOConfig
                    {
                        
                        ProgramInput = submissionInfo.CustomTest,
                        
                        WriteInputToFile = true,
                        InputFileName = "input",
                        
                        PreferReadFromOutputFile = true,
                        OutputFileName = "output",
                        
                        OutputCharsLimit = -1
                        
                    }
                    
                }
                
            ).Execute();
            
            // Осуществляем проверки на наличие ошибок
            if (!authorTestingResult.IsMiddleSuccessful)
                throw new AuthorSolutionException("AUTHOR_SOLUTION_RUNTIME_EXCEPTION");
            
            //========================================================================================================//
            
            // Определяем конфигурацию компиляционного плагина
            var userLanguageConfiguration = SCompiler.GetCompilerConfig(
                submissionInfo.UserSolution.ProgrammingLanguage
            );

            // Run user's solution
            var userTestingResult = new SRunner(
                
                new TestingRequestStuct
                {
                    
                    RuntimeInfo = SCompiler.FindCompilerPlugin(
                        (string)(userLanguageConfiguration.module_name)
                    ).SetRunningMethod(
                        ref userLanguageConfiguration,
                        exeFilePath,
                        "--user-solution"
                    ),
                    
                    RunAsInfo = defaultRunAsInfo,
                    
                    LimitsInfo = processLimitsInfo,
                    
                    IOConfig = new TestingRequestStuct.ProcessIOConfig
                    {
                        
                        ProgramInput = submissionInfo.CustomTest,
                        
                        WriteInputToFile = true,
                        InputFileName = "input",
                        
                        PreferReadFromOutputFile = true,
                        OutputFileName = "output",
                        
                        OutputCharsLimit = Encoding.UTF8.GetString(authorTestingResult.ProgramOutputData).Length * 2
                        
                    }
                    
                }
                
            ).Execute();
            
            //========================================================================================================//
            
            // Выносим финальный вердикт по тесту
            MakeFinalTestResult(ref userTestingResult, authorTestingResult.ProgramOutputData);

            // Удаляем директорию с авторским решением
            SWaiter.ClearCache(authorSolutionExePath);
            
            // Формируем результат тестирования пользовательского решения
            var programTestingResult = new SolutionTestingResult(1)
            {

                TestingResults =
                {
                    [0] = userTestingResult
                }

            };

            logger.Trace("#" + submissionInfo.SubmissionId + ": DebugTesting.RunTesting() [finished]");
            
            // Возвращаем результат тестирования пользовательского решения
            return programTestingResult;

        }
        
        private string GetAuthorSolutionExePath(
            out dynamic authorLanguageConfiguration,
            out ICompilerPlugin authorCompilerPlugin
        )
        {
            
            logger.Trace("GetAuthorSolutionExePath for submission #" + submissionInfo.SubmissionId + " [started]");
            
            // Определяем конфигурацию компиляционного плагина
            authorLanguageConfiguration = SCompiler.GetCompilerConfig(
                submissionInfo.ProblemInformation.AuthorSolution.ProgrammingLanguage
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
                    submissionInfo.ProblemInformation.AuthorSolution.SourceCode
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
                throw new FileLoadException("Author solution executable file not found! Path: " + cResult.ExeFullname);
            
            logger.Trace("GetAuthorSolutionExePath for submission #" + submissionInfo.SubmissionId + " [finished]");
            
            // Возвращаем полный путь к исполняемому файлу
            return cResult.ExeFullname;

        }
        
        private void GetDebugProgramLimits(out int memoryLimit, out int timeLimit)
        {

            logger.Trace("GetDebugProgramLimits for submission #" + submissionInfo.SubmissionId + " [started]");
            
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
                throw new DatabaseQueryException("MySQL query in DebugTesting.GetDebugProgramLimits() failed!");

            }
            
            logger.Trace("GetDebugProgramLimits for submission #" + submissionInfo.SubmissionId + " [finished]");

        }
        
    }
    
}