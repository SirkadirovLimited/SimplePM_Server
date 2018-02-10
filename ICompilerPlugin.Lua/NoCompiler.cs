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

// Для работы с процессами

using System;
using System.Diagnostics;
// Для использования базовых
// методов и полей системы плагинов
using CompilerBase;
// Парсер INI файлов конфигурации
using IniParser.Model;

namespace CompilerPlugin
{
    
    public class Compiler : ICompilerPlugin
    {
        
        /* Начало раздела безопасной передачи */
        public string CompilerPluginLanguageName => "lua";
        public string CompilerPluginLanguageExt => "lua";
        public string CompilerPluginDisplayName => "SimplePM Lua compiler module";
        public string CompilerPluginAuthor => "Kadirov Yurij";
        public string CompilerPluginSupportUrl => "https://spm.sirkadirov.com/";
        /* Конец раздела безопасной передачи */

        ///////////////////////////////////////////////////
        /// Метод, который занимается запуском компилятора
        /// для данного пользовательского решения
        /// поставленной задачи, а также обработкой
        /// результата компиляции данной программы.
        ///////////////////////////////////////////////////

        public CompilerResult StartCompiler(ref IniData sConfig, ref IniData sCompilersConfig, string submissionId, string fileLocation)
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

        public bool SetRunningMethod(ref IniData sConfig, ref IniData sCompilersConfig, ref ProcessStartInfo startInfo, string filePath)
        {
            try
            {

                // Устанавливаем имя запускаемой программы
                startInfo.FileName = sCompilersConfig["Lua"]["Path"];
                
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
