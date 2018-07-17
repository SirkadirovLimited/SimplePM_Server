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
        
        public static void ClearCache(string fileLocation)
        {

            try
            {

                lock (new object())
                {

                    // Обновляем аттрибуты для всех файлов и папок по указанному пути
                    UpdateFileAttributes(
                        new DirectoryInfo(
                            new FileInfo(fileLocation).DirectoryName ?? throw new DirectoryNotFoundException()
                        )
                    );
                    
                    // Удаляем каталог временных файлов
                    Directory.Delete(
                        new FileInfo(fileLocation).DirectoryName
                        ?? throw new DirectoryNotFoundException(),
                        true
                    );

                    // Вызываем сборщик мусора
                    GC.Collect(
                        GC.MaxGeneration,
                        GCCollectionMode.Forced
                    );

                }

            }
            catch (Exception ex)
            {

                // Записываем исключение в лог-файл
                logger.Warn("Cache clearing failed: " + ex);

            }
            
            void UpdateFileAttributes(DirectoryInfo dInfo)
            {
                
                // Set Directory attribute
                dInfo.Attributes = FileAttributes.Directory | FileAttributes.Normal;

                foreach (var file in dInfo.GetFiles())
                    file.Attributes = FileAttributes.Normal;

                // recurse all of the subdirectories
                foreach (var subDir in dInfo.GetDirectories())
                    UpdateFileAttributes(subDir);
                
            }

        }
        
    }
    
}