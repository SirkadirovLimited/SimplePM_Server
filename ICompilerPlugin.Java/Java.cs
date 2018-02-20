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
using System.IO;
using CompilerBase;

namespace CompilerPlugin
{
    
    public class Compiler : ICompilerPlugin
    {

        public string PluginName => "Java";
        public string AuthorName => "Kadirov Yurij";
        public string SupportUri => "https://spm.sirkadirov.com/";

        public CompilerResult StartCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation)
        {

            // Инициализируем объект CompilerRefs
            var cRefs = new CompilerRefs();

            //Запуск компилятора с заранее определёнными аргументами
            var result = cRefs.RunCompiler(
                languageConfiguration.compiler_path,
                languageConfiguration.compiler_arguments
            );
            
            try
            {

                // Получаем информацию о файле исходного кода
                var fileInfo = new FileInfo(fileLocation);

                // Указываем полный путь к главному исполняемому файлу
                result.ExeFullname = fileInfo.DirectoryName + "\\" + languageConfiguration.default_class_name + ".class";

                // Проверяем на существование главного класса
                if (!File.Exists(result.ExeFullname))
                    throw new FileNotFoundException();

                // Ошибок не найдено!
                result.HasErrors = false;

            }
            catch (Exception)
            {

                // В случае любой ошибки считаем что она
                // произошла по прямой вине пользователя.
                result.HasErrors = true;

            }

            // Возвращаем результат компиляции
            return cRefs.ReturnCompilerResult(result);

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {
            
            try
            {
                
                // Получаем информацию о файле
                var fileInfo = new FileInfo(filePath);

                // Устанавливаем рабочую папку процесса
                startInfo.WorkingDirectory = fileInfo.DirectoryName;

                // Устанавливаем имя запускаемой программы
                startInfo.FileName = languageConfiguration.runtime_path;
                
                // Аргументы запуска данной программы
                startInfo.Arguments = "-d64 -cp . " + '"' +
                                      Path.GetFileNameWithoutExtension(fileInfo.Name) + '"';

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
