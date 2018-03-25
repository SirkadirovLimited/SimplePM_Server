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
             * Объявляем переменную, которая  будет  хранить
             * ссылку на объект, который хранит конфигурацию
             * безопасности сервера проверки решений.
             */

            dynamic securityConfiguration;

            /*
             * Для  обеспечения  безопасности  выполнения
             * при распараллеливании, требуется временная
             * блокировка доступа к чтению из файла.
             */
            lock (new object())
            {

                /*
                 * Производим     чтение    конфигурационного
                 * файла, после чего произвоим десериализацию
                 * Json данных в удобный для нас формат.
                 */

                securityConfiguration = JsonConvert.DeserializeObject(
                    File.ReadAllText("./config/security.json")
                );

            }

            /*
             * Проверяем, включена  ли  функция  запуска
             * пользовательских программ от имени инного
             * пользователя. Если отключена - выходим.
             */

            if ((string)securityConfiguration.runas.enabled != "true")
                return;

            /*
             * Указываем, что будем запускать процесс
             * от имени другого пользователя.
             */

            proc.StartInfo.Verb = "runas";

            /*
             * Передаём имя пользователя
             */

            proc.StartInfo.UserName = (string)securityConfiguration.runas.username;

            /*
             * Передаём,  что   необходимо
             * вытянуть профайл из реестра
             */

            proc.StartInfo.LoadUserProfile = true;

            /*
             * Передаём пароль пользователя
             */

            // Создаём защищённую строку
            var encPassword = new SecureString();

            // Добавляем данные в защищённую строку
            foreach (var c in (string)securityConfiguration.runas.password)
                encPassword.AppendChar(c);

            // Устанавливаем пароль пользователя
            proc.StartInfo.Password = encPassword;
            
        }

    }

}
