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

using System.Diagnostics;
using CompilerBase;

namespace CompilerPlugin
{
    
    public class Compiler : ICompilerPlugin
    {

        public string PluginName => "TypicalCompiler";
        public string AuthorName => "Kadirov Yurij";
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
                (string)languageConfiguration.compiler_arguments
            );

            // Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

            // Возвращаем результат компиляции
            return cRefs.ReturnCompilerResult(result);

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {

            // Устанавливаем имя запускаемой программы
            startInfo.FileName = filePath;

            // Устанавливаем аргументы запуска данной программы
            startInfo.Arguments = "";

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
