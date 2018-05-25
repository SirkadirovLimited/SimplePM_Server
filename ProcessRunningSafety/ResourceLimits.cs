/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server
 * A part of SimplePM programming contests management system.
 *
 * Copyright 2017 Yurij Kadirov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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
using System.Diagnostics;

namespace ProcessRunningSafety
{

    /// <summary>
    /// Данный класс предоставляет функционал для
    /// ограничения ресурсов указанного процесса,
    /// в зависимости от используемой операционной
    /// системы выполняя соответствующие системные
    /// вызовы и прочие полезные штуки.
    /// </summary>

    public partial class ResourceLimits
    {

        /// <summary>
        /// Идентификатор связанного процесса
        /// </summary>

        public int ProcessId { get; }

        /// <summary>
        /// Основной конструктор класса ResourceLimits
        /// </summary>
        /// <param name="processId">Идентификатор связанного процесса</param>
        
        public ResourceLimits(int processId)
        {

            /*
             * Проверка на существование процесса
             * с указанным идентификатором.
             */

            try
            {

                // Если всё плохо - выбросит исключение
                Process.GetProcessById(processId);

            }
            catch (Exception ex)
            {

                // Идентификатор не верен, выбрасываем исключение
                throw new ArgumentException(
                    "Process associated with ID " + processId + " not found!",
                    nameof(processId),
                    ex
                );

            }

            // Сохраняем полученный идентификатор
            ProcessId = processId;

        }

    }

}
