/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under CC BY-NC-SA 4.0 license.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */
/*! \file */

//Основа
using System;
//Подключаем коллекции
using System.Collections.Generic;
//Подключение к БД
using MySql.Data.MySqlClient;
//Конфигурационный файл
using IniParser.Model;
//Работа с файлами
using System.IO;
//Журнал событий
using NLog;

namespace SimplePM_Server
{
    /*!
     * \brief
     * Класс официанта, который занимается
     * обработкой пользовательских запросов
     * на тестирование решений задач по
     * программированию.
     */

    class SimplePM_Officiant
    {
        ///////////////////////////////////////////////////
        // РАЗДЕЛ ОБЪЯВЛЕНИЯ ГЛОБАЛЬНЫХ ПЕРЕМЕННЫХ
        ///////////////////////////////////////////////////
        
        /*!
            Объявляем переменную указателя на менеджер журнала собылий
            и присваиваем ей указатель на журнал событий текущего класса
        */
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public MySqlConnection connection; //!< Дескриптор соединения с БД
        public Dictionary<string, string> submissionInfo; //!< Словарь информации о запросе
        public IniData sConfig; //!< Дескриптор конфигурационного файла

        ///////////////////////////////////////////////////
        /// Функция-конструктор официанта, обрабатывающего
        /// пользовательский запрос на тестирование
        /// решения поставленной задачи.
        ///////////////////////////////////////////////////

        public SimplePM_Officiant(MySqlConnection connection, IniData sConfig, Dictionary<string, string> submissionInfo)
        {
            this.connection = connection;
            this.sConfig = sConfig;
            this.submissionInfo = submissionInfo;
        }

        ///////////////////////////////////////////////////
        /// Функция серверует запрос на тестирование,
        /// контролирует работу компиляторов и
        /// тестировщиков. Всё необходимое для работы
        /// функции изымается из глобальных переменных
        /// класса SimplePM_Officiant (текущего).
        ///////////////////////////////////////////////////

        public void ServeSubmission()
        {
            //Определяем язык написания пользовательской программы
            SimplePM_Submission.SubmissionLanguage codeLang = SimplePM_Submission.GetCodeLanguageByName(submissionInfo["codeLang"]);

            ///////////////////////////////////////////////////
            // РАБОТА С ФАЙЛОМ ИСХОДНОГО КОДА
            ///////////////////////////////////////////////////

            //Определяем расширение файла
            string fileExt = "." + SimplePM_Submission.GetExtByLang(codeLang);
            //Определяем полный путь к файлу
            string fileLocation = sConfig["Program"]["tempPath"] + submissionInfo["submissionId"] + fileExt;

            //Создаём файл исходного кода
            StreamWriter codeWriter = File.CreateText(fileLocation);

            //Устанавливаем его аттрибуты
            File.SetAttributes(fileLocation, FileAttributes.Temporary | FileAttributes.NotContentIndexed);
            
            //Записываем в него исходный код, очищаем буфер и закрываем поток записи
            codeWriter.WriteLine(submissionInfo["problemCode"]);
            codeWriter.Flush();
            codeWriter.Close();

            //Объявляем экземпляр класса компиляции
            SimplePM_Compiler compiler = new SimplePM_Compiler(ref sConfig, ulong.Parse(submissionInfo["submissionId"]), fileExt);
            //Объявляем переменную результата компиляции
            SimplePM_Compiler.CompilerResult cResult;

            ///////////////////////////////////////////////////
            // ЗАПУСК СПЕЦИФИЧЕСКОГО КОМПИЛЯТОРА В ЗАВИСИМОСТИ
            // ОТ ЯЗЫКА РЕШЕНИЯ ЗАДАЧИ
            ///////////////////////////////////////////////////

            switch (codeLang)
            {
                /*   COMPILERS REQUIRED   */
                case SimplePM_Submission.SubmissionLanguage.Freepascal:
                    //Запускаем компилятор Pascal
                    cResult = compiler.StartFreepascalCompiler();
                    break;
                case SimplePM_Submission.SubmissionLanguage.CSharp:
                    //Запускаем компилятор C#
                    cResult = compiler.StartCSharpCompiler();
                    break;
                case SimplePM_Submission.SubmissionLanguage.C:
                    //Запускаем компилятор C
                    cResult = compiler.StartCCompiler();
                    break;
                case SimplePM_Submission.SubmissionLanguage.Cpp:
                    //Запускаем компилятор C++
                    cResult = compiler.StartCppCompiler();
                    break;

                /*   NO COMPILERS REQUIRED   */
                case SimplePM_Submission.SubmissionLanguage.Lua:
                case SimplePM_Submission.SubmissionLanguage.Python:
                case SimplePM_Submission.SubmissionLanguage.PHP:
                    //Некоторым файлам не требуется компиляция
                    //но для обратной совместимости функцию вкатать нужно
                    cResult = compiler.StartNoCompiler();
                    break;
                
                /*   LANGUAGE NOT SUPPORTED BY SYSTEM   */
                default:
                    cResult = new SimplePM_Compiler.CompilerResult();
                    cResult.HasErrors = true;
                    cResult.CompilerMessage = "Language not supported by SimplePM!";
                    break;
            }
            ///////////////////////////////////////////////////
            // Записываем в базу данных сообщение компилятора
            ///////////////////////////////////////////////////
            string queryUpdate = $@"
                UPDATE 
                    `spm_submissions` 
                SET 
                    `compiler_text` = '{cResult.CompilerMessage}' 
                WHERE 
                    `submissionId` = '{submissionInfo["submissionId"]}' 
                LIMIT 
                    1
                ;
            ";
            new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();

            ///////////////////////////////////////////////////
            // ПРОВЕРКА НА НАЛИЧИЕ ОШИБОК ПРИ КОМПИЛЯЦИИ
            ///////////////////////////////////////////////////

            if (cResult.HasErrors)
            {
                //Ошибка компиляции, записываем это в БД
                queryUpdate = $@"
                    UPDATE 
                        `spm_submissions` 
                    SET 
                        `status` = 'ready', 
                        `hasError` = true 
                    WHERE 
                        `submissionId` = '{submissionInfo["submissionId"]}' 
                    LIMIT 
                        1
                    ;
                ";
                new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
            }
            else
            {
                try
                {
                    ///////////////////////////////////////////////////
                    // ВЫПОЛНЯЕМ РАЗЛИЧНЫЕ ДЕЙСТВИЯ В ЗАВИСИМОСТИ ОТ
                    // ТИПА ПРОВЕРКИ РЕШЕНИЯ ПОСТАВЛЕННОЙ ЗАДАЧИ
                    ///////////////////////////////////////////////////

                    switch (submissionInfo["testType"])
                    {
                        //Проверка синтаксиса
                        case "syntax":

                            queryUpdate = $@"
                                UPDATE 
                                    `spm_submissions` 
                                SET 
                                    `status` = 'ready' 
                                WHERE 
                                    `submissionId` = '{submissionInfo["submissionId"]}' 
                                LIMIT 
                                    1
                                ;
                            ";
                            new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();

                            break;
                        //Отладка программы по пользовательскому тесту
                        case "debug":
                            //Запускаем тестирование программы
                            new SimplePM_Tester(
                                ref connection, //дескриптор соединения с БД
                                ref cResult.ExeFullname, //полный путь к исполняемому файлу
                                ref submissionInfo, //информация о запросе на тестирование
                                ref sConfig //дескриптор конфигурационного файла сервера
                            ).DebugTest();

                            break;
                        //Отправка решения задачи
                        case "release":

                            //Запускаем тестирование программы
                            new SimplePM_Tester(
                                ref connection, //дескриптор соединения с БД
                                ref cResult.ExeFullname, //полный путь к исполняемому файлу
                                ref submissionInfo, //информация о запросе на тестирование
                                ref sConfig
                            ).ReleaseTest();

                            break;
                    }

                    ///////////////////////////////////////////////////
                }

                ///////////////////////////////////////////////////
                // ДЕЙСТВИЯ В СЛУЧАЕ ОШИБКИ СЕРВЕРА ПРОВЕРКИ
                ///////////////////////////////////////////////////
                
                catch (Exception)
                {
                    //Делаем так, чтобы несчастливую отправку обрабатывал
                    //кто-то другой, но только не мы (а может и мы, но позже)
                    queryUpdate = $@"
                        UPDATE 
                            `spm_submissions` 
                        SET 
                            `status` = 'waiting' 
                        WHERE 
                            `submissionId` = '{submissionInfo["submissionId"]}' 
                        LIMIT 
                            1
                        ;
                    ";
                    new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();

                    //Вызываем сборщика мусора
                    ClearCache(cResult.ExeFullname, fileLocation);

                    //Выходим
                    return;
                }

            }

            //Вызываем сборщика мусора
            ClearCache(cResult.ExeFullname, fileLocation);
        }

        ///////////////////////////////////////////////////
        /// Функция очищает кэш, временные файлы и
        /// совершает вызов системного сборщика мусора.
        /// Используется для экономии оперативной памяти
        /// сервера, на котором работает SimplePM_Server.
        ///////////////////////////////////////////////////

        public void ClearCache(string exe_fullname, string fileLocation)
        {
            //Очищаем папку экзешников от мусора
            try
            {
                File.Delete(exe_fullname);
                File.Delete(fileLocation);
                GC.Collect(2, GCCollectionMode.Optimized);
            }
            catch (Exception) { }
        }
        
        ///////////////////////////////////////////////////
    }
}
