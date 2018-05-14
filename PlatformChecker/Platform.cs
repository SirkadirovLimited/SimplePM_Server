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

namespace PlatformChecker
{

    /// <summary>
    /// Данный класс содержит необходимый
    /// инструментарий для определения
    /// используемой пользователем
    /// операционной системы.
    /// </summary>

    public class Platform
    {

        /// <summary>
        /// Проверка на Windows-подорбную платформу
        /// </summary>

        public static bool IsWindows => !IsLovelyLinux && !IsUglyMac;

        /// <summary>
        /// Проверка на GNU/Linux подобную платформу
        /// </summary>

        public static bool IsLovelyLinux
        {

            get
            {

                int platform = (int)Environment.OSVersion.Platform;

                return (platform == 4) || (platform == 128);

            }

        }

        /// <summary>
        /// Проверка на надкушенное яблоко
        /// </summary>

        public static bool IsUglyMac => (int)Environment.OSVersion.Platform == 6;

    }

}
