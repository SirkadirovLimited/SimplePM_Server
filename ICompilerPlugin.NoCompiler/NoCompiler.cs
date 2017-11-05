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

// База для всего
using System;
// Для работы с файловой системой
using System.IO;
// Для работы с процессами
using System.Diagnostics;
// Для использования базовых
// методов и полейсистемы плагинов
using CompilerBase;
// Парсер INI файлов конфигурации
using IniParser.Model;

namespace NoCompiler
{
    
    public class NoCompilerPlugin : ICompilerPlugin
    {

        // Поддерживаемые языки программирования
        private const string _progLang = "lua,php,python,ruby";
        // Отображаемое имя
        private const string _displayName = "SimplePM Lua";
        // Автор модуля
        private const string _author = "Kadirov Yurij";
        // Адрес технической поддержки
        private const string _supportUrl = "https://spm.sirkadirov.com/";

        /* Начало раздела безопасной передачи */
        public string CompilerPluginLanguageName => _progLang;
        public string CompilerPluginDisplayName => _displayName;
        public string CompilerPluginAuthor => _author;
        public string CompilerPluginSupportUrl => _supportUrl;
        /* Конец раздела безопасной передачи */

        ///////////////////////////////////////////////////
        /// Метод, который занимается запуском компилятора
        /// для данного пользовательского решения
        /// поставленной задачи, а также обработкой
        /// результата компиляции данной программы.
        ///////////////////////////////////////////////////

        public CompilerResult StartCompiler(ref IniData sConfig, string submissionId, string fileLocation)
        {

            //Делаем преждевременные выводы
            //прям как некоторые девушки
            //(по крайней мере на данный момент)

            CompilerResult result = new CompilerResult()
            {

                //ошибок нет - но вы держитесь
                HasErrors = false,

                //что дали - то и скинул
                ExeFullname = fileLocation,

                //хз зачем, но надо
                CompilerMessage = Properties.Resources.noCompilerRequired

            };

            //Возвращаем результат фальш-компиляции
            return result;

        }

        ///////////////////////////////////////////////////
        /// Метод, который вызывается перед запуском
        /// пользовательского решения поставленной задачи
        /// и выполняет роль выборщика метода запуска
        /// пользовательской программы.
        ///////////////////////////////////////////////////

        public bool SetRunningMethod(ref IniData sConfig, ref ProcessStartInfo startInfo, string filePath)
        {
            try
            {

                // Устанавливаем имя запускаемой программы
                startInfo.FileName = sConfig["Compiler"][new FileInfo(filePath).Extension.ToLower().Replace(".", "")];

                // Аргументы запуска данной программы
                startInfo.Arguments = '"' + filePath + '"';

            }
            catch
            {

                // В случае ошибки указываем, что работа
                // была выполнена не успешно.
                return false;

            }
            
            // Возвращаем родителю информацию о том,
            // что запашиваемая операция была выполнена
            // самым успешным образом.
            return true;

        }

        ///////////////////////////////////////////////////

    }
}
