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

    /*
     * Класс официанта, который занимается
     * обработкой пользовательских запросов
     * на тестирование решений задач по
     * программированию.
     */

    internal class SimplePM_Officiant
    {

        /*
         * Объявляем переменную указателя
         * на менеджер  журнала собылий и
         * присваиваем  ей  указатель  на
         * журнал событий текущего класса
         */
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MySqlConnection connection; // Дескриптор соединения с БД
        private SubmissionInfo.SubmissionInfo submissionInfo; // Ссылка на объект, содержащий информацию о запросе на тестирование
        private IniData sConfig; // Дескриптор конфигурационного файла
        private IniData sCompilersConfig; // Дескриптор конфигурационного файла модулей компиляции
        private List<ICompilerPlugin> _compilerPlugins; // Список загруженных модулей компиляторв
        
        public SimplePM_Officiant(
            MySqlConnection connection,
            ref IniData sConfig,
            ref IniData sCompilersConfig,
            ref List<ICompilerPlugin> _compilerPlugins,
            SubmissionInfo.SubmissionInfo submissionInfo
        )
        {
            
            this.connection = connection;
            this.sConfig = sConfig;
            this.sCompilersConfig = sCompilersConfig;
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

            // Генерируем имя директории
            var directoryName = sConfig["Program"]["temp_path"] + 
                                @"\" + Guid.NewGuid() + 
                                submissionId + @"\";

            // Создаём все необходимые каталоги
            Directory.CreateDirectory(directoryName);

            // Возвращаем результат работы функции
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
            
            /*
             * Проводим работу с файлом исходного кода
             */

            // Определяем расширение файла
            var fileExt = "." + SimplePM_Submission.GetExtByLang(
                submissionInfo.CodeLang,
                ref _compilerPlugins
            );

            // Определяем полный путь к файлу
            var fileLocation = RandomGenSourceFileLocation(
                submissionInfo.SubmissionId.ToString(),
                fileExt
            );
            
            /*
             * Записываем в него исходный код,
             * очищаем буфер и закрываем поток
             * записи.
             */
            File.WriteAllBytes(
                fileLocation,
                submissionInfo.ProblemCode
            );

            // Устанавливаем его аттрибуты
            File.SetAttributes(
                fileLocation,
                FileAttributes.NotContentIndexed
            );

            // Объявляем экземпляр класса компиляции
            var compiler = new SimplePM_Compiler(
                ref sConfig,
                ref sCompilersConfig,
                ref _compilerPlugins,
                submissionInfo.SubmissionId.ToString(),
                fileLocation,
                submissionInfo.CodeLang
            );
            
            /*
             * Вызываем функцию  запуска  компилятора для
             * данного языка программирования.
             * Функция возвращает информацию о результате
             * компиляции пользовательской программы.
             */
            var cResult = compiler.ChooseCompilerAndRun();
            
            ///////////////////////////////////////////////////
            // Записываем в базу данных сообщение компилятора
            ///////////////////////////////////////////////////
            
            var queryUpdate = $@"
                UPDATE 
                    `spm_submissions` 
                SET 
                    `compiler_text` = '{cResult.CompilerMessage}' 
                WHERE 
                    `submissionId` = '{submissionInfo.SubmissionId}' 
                LIMIT 
                    1
                ;
            ";

            new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
            
            /*
             * Проверка на наличие ошибок при компиляции
             */
            if (cResult.HasErrors)
            {

                // Ошибка компиляции, записываем это в БД
                queryUpdate = $@"
                    UPDATE 
                        `spm_submissions` 
                    SET 
                        `status` = 'ready', 
                        `hasError` = true 
                    WHERE 
                        `submissionId` = '{submissionInfo.SubmissionId}' 
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
                    
                    /*
                     * Выполняем  различные  действия
                     * в зависимости от типа проверки
                     * решения поставленной задачи.
                     */
                    switch (submissionInfo.TestType)
                    {
                        case "syntax":
                            
                            break;
                        case "debug":
                            
                            break;
                        case "release":
                            
                            break;
                    }

                    ///////////////////////////////////////////////////
                    
                }
                catch (Exception ex)
                {

                    //Записываем информацию об ошибке в лог сервера
                    logger.Error(ex);

                    /*
                     * Делаем так, чтобы несчастливую
                     * отправку  обрабатывал   кто-то
                     * другой,    но    только  не мы
                     * (а может и мы, но позже).
                     */
                    queryUpdate = $@"
                        UPDATE 
                            `spm_submissions` 
                        SET 
                            `status` = 'ready', 
                            `errorOutput` = @errorOutput, 
                            `b` = '0'
                        WHERE 
                            `submissionId` = '{submissionInfo.SubmissionId}' 
                        LIMIT 
                            1
                        ;
                    ";

                    // Создаём команду
                    var cmd = new MySqlCommand(queryUpdate, connection);

                    // Устанавливаем значения параметров
                    cmd.Parameters.AddWithValue("@output", ex);

                    // Выполняем команду
                    cmd.ExecuteNonQuery();

                    // Вызываем сборщика мусора
                    ClearCache(cResult.ExeFullname, fileLocation);

                    // Выходим
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

            /*
             * Очищаем папку экзешников от мусора.
             * В случае ошибки ничего не делаем.
             */
            try
            {

                // Удаляем каталог временных файлов
                Directory.Delete(
                    new FileInfo(fileLocation).DirectoryName,
                    true
                );

                // Вызываем сборщик мусора оптимизированным методом
                GC.Collect(2, GCCollectionMode.Optimized);

            }
            catch (Exception) { /* Deal with it. */ }

        }
        
        ///////////////////////////////////////////////////
        
    }
}
