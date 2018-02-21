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

using System.Collections.Generic;
using System.IO;
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
        private dynamic _compilerConfigs;
        private List<ICompilerPlugin> _compilerPlugins; // Список загруженных модулей компиляторв
        private readonly string _codeLang; // Название языка программирования, на котором написан код
        
        public SimplePM_Compiler(
            ref dynamic _compilerConfigs,
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
            this._compilerConfigs = _compilerConfigs;
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
            ref dynamic _compilerConfigs,
            ref List<ICompilerPlugin> _compilerPlugins,
            string programmingLanguage
        )
        {

            /*
             * Производим перебор всех конфигураций компиляторов
             */
            foreach (var compilerConfig in _compilerConfigs)
            {

                /*
                 * Если конфигурация не та - продолжаем искать далее
                 */
                if (compilerConfig.language_name != programmingLanguage)
                    continue;

                /*
                 * Если верная конфигурация найдена,
                 * теперь  нашей   задачей  является
                 * нахождение   модуля   компиляции,
                 * ответственного   за    компиляцию
                 * пользовательских    программ   на
                 * указанном языке программирования.
                 */
                foreach (var compilerPlugin in _compilerPlugins)
                {

                    /*
                     * Если соответствующий плагин найден,
                     * возвращаем ссылку  на  него, в ином
                     * случае продолжаем поиск.
                     */
                    if (compilerPlugin.PluginName == compilerConfig.module_name)
                        return compilerPlugin;

                }

            }

            /*
             * Если соответствующий модуль компиляции
             * не найден, возвращаем значение null.
             */
            return null;

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
                ref _compilerConfigs,
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
             * Возвращаем   ссылку  на  объект,
             * содержащий результаты компиляции
             * пользовательского        решения
             * поставленной задачи.
             */
            return requestedCompiler.StartCompiler(
                ref _compilerConfigs,
                _submissionId,
                _fileLocation
            );

        }

        ///////////////////////////////////////////////////

    }
}
