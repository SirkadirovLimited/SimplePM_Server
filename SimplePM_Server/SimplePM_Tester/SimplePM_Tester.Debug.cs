/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

using System;
using System.IO;
using System.Text;
using CompilerBase;
using ProgramTesting;
using MySql.Data.MySqlClient;

namespace SimplePM_Server.SimplePM_Tester
{
    
    internal partial class SimplePM_Tester
    {
        
        /*
         * Функция, ответственная за debug
         * тестирование   пользовательских
         * решений.
         */

        public ProgramTestingResult Debug()
        {

            /*
             * Переменная хранит полный путь
             * к запускаемому файлу авторского
             * решения задачи
             */

            var authorSolutionExePath = GetAuthorSolutionExePath(
                out var authorLanguageConfiguration,
                out var authorCompilerPlugin
            );

            /*
             * Передаём новосозданным переменным
             * информацию о лимитах для пользовательского
             * процесса (пользовательского решения задачи)
             */

            GetDebugProgramLimits(
                out var memoryLimit, // переменная, хранящая значение лимита по памяти
                out var timeLimit // переменная, хранящая значение лимита по процессорному времени
            );
            
            /*
             * Проводим нетестовый запуск авторского решения
             * и получаем всё необходимое для тестирования
             * пользовательской программы
             */

            var authorTestingResult = new ProgramTester(
                ref authorLanguageConfiguration,
                ref authorCompilerPlugin,
                authorSolutionExePath,
                "-author-solution",
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
            
            if (authorTestingResult.Result != TestResult.MiddleSuccessResult)
                throw new SimplePM_Exceptions.AuthorSolutionRunningException();
            
            /*
             * Получаем ссылку на объект, который
             * хранит информацию  о  конфигурации
             * компиляционного модуля для данного
             * языка программирования.
             */
            
            var userLanguageConfiguration = SimplePM_Compiler.GetCompilerConfig(
                ref _languageConfigurations,
                submissionInfo.CodeLang
            );

            /*
             * Получаем ссылку на объект,
             * созданный на основании класса,
             * который, в свою очередь, создан
             * по подобию интерфейса ICompilerPlugin.
             */
            
            var userCompilerPlugin = SimplePM_Compiler.FindCompilerPlugin(
                ref _compilerPlugins,
                (string)userLanguageConfiguration.module_name
            );

            /*
             * Проводим тестовый запуск пользовательского
             * решения поставленной задачи и получаем всё
             * необходимое для тестирования программы.
             */

            var userTestingResult = new ProgramTester(
                ref userLanguageConfiguration,
                ref userCompilerPlugin,
                exeFileUrl,
                "-user-solution",
                memoryLimit,
                timeLimit,
                submissionInfo.CustomTest,
                Encoding.UTF8.GetString(authorTestingResult.Output).Length * 2,
                submissionInfo.ProblemInformation.AdaptProgramOutput
            ).RunTesting();
            
            /*
             * Если результат тестирования не полностью
             * известен, осуществляем проверку по дополнительным
             * тестам и выдвигаем остаточный результат debug
             * тестирования пользовательского решения задачи.
             */
            
            if (userTestingResult.Result == TestResult.MiddleSuccessResult)
            {

                userTestingResult.Result =
                    Convert.ToBase64String(userTestingResult.Output) == Convert.ToBase64String(authorTestingResult.Output)
                    ? TestResult.FullSuccessResult
                    : TestResult.FullNoSuccessResult;
                
            }

            /*
             * Формируем результат тестирования пользовательского
             * решения поставленной задачи, добавляем информацию
             * о единственном тесте, который был проведен
             * непосредственно при тестировании данного
             * пользовательского решения данной задачи.
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
             * тестирования пользовательского
             * решения поставленной задачи.
             */

            return programTestingResult;

        }

        /*
         * Функция, ответственная за скачивание,
         * компиляцию и получение  полного  пути
         * к  авторскому   решению  поставленной
         * задачи.
         */

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
            
            authorLanguageConfiguration = SimplePM_Compiler.GetCompilerConfig(
                ref _languageConfigurations,
                submissionInfo.ProblemInformation.AuthorSolutionCodeLanguage
            );

            /*
             * Получаем ссылку на объект,
             * созданный на основании класса,
             * который, в свою очередь, создан
             * по подобию интерфейса ICompilerPlugin.
             */

            authorCompilerPlugin = SimplePM_Compiler.FindCompilerPlugin(
                ref _compilerPlugins,
                (string)authorLanguageConfiguration.module_name
            );
            
            /*
             * Компиляция авторского решения
             * поставленной задачи с последующим
             * возвращением результатов компиляции.
             */
            
            // Определяем расширение файла
            var authorFileExt = "." + (string)authorLanguageConfiguration.source_ext;

            // Получаем случайный путь к директории авторского решения
            var tmpAuthorDir = (string)_serverConfiguration.path.temp + 
                @"\author\" + Guid.NewGuid() + @"\";

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

            var tmpAuthorSrcLocation = 
                tmpAuthorDir + "sa" + 
                submissionInfo.SubmissionId + 
                authorFileExt;

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
                File.SetAttributes(
                    tmpAuthorSrcLocation,
                    FileAttributes.Temporary | FileAttributes.NotContentIndexed
                );

            }
            
            // Инициализируем экземпляр класса компилятора
            var compiler = new SimplePM_Compiler(
                ref authorLanguageConfiguration,
                ref authorCompilerPlugin,
                "a" + submissionInfo.SubmissionId,
                tmpAuthorSrcLocation
            );

            // Получаем структуру результата компиляции
            var cResult = compiler.ChooseCompilerAndRun();

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

        /*
         * Функция, ответственная за получение
         * информации  о  лимитах   для  debug
         * тестирования       пользовательских
         * решений  задач по программированию.
         */

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
                    `timeLimit` DESC,
                    `memoryLimit` DESC,
                    `id` ASC 
                LIMIT 
                    1
                ;
            ";

            var cmdSelect = new MySqlCommand(querySelect, connection);

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

                throw new SimplePM_Exceptions.DatabaseQueryException();

            }

        }

    }

}