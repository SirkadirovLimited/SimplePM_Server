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

                // Синхронизируем потоки
                lock (new object())
                {

                    // Удаляем каталог временных файлов
                    DeleteDirectory(
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
            
            void DeleteDirectory(string directoryPath, bool recursive)
            {
    
                // Выполняем рекурсивное удаление директорий лишь по требованию
                if (recursive)
                {
                    // Перебираем все папки по указанному пути
                    foreach (var recursivePath in Directory.GetDirectories(directoryPath))
                        DeleteDirectory(recursivePath, true); // Углубляемся на уровень ниже
                }
    
                // Осуществляем перебор всех файлов по указанному пути
                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    // Получаем аттрибуты найденного файла
                    var attr = File.GetAttributes(file);
        
                    // Если файл доступен только для чтения, исправляем это
                    if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        File.SetAttributes(file, attr ^ FileAttributes.ReadOnly);
        
                    // Удаляем найденный файл
                    File.Delete(file);
                }
 
                // Теперь мы попросту удаляем пустую папку
                Directory.Delete(directoryPath);
    
            }

        }
        
    }
    
}