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

// Для работы с коллекциями

using System;
using System.Collections.Generic;
// Конфигурационный файл
using IniParser.Model;
// Работа с файлами
using System.IO;
// Работа с запросами Linq
using System.Linq;
// Работа с компиляторами
using CompilerBase;

namespace SimplePM_Server
{

    /*
     * Класс компиляции  пользовательских  решений
     * задач по программированию. Функции в классе
     * вызывается функциями класса-официанта.
     */

    internal class SimplePM_Compiler
    {

        private readonly string _submissionId; // Идентификатор запроса
        private readonly string _fileLocation; // Полный путь к файлу и его расширение
        private List<ICompilerPlugin> _compilerPlugins; // Список загруженных модулей компиляторв
        private readonly string _codeLang; // Название языка программирования, на котором написан код
        
        public SimplePM_Compiler(
            ref List<ICompilerPlugin> _compilerPlugins,
            string submissionId,
            string fileLocation,
            string codeLang
        )
        {
            
            // Проверяем путь к исходному коду на ошибки
            if (string.IsNullOrEmpty(fileLocation) || string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
                throw new FileNotFoundException("File not found!", "fileLocation");

            /*
             * Присваиваем глобальным для класса переменным
             * значения локальных  переменных  конструктора
             * класса.
             */
            this._compilerPlugins = _compilerPlugins;
            _submissionId = submissionId;
            _fileLocation = fileLocation;
            _codeLang = codeLang;

        }
        
        /*
         * Функция возвращает объект типа ICompilerPlugin,
         * который  отвечает  за  компиляцию  программ  на
         * указанном   в    параметрах    функции    языке
         * программирования.
         */
        public static ICompilerPlugin GetCompPluginByProgLangName(
            ref List<ICompilerPlugin> _compilerPlugins,
            string programmingLanguage
        )
        {

            /*
             * Производим быстрый поиск по модулям компиляции,
             * и, когда находим - возврщаем  ссылку на объект.
             */
            return (
                from compilerPlugin
                in _compilerPlugins
                where compilerPlugin.CompilerPluginLanguageName == programmingLanguage
                select compilerPlugin
            ).FirstOrDefault();

        }
        
        /*
         * Функция, которая по enum-у выбирает и
         * запускает определённый компилятор, а также
         * возвращает результат компиляции.
         */
        public CompilerResult ChooseCompilerAndRun()
        {

            /*
             * Получаем экземпляр реализации
             * интерфейса модуля компиляции.
             */
            var requestedCompiler = GetCompPluginByProgLangName(
                ref _compilerPlugins,
                _codeLang
            );

            /*
             * Выполняем необходимые действия в
             * случае обнаружения нулевого возврата
             */
            if (requestedCompiler == null)
            {

                return new CompilerResult
                {
                    HasErrors = true, // Хьюстон, у нас проблема!
                    CompilerMessage = "Language not supported by SimplePM!"
                };

            }

            /*
             * Возвращаем ссылку на объект,
             * содержащий результаты компиляции
             */
            return requestedCompiler.StartCompiler(
                _submissionId,
                _fileLocation
            );

        }

        ///////////////////////////////////////////////////

    }
}
