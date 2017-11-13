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

// Основа всего
using System;
// Подключаем коллекции
using System.Collections.Generic;
// Подключение к БД
using MySql.Data.MySqlClient;
// Конфигурационный файл
using IniParser.Model;
// Работа с файлами
using System.IO;
// Работа с компиляторами
using CompilerBase;
// Журнал событий
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

    internal class SimplePM_Officiant
    {

        ///////////////////////////////////////////////////
        // РАЗДЕЛ ОБЪЯВЛЕНИЯ ГЛОБАЛЬНЫХ ПЕРЕМЕННЫХ
        ///////////////////////////////////////////////////

        /*!
            Объявляем переменную указателя на менеджер журнала собылий
            и присваиваем ей указатель на журнал событий текущего класса
        */
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MySqlConnection connection; //!< Дескриптор соединения с БД
        private Dictionary<string, string> submissionInfo; //!< Словарь информации о запросе
        private IniData sConfig; //!< Дескриптор конфигурационного файла
        private List<ICompilerPlugin> _compilerPlugins; //!< Список загруженных модулей компиляторв

        ///////////////////////////////////////////////////
        /// Функция-конструктор официанта, обрабатывающего
        /// пользовательский запрос на тестирование
        /// решения поставленной задачи.
        ///////////////////////////////////////////////////

        public SimplePM_Officiant(MySqlConnection connection, ref IniData sConfig, ref List<ICompilerPlugin> _compilerPlugins, Dictionary<string, string> submissionInfo)
        {

            this.connection = connection;
            this.sConfig = sConfig;
            this._compilerPlugins = _compilerPlugins;
            this.submissionInfo = submissionInfo;

        }

        ///////////////////////////////////////////////////
        /// Функция, генерирующая случайный путь к файлу,
        /// содержащему исходный код пользовательского
        /// решения поставленной задачи. Можно
        /// использовать данную функцию и в других целях.
        ///////////////////////////////////////////////////

        public string RandomGenSourceFileLocation(string submissionId, string fileExt)
        {

            //Генерируем имя директории
            string directoryName = sConfig["Program"]["tempPath"] + @"\" + Guid.NewGuid() + submissionId + @"\";

            //Создаём все необходимые каталоги
            Directory.CreateDirectory(directoryName);

            //Возвращаем результат работы функции
            return directoryName + "s" + submissionId + fileExt;

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
            
            ///////////////////////////////////////////////////
            // РАБОТА С ФАЙЛОМ ИСХОДНОГО КОДА
            ///////////////////////////////////////////////////

            //Определяем расширение файла
            string fileExt = "." + SimplePM_Submission.GetExtByLang(submissionInfo["codeLang"], ref _compilerPlugins);
            //Определяем полный путь к файлу
            string fileLocation = RandomGenSourceFileLocation(submissionInfo["submissionId"], fileExt);

            //Создаём файл исходного кода
            StreamWriter codeWriter = File.CreateText(fileLocation);
            
            //Записываем в него исходный код, очищаем буфер и закрываем поток записи
            codeWriter.WriteLine(submissionInfo["problemCode"]);
            codeWriter.Flush();
            codeWriter.Close();

            //Устанавливаем его аттрибуты
            File.SetAttributes(fileLocation, FileAttributes.Temporary | FileAttributes.NotContentIndexed);

            //Объявляем экземпляр класса компиляции
            SimplePM_Compiler compiler = new SimplePM_Compiler(
                ref sConfig,
                ref _compilerPlugins,
                submissionInfo["submissionId"],
                fileLocation,
                submissionInfo["codeLang"]
            );

            ///////////////////////////////////////////////////
            // Вызываем функцию запуска компилятора для
            // данного языка программирования.
            // Функция возвращает информацию о результате
            // компиляции пользовательской программы.
            ///////////////////////////////////////////////////

            CompilerResult cResult = compiler.ChooseCompilerAndRun();

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

                            try
                            {

                                //Запускаем тестирование программы
                                new SimplePM_Tester(
                                    ref connection, // дескриптор соединения с БД
                                    ref _compilerPlugins, // список модулей поддержки компиляторов
                                    ref cResult.ExeFullname, // полный путь к исполняемому файлу
                                    ref submissionInfo, // информация о запросе на тестирование
                                    ref sConfig // дескриптор конфигурационного файла сервера
                                ).DebugTest();

                            }
                            //Выполняем необходимые действия в случае,
                            //когда авторское решение "ломается" при запуске или работе
                            catch (SimplePM_Exceptions.AuthorSolutionRunningException)
                            {

                                //Запрос на обновление данных в базе данных
                                queryUpdate = $@"
                                UPDATE 
                                    `spm_submissions` 
                                    SET 
                                        `status` = 'ready', 
                                        `errorOutput` = 'ERR_AUTHOR_SOLUTION_CRASHED', 
                                        `hasError` = true 
                                    WHERE 
                                        `submissionId` = '{submissionInfo["submissionId"]}' 
                                    LIMIT 
                                        1
                                    ;
                                ";

                                //Выполняем запрос, адресованный к серверу баз данных MySQL
                                new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();

                            }
                            //Выполняем необходимые действия в случае,
                            //когда авторское решение для задачи не найдено
                            catch (SimplePM_Exceptions.AuthorSolutionNotFoundException)
                            {

                                //Запрос на обновление данных в базе данных
                                queryUpdate = $@"
                                    UPDATE 
                                        `spm_submissions` 
                                    SET 
                                        `status` = 'ready', 
                                        `errorOutput` = 'ERR_NO_AUTHOR_SOLUTION', 
                                        `hasError` = true 
                                    WHERE 
                                        `submissionId` = '{submissionInfo["submissionId"]}' 
                                    LIMIT 
                                        1
                                    ;
                                ";

                                //Выполняем запрос, адресованный к серверу баз данных MySQL
                                new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();

                            }

                            break;
                        //Отправка решения задачи
                        case "release":

                            //Запускаем тестирование программы
                            new SimplePM_Tester(
                                ref connection, // дескриптор соединения с БД
                                ref _compilerPlugins, // список модулей поддержки компиляторов
                                ref cResult.ExeFullname, // полный путь к исполняемому файлу
                                ref submissionInfo, // информация о запросе на тестирование
                                ref sConfig
                            ).ReleaseTest();

                            break;
                    }

                    ///////////////////////////////////////////////////
                    
                }

                ///////////////////////////////////////////////////
                // ДЕЙСТВИЯ В СЛУЧАЕ ОШИБКИ СЕРВЕРА ПРОВЕРКИ
                ///////////////////////////////////////////////////
                
                catch (Exception ex)
                {

                    //Записываем информацию об ошибке в лог сервера
                    logger.Error(ex);

                    //Делаем так, чтобы несчастливую отправку обрабатывал
                    //кто-то другой, но только не мы (а может и мы, но позже)
                    queryUpdate = $@"
                        UPDATE 
                            `spm_submissions` 
                        SET 
                            `status` = 'ready', 
                            `errorOutput` = @errorOutput, 
                            `b` = '0'
                        WHERE 
                            `submissionId` = '{submissionInfo["submissionId"]}' 
                        LIMIT 
                            1
                        ;
                    ";

                    //Создаём команду
                    MySqlCommand cmd = new MySqlCommand(queryUpdate, connection);

                    //Устанавливаем значения параметров
                    cmd.Parameters.AddWithValue("@output", ex);

                    //Выполняем команду
                    cmd.ExecuteNonQuery();

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

            //Очищаем папку экзешников от мусора.
            //В случае ошибки ничего не делаем.
            try
            {

                //Удаляем каталог временных файлов
                Directory.Delete(new FileInfo(fileLocation).DirectoryName, true);

                //Вызываем сборщик мусора оптимизированным методом
                GC.Collect(2, GCCollectionMode.Optimized);

            }
            catch (Exception) {  }

        }
        
        ///////////////////////////////////////////////////
        
    }
}
