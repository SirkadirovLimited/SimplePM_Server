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

using CompilerPlugin;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        public ProgramExecutor(
            ref dynamic compilerConfiguration,
            ref ICompilerPlugin _compilerPlugin,
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