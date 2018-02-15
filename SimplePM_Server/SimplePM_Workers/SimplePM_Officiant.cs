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

        private MySqlConnection _connection; // Дескриптор соединения с БД
        private SubmissionInfo.SubmissionInfo _submissionInfo; // Ссылка на объект, содержащий информацию о запросе на тестирование
        private dynamic _serverConfiguration;
        private List<ICompilerPlugin> _compilerPlugins; // Список загруженных модулей компиляторв
        
        public SimplePM_Officiant(
            MySqlConnection connection,
            ref dynamic serverConfiguration,
            ref List<ICompilerPlugin> _compilerPlugins,
            SubmissionInfo.SubmissionInfo submissionInfo
        )
        {
            
            _connection = connection;
            _serverConfiguration = serverConfiguration;
            this._compilerPlugins = _compilerPlugins;
            _submissionInfo = submissionInfo;

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
            var directoryName = _serverConfiguration.temp_path + 
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
                _submissionInfo.CodeLang,
                ref _compilerPlugins
            );

            // Определяем полный путь к файлу
            var fileLocation = RandomGenSourceFileLocation(
                _submissionInfo.SubmissionId.ToString(),
                fileExt
            );
            
            /*
             * Записываем в него исходный код,
             * очищаем буфер и закрываем поток
             * записи.
             */
            File.WriteAllBytes(
                fileLocation,
                _submissionInfo.ProblemCode
            );

            // Устанавливаем его аттрибуты
            File.SetAttributes(
                fileLocation,
                FileAttributes.NotContentIndexed
            );

            // Объявляем экземпляр класса компиляции
            var compiler = new SimplePM_Compiler(
                ref _compilerPlugins,
                _submissionInfo.SubmissionId.ToString(),
                fileLocation,
                _submissionInfo.CodeLang
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
                    `submissionId` = '{_submissionInfo.SubmissionId}' 
                LIMIT 
                    1
                ;
            ";

            new MySqlCommand(queryUpdate, _connection).ExecuteNonQuery();
            
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
                        `submissionId` = '{_submissionInfo.SubmissionId}' 
                    LIMIT 
                        1
                    ;
                ";

                new MySqlCommand(queryUpdate, _connection).ExecuteNonQuery();

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
                    switch (_submissionInfo.TestType)
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
                            `submissionId` = '{_submissionInfo.SubmissionId}' 
                        LIMIT 
                            1
                        ;
                    ";

                    // Создаём команду
                    var cmd = new MySqlCommand(queryUpdate, _connection);

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
