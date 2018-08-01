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