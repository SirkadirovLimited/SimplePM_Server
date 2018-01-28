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
        
        public void Debug()
        {

            /*
             * Переменная хранит полный путь
             * к запускаемому файлу авторского
             * решения задачи
             **/
            string authorSolutionCodeLanguage;
            string authorSolutionExePath = GetAuthorSolutionExePath(out authorSolutionCodeLanguage);

            /*
             * Передаём новосозданным переменным
             * информацию о лимитах для пользовательского
             * процесса (пользовательского решения задачи)
             **/
            int memoryLimit, timeLimit;
            GetDebugProgramLimits(out memoryLimit, out timeLimit);
            
            /*
             * Проводим нетестовый запуск авторского решения
             * и получаем всё необходимое для тестирования
             * пользовательской программы
             **/
            Test authorTestingResult = new ProgramTester(
                ref sConfig,
                ref sCompilersConfig,
                ref _compilerPlugins,
                authorSolutionCodeLanguage,
                authorSolutionExePath,
                "-author-solution",
                0,
                0,
                submissionInfo.CustomTest
            ).RunTesting();

            if (authorTestingResult.Result != Test.MiddleSuccessResult)
                throw new SimplePM_Exceptions.AuthorSolutionRunningException();

			throw new NotImplementedException();

        }

        private string GetAuthorSolutionExePath(out string authorSolutionCodeLanguage)
        {
            ///////////////////////////////////////////////////
            // Выборка информации об авторском решении задачи
            ///////////////////////////////////////////////////

            // Запрос на выборку авторского решения из БД
            string querySelect = $@"
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
            MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);

            // Параметры запроса
            cmdSelect.Parameters.AddWithValue("@problemId", submissionInfo.ProblemId);

            // Чтение результатов запроса
            MySqlDataReader dataReader = cmdSelect.ExecuteReader();

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
            string authorFileExt = "." + SimplePM_Submission.GetExtByLang(
                authorSolutionCodeLanguage,
                ref _compilerPlugins
            );

            // Получаем случайный путь к директории авторского решения
            string tmpAuthorDir = sConfig["Program"]["tempPath"] + 
                @"\author\" + Guid.NewGuid() + @"\";

            // Создаём папку текущего авторского решения задачи
            Directory.CreateDirectory(tmpAuthorDir);

            // Определяем путь хранения файла исходного кода вторского решения
            string tmpAuthorSrcLocation = 
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
            SimplePM_Compiler compiler = new SimplePM_Compiler(
                ref sConfig,
                ref sCompilersConfig,
                ref _compilerPlugins,
                "a" + submissionInfo.SubmissionId,
                tmpAuthorSrcLocation,
                authorSolutionCodeLanguage
            );

            // Получаем структуру результата компиляции
            CompilerResult cResult = compiler.ChooseCompilerAndRun();

            // В случае возникновения ошибки при компиляции
            // авторского решения аварийно завершаем работу
            if (cResult.HasErrors)
                throw new FileLoadException(cResult.ExeFullname);
            
            return cResult.ExeFullname;

        }

        private void GetDebugProgramLimits(out int memoryLimit, out int timeLimit)
        {

            // Запрос на выборку лимитов из БД
            string querySelect = $@"
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
            MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);

            // Параметры запроса
            cmdSelect.Parameters.AddWithValue("@problemId", submissionInfo.ProblemId);

            // Чтение результатов запроса
            MySqlDataReader dataReader = cmdSelect.ExecuteReader();
            
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