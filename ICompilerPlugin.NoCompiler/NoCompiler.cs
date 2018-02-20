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

        public string PluginName => "NoCompiler";
        public string AuthorName => "Kadirov Yurij";
        public string SupportUri => "https://spm.sirkadirov.com/";

        public CompilerResult StartCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation)
        {

            // Делаем преждевременные выводы
            // прям как некоторые девушки
            // (по крайней мере на данный момент)

            var result = new CompilerResult
            {

                // ошибок нет - но вы держитесь
                HasErrors = false,

                // что дали - то и скинул
                ExeFullname = fileLocation,

                // хз зачем, но надо
                CompilerMessage = Properties.Resources.noCompilerRequired

            };

            // Возвращаем результат фальш-компиляции
            return result;

        }
        
        public bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath)
        {
            try
            {

                // Устанавливаем имя запускаемой программы
                startInfo.FileName = languageConfiguration.runtime_path;
                
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
        
    }

}
