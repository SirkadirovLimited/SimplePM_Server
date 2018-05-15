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
