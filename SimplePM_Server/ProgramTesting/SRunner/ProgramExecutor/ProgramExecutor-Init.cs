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

using System.IO;
using System.Diagnostics;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        private void Init()
        {

            logger.Trace("ProgramExecutor for <" + _programPath + ">: Init() [started]");
            
            /*
             * Инициализация необходимых для тестирования переменных
             */
            
            _programProcess = new Process
            {

                StartInfo = programStartInfo, // устанавливаем информацию о программе
                EnableRaisingEvents = true, // указываем, что хотим обрабатывать события

            };

            // Устанавливаем рабочую директорию для пользовательской программы
            _programProcess.StartInfo.WorkingDirectory =
                new FileInfo(_programPath).DirectoryName ?? throw new DirectoryNotFoundException();

            /*
             * Управление методом запуска пользовательского процесса
             */

            // Устанавливаем служебную информацию о типе запуска
            ProgramExecutorAdditions.SetExecInfoByFileExt(
                ref _compilerConfiguration,
                ref _compilerPlugin,
                ref programStartInfo,
                _programPath,
                _programArguments
            );

            // Устанавливаем информацию для запуска от имени иного пользователя
            ProgramExecutorAdditions.SetProcessRunAs(
                ref _programProcess
            );
            
            /*
             * Добавляем обработчики для некоторых событий
             */
            
            // Добавляем обработчик события записи данных в выходной поток
            _programProcess.OutputDataReceived += ProgramProcess_OutputDataReceived;
            
            logger.Trace("ProgramExecutor for <" + _programPath + ">: Init() [finished]");

        }
        
    }
    
}