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