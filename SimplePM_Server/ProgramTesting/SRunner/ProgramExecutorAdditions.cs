/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server is a part of software product "Automated
 * vefification system for programming tasks "SimplePM".
 *
 * Copyright 2018 Yurij Kadirov
 *
 * Source code of the product licensed under the Apache License,
 * Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Visit website for more details: https://spm.sirkadirov.com/
 */

using CompilerPlugin;
using System.Security;
using System.Diagnostics;
using SimplePM_Server.Workers;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public class ProgramExecutorAdditions
    {
        
        public static void SetExecInfoByFileExt(
            ref dynamic languageConfiguration,
            ref ICompilerPlugin _compilerPlugin,
            ref ProcessStartInfo startInfo,
            string filePath,
            string arguments
        )
        {
            
            /*
             * Вызываем метод, отвечающий за установку
             * дополнительных параметров запуска.
             */
            var f = _compilerPlugin.SetRunningMethod(
                ref languageConfiguration,
                ref startInfo,
                filePath
            );
            
            // В случае возникновения ошибок выбрасываем исключение
            if (!f)
                throw new SimplePM_Exceptions.UnknownException("SetRunningMethod() failed!");

            // Обрабатываем аргументы запуска
            if (startInfo.Arguments.Length > 0)
                startInfo.Arguments += " " + arguments;
            else
                startInfo.Arguments = arguments;

        }
        
        public static void SetProcessRunAs(ref Process proc)
        {

            /*
             * Проверяем, включена  ли  функция  запуска
             * пользовательских программ от имени инного
             * пользователя. Если отключена - выходим.
             */

            if ((string)(SWorker._securityConfiguration.runas.enabled) != "true")
                return;

            // Указываем имя пользователя
            proc.StartInfo.UserName = (string)(SWorker._securityConfiguration.runas.username);

            /*
             * Передаём, что загружать пользова-
             * тельский профайл не нужно.
             */

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