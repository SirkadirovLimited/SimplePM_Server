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
        private dynamic _compilerConfigurations;
        private List<ICompilerPlugin> _compilerPlugins; // Список загруженных модулей компиляторв
        
        public SimplePM_Officiant(
            MySqlConnection connection,
            ref dynamic _serverConfiguration,
            ref dynamic _compilerConfigurations,
            ref List<ICompilerPlugin> _compilerPlugins,
            SubmissionInfo.SubmissionInfo submissionInfo
        )
        {
            
            _connection = connection;
            this._serverConfiguration = _serverConfiguration;
            this._compilerConfigurations = _compilerConfigurations;
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
            var directoryName = _serverConfiguration.path.temp + 
                                @"\" + Guid.NewGuid() + 
                                submissionId + @"\";

            // Создаём все необходимые каталоги
            Directory.CreateDirectory(directoryName);

            // Возвращаем результат работы функции
            return directoryName + "s" + submissionId + fileExt;

        }

        /*
         * Метод сопровождает пользовательский
         * запрос на тестирование данного решения
         * поставленной задачи на всех стадиях
         * тестирования, и занимается вызовом
         * всех необходимых для осуществления
         * тестирования пользовательского решения
         * методов.
         */

        public void ServeSubmission()
        {
            
            /*
             * Определяем соответствующую данному запросу
             * на тестирование конфигурацию модуля компиляции
             */

            var compilerConfiguration = SimplePM_Compiler.GetCompilerConfig(
                ref _compilerConfigurations,
                _submissionInfo.CodeLang
            );

            /*
             * Определяем расширение файла
             * исходного кода пользовательского
             * решения поставленной задачи.
             */
            var fileExt = "." + compilerConfiguration.source_ext;

            /*
             * Случайным образом генерируем путь
             * к файлу исходного кода пользовательского
             * решения поставленной задачи.
             */
            var fileLocation = RandomGenSourceFileLocation(
                _submissionInfo.SubmissionId.ToString(),
                fileExt
            );
            
            /*
             * Записываем в него исходный код,
             * очищаем буфер и закрываем поток
             * записи в данный файл.
             * При этом, осуществляем побайтовую
             * запись в файл, дабы не повредить его.
             */
            File.WriteAllBytes(
                fileLocation,
                _submissionInfo.ProblemCode
            );

            /*
             * Устанавливаем аттрибуты данного файла
             * таким образом, дабы исключить возможность
             * индексирования его содержимого и остальные
             * не приятные для нас ситуации, которые могут
             * привести к непредвиденным последствиям.
             */
            File.SetAttributes(
                fileLocation,
                FileAttributes.NotContentIndexed
            );

            /*
             * Объявляем и нициализируем переменную,
             * в которой будет храниться ссылка на
             * объект, методы которого осуществляют
             * все этапы компиляции пользовательского
             * решения задачи.
             */
            var compiler = new SimplePM_Compiler(
                ref _compilerConfigurations111,
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
                        default:

                            logger.Error(
                                "Unsupported test type at submission #" + _submissionInfo.SubmissionId
                            );

                            break;
                    }
                    
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
                    ClearCache(fileLocation);

                    // Выходим
                    return;

                }

            }

            /*
             * Вызываем метод,  который несёт
             * ответственность  за   удаление
             * всех временных файлов запросов
             * на  тестирование,  а  также за
             * вызов сборщика мусора.
             */
            ClearCache(fileLocation);

        }

        /*
         *
         */
        public void ClearCache(string fileLocation)
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
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

            }
            catch (Exception) { /* Deal with it. */ }

        }
        
        ///////////////////////////////////////////////////
        
    }
}
