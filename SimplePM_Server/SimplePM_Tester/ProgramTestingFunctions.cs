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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using CompilerBase;
using IniParser.Model;
using Newtonsoft.Json;

namespace SimplePM_Server.SimplePM_Tester
{

    /*
     * Класс,  содержащий методы,  необходимые
     * для проведения корректного тестирования
     * пользовательских   решений   задач   по
     * программированию.
     */
    
    internal class ProgramTestingFunctions
    {
        
        /*
         * Функция определяет необходимые действия
         * при запуске  процесса пользовательского
         * или авторского решения задачи.
         */
        public static void SetExecInfoByFileExt(
            ref IniData sCompilersConfig,
            ref List<ICompilerPlugin> _compilerPlugins,
            ref ProcessStartInfo startInfo,
            string filePath,
            string arguments,
            string codeLanguage
        )
        {
            
            /*
             * Вызываем ассоциированный метод,
             * который  знает  лучше, как  это
             * делать.
             */
            var f = SimplePM_Compiler.GetCompPluginByProgLangName(
                ref _compilerPlugins,
                codeLanguage
            ).SetRunningMethod(
                ref sCompilersConfig,
                ref startInfo,
                filePath
            );

            /*
             * Добавляем   к   этому  всему
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
             * Считываем параметры безопасности сервера
             * из конфигурационного файла
             */
            dynamic securityConfiguration = JsonConvert.DeserializeObject(
                File.ReadAllText("./config/security.config")
            );

            /*
             * Проверяем, включена  ли  функция  запуска
             * пользовательских программ от имени инного
             * пользователя. Если отключена - выходим.
             */
            if (securityConfiguration.runas.enabled != true)
                return;

            /*
             * Указываем, что будем запускать процесс
             * от имени другого пользователя.
             */
            proc.StartInfo.Verb = "runas";

            /*
             * Передаём имя пользователя
             */
            proc.StartInfo.UserName = securityConfiguration.runas.username;

            /*
             * Передаём,  что   необходимо
             * вытянуть профайл из реестра
             */
            proc.StartInfo.LoadUserProfile = false;

            /*
             * Передаём пароль пользователя
             */

            // Создаём защищённую строку
            var encPassword = new SecureString();

            // Добавляем данные в защищённую строку
            foreach (var c in securityConfiguration.runas.password)
                encPassword.AppendChar(c);

            // Устанавливаем пароль пользователя
            proc.StartInfo.Password = encPassword;
            
        }

    }

}
