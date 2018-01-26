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

using System;
using System.Diagnostics;
using System.IO;
// Для использования базовых
// методов и полейсистемы плагинов
using CompilerBase;
// Парсер INI файлов конфигурации
using IniParser.Model;

namespace CompilerPlugin
{
    
    public class Compiler : ICompilerPlugin
    {

        // Поддерживаемый язык программирования
        private const string _progLang = "java";
        // Расширение файла поддерживаемого языка программирования
        private const string _progLangExt = "java";
        // Отображаемое имя
        private const string _displayName = "SimplePM Java Compiler module";
        // Автор модуля
        private const string _author = "Kadirov Yurij";
        // Адрес технической поддержки
        private const string _supportUrl = "https://spm.sirkadirov.com/";

        /* Начало раздела безопасной передачи */
        public string CompilerPluginLanguageName => _progLang;
        public string CompilerPluginLanguageExt => _progLangExt;
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

        public CompilerResult StartCompiler(ref IniData sConfig, ref IniData sCompilersConfig, string submissionId, string fileLocation)
        {

            // Инициализируем объект CompilerRefs
            CompilerRefs cRefs = new CompilerRefs();

            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = cRefs.RunCompiler(
                sCompilersConfig["GCC"]["Path"],
                sCompilersConfig["FreePascal"]["Arguments"] + " " + '"' + fileLocation + '"'
            );
            
            // Для отлавливания всевозможных ошибок
            // создаём их улавливатель.
            // Он также поможет отловить пользовательские
            // ошибки в связи с незнанием правил использования
            // автоматизированной системы проверки решений SimplePM.
            try
            {

                //Получаем информацию о файле исходного кода
                FileInfo fileInfo = new FileInfo(fileLocation);

                //Указываем полный путь к главному исполняемому файлу
                result.ExeFullname = fileInfo.DirectoryName + "\\" + sCompilersConfig["Java"]["DefaultClassName"] + ".class";

                //Проверяем на существование главного класса
                if (!File.Exists(result.ExeFullname))
                    throw new FileNotFoundException();

                //Ошибок не найдено!
                result.HasErrors = false;

            }
            catch (Exception)
            {

                //В случае любой ошибки считаем что она
                //произошла по прямой вине пользователя.
                result.HasErrors = true;

            }

            //Возвращаем результат компиляции
            return cRefs.ReturnCompilerResult(result);

        }

        ///////////////////////////////////////////////////
        /// Метод, который вызывается перед запуском
        /// пользовательского решения поставленной задачи
        /// и выполняет роль выборщика метода запуска
        /// пользовательской программы.
        ///////////////////////////////////////////////////

        public bool SetRunningMethod(ref IniData sConfig, ref IniData sCompilersConfig, ref ProcessStartInfo startInfo, string filePath)
        {
            try
            {
                
                //Получаем информацию о файле
                FileInfo fileInfo = new FileInfo(filePath);

                //Устанавливаем рабочую папку процесса
                startInfo.WorkingDirectory = fileInfo.DirectoryName;

                // Устанавливаем имя запускаемой программы
                startInfo.FileName = sCompilersConfig["Java"]["RuntimePath"];
                
                // Аргументы запуска данной программы
                startInfo.Arguments = "-d64 -cp . " + '"' + Path.GetFileNameWithoutExtension(fileInfo.Name) + '"';

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
