/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server is a part of software product "Automated
 * vefification system for programming tasks "SimplePM".
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