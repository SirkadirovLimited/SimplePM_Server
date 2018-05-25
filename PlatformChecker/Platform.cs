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
