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

// Для работы с процессами
using System.Diagnostics;
// Для использования базовых
// методов и полей системы плагинов
using CompilerBase;
// Парсер INI файлов конфигурации
using IniParser.Model;

namespace FreePascalCompiler
{
    
    public class FreePascalCompilerPlugin : ICompilerPlugin
    {

        // Поддерживаемые языки программирования
        private const string _progLang = "freepascal";
        // Отображаемое имя
        private const string _displayName = "SimplePM Free Pascal & other dialects Compiler module";
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

            // Инициализируем объект CompilerRefs
            CompilerRefs cRefs = new CompilerRefs();

            //Будущее местонахождение исполняемого файла
            string exeLocation = cRefs.GenerateExeFileLocation(fileLocation, submissionId, sConfig["UserProc"]["exeFileExt"]);

            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = cRefs.RunCompiler(
                sConfig["Compilers"]["freepascal_location"],
                sConfig["Compilers"]["freepascal_arguments"] + " \"" + fileLocation + "\""
            );

            //Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

            //Возвращаем результат компиляции
            return cRefs.ReturnCompilerResult(result);

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
                startInfo.FileName = filePath;

                // Аргументы запуска данной программы
                startInfo.Arguments = "";

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
