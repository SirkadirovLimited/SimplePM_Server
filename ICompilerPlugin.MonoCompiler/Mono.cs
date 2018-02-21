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

using System;
using System.Diagnostics;
using CompilerBase;

namespace CompilerPlugin
{
    
    public class Compiler : ICompilerPlugin
    {

        public string PluginName => "Mono";
        public string AuthorName => "Yurij Kadirov";
        public string SupportUri => "https://spm.sirkadirov.com/";

        public CompilerResult StartCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation)
        {

            // Инициализируем объект CompilerRefs
            var cRefs = new CompilerRefs();

            // Будущее местонахождение исполняемого файла
            var exeLocation = cRefs.GenerateExeFileLocation(
                fileLocation,
                submissionId
            );

            // Запуск компилятора с заранее определёнными аргументами
            var result = cRefs.RunCompiler(
                languageConfiguration.compiler_path,
                languageConfiguration.compiler_arguments
            );

            // Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

            // Возвращаем результат компиляции
            return cRefs.ReturnCompilerResult(result);

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {
            try
            {
                
                // Получаем информацию о платформе запуска
                var platform = (int)Environment.OSVersion.Platform;
                
                /*
                 * В зависимости от установленной на машине
                 * операционной системы, выполняем специфи-
                 * ческие действия для указания верного ме-
                 * тода  запуска  пользовательского решения
                 * поставленной задачи.
                 */
                if (platform == 4 || platform == 6 || platform == 128)
                {

                    // Указываем имя запускаемой программы (полный путь к ней)
                    startInfo.FileName = languageConfiguration.runtime_path;

                    // Указываем аргументы запуска
                    startInfo.Arguments = '"' + filePath + '"';

                }
                else
                {

                    // Указываем имя запускаемой программы (полный путь к ней)
                    startInfo.FileName = filePath;

                    // Указываем аргументы запуска
                    startInfo.Arguments = "";

                }

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
        
    }

}
