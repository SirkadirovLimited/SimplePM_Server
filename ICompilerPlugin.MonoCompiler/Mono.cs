/*
 * Copyright (C) 2018, Yurij Kadirov.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Yurij Kadirov
 * @Website: https://spm.sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

using System;
using CompilerBase;
using PlatformChecker;
using System.Diagnostics;

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
                (string)languageConfiguration.compiler_path,
                ((string)languageConfiguration.compiler_arguments).Replace("{%fileLocation%}", fileLocation)
            );

            // Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

            // Фикс для GNU/Linux-based систем
            if (!Platform.IsWindows)
                result.ExeFullname += ".exe";

            // Возвращаем результат компиляции
            return cRefs.ReturnCompilerResult(result);

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {

            /*
             * Выполняем  все  необходимые действия
             * в  блоке  обработки  исключений  для
             * исключения возможности возникновения
             * непредвиденных исключений.
             */

            try
            {
                
                /*
                 * В зависимости от установленной на машине
                 * операционной системы, выполняем специфи-
                 * ческие действия для указания верного ме-
                 * тода  запуска  пользовательского решения
                 * поставленной задачи.
                 */

                if (Platform.IsLovelyLinux || Platform.IsUglyMac)
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

                /*
                 * В случае возникновения каких-либо
                 * ошибок  сигнализируем  об этом  с
                 * помощью return false.
                 */

                return false;

            }

            /*
             * Возвращаем родителю информацию о
             * том, что  запашиваемая  операция
             * была  выполнена  самым  успешным
             * образом.
             */

            return true;

        }
        
    }

}
