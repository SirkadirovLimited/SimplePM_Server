/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server
 * A part of SimplePM programming contests management system.
 *
 * Copyright 2017 Yurij Kadirov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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

using System.IO;
using CompilerBase;
using System.Security;
using Newtonsoft.Json;
using System.Diagnostics;

namespace SimplePM_Server.SimplePM_Tester
{

    /*
     * Класс,  содержащий методы,  необходимые
     * для проведения корректного тестирования
     * пользовательских   решений   задач   по
     * программированию.
     */
    
    internal static class ProgramTestingFunctions
    {
        
        /*
         * Функция определяет необходимые действия
         * при запуске  процесса пользовательского
         * или авторского решения задачи.
         */

        public static void SetExecInfoByFileExt(
            ref dynamic languageConfiguration,
            ref ICompilerPlugin _compilerPlugin,
            ref ProcessStartInfo startInfo,
            string filePath,
            string arguments
        )
        {
            
            /*
             * Вызываем ассоциированный метод,
             * который  знает  лучше, как  это
             * делать.
             */
            
            var f = _compilerPlugin.SetRunningMethod(
                ref languageConfiguration,
                ref startInfo,
                filePath
            );

            /*
             * Добавляем к этому всему
             * аргументы коммандной строки.
             */
            
            if (startInfo.Arguments.Length > 0)
                startInfo.Arguments += " " + arguments;
            else
                startInfo.Arguments = arguments;

            /*
             * В случае возникновения непредвиденных
             * ошибок выбрасываем исключение.
             */
            
            if (!f)
                throw new SimplePM_Exceptions.UnknownException("SetRunningMethod() failed!");

        }
        
        /*
         * Функция в зависимости от конфигурации сервера
         * указывает объекту  процесса, что  инициатором
         * его  запуска  должен   являться  либо  другой
         * пользователь, либо тот же,  от имени которого
         * запущен сервер проверки решений задач.
         */

        public static void SetProcessRunAs(ref Process proc)
        {

            /*
             * Проверяем, включена  ли  функция  запуска
             * пользовательских программ от имени инного
             * пользователя. Если отключена - выходим.
             */

            if ((string)(SimplePM_Worker._securityConfiguration.runas.enabled) != "true")
                return;

            /*
             * Передаём имя пользователя
             */
            
            proc.StartInfo.UserName = (string)(SimplePM_Worker._securityConfiguration.runas.username);

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
            foreach (var c in (string)(SimplePM_Worker._securityConfiguration.runas.password))
                encPassword.AppendChar(c);

            // Устанавливаем пароль пользователя
            proc.StartInfo.Password = encPassword;
            
        }

    }

}
