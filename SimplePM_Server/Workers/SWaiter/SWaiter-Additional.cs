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

using System;
using System.IO;

namespace SimplePM_Server.Workers
{
    
    public partial class SWaiter
    {
        
        private string RandomGenSourceFileLocation(string submissionId, string fileExt){

            // Генерируем имя директории
            var directoryName = Path.Combine(
                (string)(SWorker._serverConfiguration.path.temp),
                Guid.NewGuid().ToString(),
                ""
            );

            // Создаём все необходимые каталоги
            Directory.CreateDirectory(directoryName);

            // Устанавливаем аттрибуты для созданной директории
            new DirectoryInfo(directoryName).Attributes = FileAttributes.Normal;

            // Возвращаем результат работы функции
            return Path.Combine(directoryName, "s" + submissionId + fileExt);

        }
        
        public static void SetSourceFileAttributes(string fileLocation)
        {

            try
            {
                
                // Устанавливаем его аттрибуты
                File.SetAttributes(
                    fileLocation,
                    FileAttributes.Temporary | FileAttributes.NotContentIndexed
                );
                
            }
            catch (Exception)
            {
                
                // Обработки исключений не предусмотрено
                
            }

        }
        
    }
    
}