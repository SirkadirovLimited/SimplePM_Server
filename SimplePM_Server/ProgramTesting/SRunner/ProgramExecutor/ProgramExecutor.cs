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

using CompilerPlugin;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        public ProgramExecutor(
            dynamic compilerConfiguration,
            ICompilerPlugin _compilerPlugin,
            string path,
            string args,
            long memoryLimit = 0,
            int processorTimeLimit = 0,
            byte[] input = null,
            int outputCharsLimit = 0,
            bool adaptOutput = true,
            int programRuntimeLimit = 0
        )
        {
            
            logger.Trace("ProgramExecutor for <" + path + "> created...");
            
            // Получаем конфигурации компиляторов
            _compilerConfiguration = compilerConfiguration;

            // Получаем список модулей компиляции
            this._compilerPlugin = _compilerPlugin;

            // Получаем полный путь к программе
            _programPath = path;

            // Получаем аргументы запуска программы
            _programArguments = args;

            // Получаем лимит по памяти
            _programMemoryLimit = memoryLimit;

            // Получаем лимит по процессорному времени
            _programProcessorTimeLimit = processorTimeLimit;

            // Получаем данные для входного потока
            _programInputBytes = input;

            // Получаем лимит по количеству данных в выходном потоке
            _outputCharsLimit = outputCharsLimit;

            // Узнаём, необходимо ли упрощать выходной поток
            _adaptOutput = adaptOutput;

            // Получаем лимит на время исполнения программы в миллисекундах
            _programRuntimeLimit = (programRuntimeLimit <= 0) ? processorTimeLimit * 5 : programRuntimeLimit;

        }

    }
    
}