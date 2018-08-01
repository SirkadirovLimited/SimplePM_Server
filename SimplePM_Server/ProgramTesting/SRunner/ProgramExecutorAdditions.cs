/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 * 
 * SimplePM Server is a part of software product "Automated
 * verification system for programming tasks "SimplePM".
 * 
 * Copyright (C) 2016-2018 Yurij Kadirov
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 * 
 * GNU Affero General Public License applied only to source code of
 * this program. More licensing information hosted on project's website.
 * 
 * Visit website for more details: https://spm.sirkadirov.com/
 */

using CompilerPlugin;
using System.Security;
using System.Diagnostics;
using SimplePM_Server.Workers;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public static class ProgramExecutorAdditions
    {
        
        public static void SetExecInfoByFileExt(
            ref dynamic languageConfiguration,
            ref ICompilerPlugin _compilerPlugin,
            ref ProcessStartInfo startInfo,
            string filePath,
            string arguments
        )
        {
            
            // Устанавливаем дополнительные параметры запуска
            var f = _compilerPlugin.SetRunningMethod(
                ref languageConfiguration,
                ref startInfo,
                filePath
            );
            
            // В случае возникновения ошибок выбрасываем исключение
            if (!f)
                throw new ServerExceptions.UnknownException("SetRunningMethod() failed!");

            // Обрабатываем аргументы запуска
            if (startInfo.Arguments.Length > 0)
                startInfo.Arguments += " " + arguments;
            else
                startInfo.Arguments = arguments;

        }
        
        public static void SetProcessRunAs(ref Process proc)
        {

            // Проверка на активизацию функции
            if ((string)(SWorker._securityConfiguration.runas.enabled) != "true")
                return;

            // Указываем имя пользователя
            proc.StartInfo.UserName = (string)(SWorker._securityConfiguration.runas.username);

            // Передаём, что загружать пользовательский профайл не нужно
            proc.StartInfo.LoadUserProfile = false;

            /*
             * Передаём пароль пользователя
             */

            // Создаём защищённую строку
            var encPassword = new SecureString();

            // Добавляем данные в защищённую строку
            foreach (var c in (string)(SWorker._securityConfiguration.runas.password))
                encPassword.AppendChar(c);

            // Устанавливаем пароль пользователя
            proc.StartInfo.Password = encPassword;
            
        }
        
    }
    
}