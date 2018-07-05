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

namespace ReleaseTestInfo
{
    
    public class ReleaseTestInfo
    {

        public int Id { get; } //!< Уникальный идентификатор теста

        public byte[] InputData { get; } //!< Входные данные для теста
        public byte[] OutputData { get; } //!< Выходные данные для теста

        public long MemoryLimit { get; } //!< Лимит по используемой памяти
        public int ProcessorTimeLimit { get; } //!< Лимит по используемому процессорному времени

        public ReleaseTestInfo(int id, byte[] input, byte[] output, long memoryLimit, int timeLimit)
        {

            /*
             * Инициализируем все необходимые переменные
             */

            Id = id;
            InputData = input;
            OutputData = output;
            MemoryLimit = memoryLimit;
            ProcessorTimeLimit = timeLimit;

        }

    }

}