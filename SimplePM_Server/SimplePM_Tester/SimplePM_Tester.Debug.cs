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
/*! \file */

using System;
using System.Collections.Generic;
using System.IO;
using CompilerBase;
using IniParser.Model;
using MySql.Data.MySqlClient;

namespace SimplePM_Server.SimplePM_Tester
{

    internal partial class SimplePM_Tester
    {
        
        public ProgramTestingResult Debug()
        {

            /*
             * Переменная хранит полный путь
             * к запускаемому файлу авторского
             * решения задачи
             **/
            var authorSolutionExePath = GetAuthorSolutionExePath(
                out var authorSolutionCodeLanguage
            );

            /*
             * Передаём новосозданным переменным
             * информацию о лимитах для пользовательского
             * процесса (пользовательского решения задачи)
             **/
            GetDebugProgramLimits(
                out var memoryLimit,
                out var timeLimit
            );
            
            /*
             * Проводим нетестовый запуск авторского решения
             * и получаем всё необходимое для тестирования
             * пользовательской программы
             **/
            var authorTestingResult = new ProgramTester(
                ref sConfig,
                ref sCompilersConfig,
                ref _compilerPlugins,
                authorSolutionCodeLanguage,
                authorSolutionExePath,
                "-author-solution",
                memoryLimit,
                timeLimit,
                submissionInfo.CustomTest,
                0,
                submissionInfo.AdaptProgramOutput
            ).RunTesting();

            if (authorTestingResult.Result != Test.MiddleSuccessResult)
                throw new SimplePM_Exceptions.AuthorSolutionRunningException();

            var userTestingResult = new ProgramTester(
                ref sConfig,
                ref sCompilersConfig,
                ref _compilerPlugins,
                submissionInfo.CodeLang,
                exeFileUrl,
                "-user-solution",
                memoryLimit,
                timeLimit,
                submissionInfo.CustomTest,
                authorTestingResult.Output.Length,
                submissionInfo.AdaptProgramOutput
            ).RunTesting();
            
            if (userTestingResult.Result == Test.MiddleSuccessResult)
            {

                userTestingResult.Result = (userTestingResult.Output == authorTestingResult.Output) ? '+' : '-';

            }

            var programTestingResult = new ProgramTestingResult(1)
            {
                TestingResults =
                {
                    [0] = userTestingResult
                }
            };

            return programTestingResult;

        }

        private string GetAuthorSolutionExePath(out string authorSolutionCodeLanguage)
        {
            ///////////////////////////////////////////////////
            // Выборка информации об авторском решении задачи
            ///////////////////////////////////////////////////

            // Запрос на выборку авторского решения из БД
            const string querySelect = @"
                SELECT 
                    `codeLang`, 
                    `code` 
                FROM 
                    `spm_problems_ready` 
                WHERE 
                    `problemId` = @problemId 
                ORDER BY 
                    `problemId` ASC 
                LIMIT 
                    1
                ;
            ";

            // Дескрипторы временных таблиц выборки из БД
            var cmdSelect = new MySqlCommand(querySelect, connection);

            // Параметры запроса
            cmdSelect.Parameters.AddWithValue("@problemId", submissionInfo.ProblemId);

            // Чтение результатов запроса
            var dataReader = cmdSelect.ExecuteReader();

            // Объявляем необходимые переменные
            byte[] authorProblemCode;

            // Читаем полученные данные
            if (dataReader.Read())
            {

                // Исходный код авторского решения
                authorProblemCode = (byte[]) dataReader["code"];

                // Язык авторского решения
                authorSolutionCodeLanguage = dataReader["codeLang"].ToString();

            }
            else
            {

                // Закрываем data reader
                dataReader.Close();

                // Авторское решение не найдено
                throw new SimplePM_Exceptions.AuthorSolutionNotFoundException();

            }

            // Закрываем data reader
            dataReader.Close();
            
            ///////////////////////////////////////////////////
            // Скачивание и компиляция авторского решения
            ///////////////////////////////////////////////////

            // Определяем расширение файла
            var authorFileExt = "." + SimplePM_Submission.GetExtByLang(
                authorSolutionCodeLanguage,
                ref _compilerPlugins
            );

            // Получаем случайный путь к директории авторского решения
            var tmpAuthorDir = sConfig["Program"]["tempPath"] + 
                @"\author\" + Guid.NewGuid() + @"\";

            // Создаём папку текущего авторского решения задачи
            Directory.CreateDirectory(tmpAuthorDir);

            // Определяем путь хранения файла исходного кода вторского решения
            var tmpAuthorSrcLocation = 
                tmpAuthorDir + "sa" + 
                submissionInfo.SubmissionId + 
                authorFileExt;

            // Записываем исходный код авторского решения в заданный временный файл
            File.WriteAllBytes(tmpAuthorSrcLocation, authorProblemCode);

            // Устанавливаем его аттрибуты
            File.SetAttributes(
                tmpAuthorSrcLocation,
                FileAttributes.Temporary | FileAttributes.NotContentIndexed
            );

            // Инициализируем экземпляр класса компилятора
            var compiler = new SimplePM_Compiler(
                ref sConfig,
                ref sCompilersConfig,
                ref _compilerPlugins,
                "a" + submissionInfo.SubmissionId,
                tmpAuthorSrcLocation,
                authorSolutionCodeLanguage
            );

            // Получаем структуру результата компиляции
            var cResult = compiler.ChooseCompilerAndRun();

            // В случае возникновения ошибки при компиляции
            // авторского решения аварийно завершаем работу
            if (cResult.HasErrors)
                throw new FileLoadException(cResult.ExeFullname);
            
            return cResult.ExeFullname;

        }

        private void GetDebugProgramLimits(out int memoryLimit, out int timeLimit)
        {

            // Запрос на выборку лимитов из БД
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

            // Дескрипторы временных таблиц выборки из БД
            var cmdSelect = new MySqlCommand(querySelect, connection);

            // Параметры запроса
            cmdSelect.Parameters.AddWithValue("@problemId", submissionInfo.ProblemId);

            // Чтение результатов запроса
            var dataReader = cmdSelect.ExecuteReader();
            
            // Читаем полученные данные
            if (dataReader.Read())
            {

                // Memory limit
                memoryLimit = int.Parse(dataReader["memoryLimit"].ToString());

                // Time limit
                timeLimit = int.Parse(dataReader["timeLimit"].ToString());

            }
            else
            {

                // Закрываем data reader
                dataReader.Close();

                // Авторское решение не найдено
                throw new SimplePM_Exceptions.UnknownException();

            }

            // Закрываем data reader
            dataReader.Close();

        }

    }

}