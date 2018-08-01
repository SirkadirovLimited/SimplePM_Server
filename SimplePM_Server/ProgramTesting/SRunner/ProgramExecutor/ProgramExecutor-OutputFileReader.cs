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
using System.Text;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        private byte[] ReadOutputData()
        {
            
            // Получаем полный путь к файлу с выходными данными
            var outputFilePath = Path.Combine(
                new FileInfo(_programPath).DirectoryName ?? throw new DirectoryNotFoundException(),
                "output"
            );

            try
            {

                //Ели файл не существует, используем стандартный выходной поток
                if (!File.Exists(outputFilePath))
                    throw new Exception(); // Выбрасываем исключение

                // Осуществляем чтение данных из файла и возвращаем их
                return File.ReadAllBytes(outputFilePath);

            }
            catch
            {
                
                // В случае ошибки или если файл не найден, используем STDOUT
                return Encoding.UTF8.GetBytes(
                    (_adaptOutput)
                        ? _programOutput.TrimEnd('\r', '\n')
                        : _programOutput
                );

            }
            
        }

    }
    
}