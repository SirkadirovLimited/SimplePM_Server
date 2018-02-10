using System;

namespace ReleaseTestInfo
{
    
    public class ReleaseTest
    {

        public int Id { get; } //!< Уникальный идентификатор теста

        public byte[] InputData { get; } //!< Входные данные для теста
        public byte[] OutputData { get; } //!< Выходные данные для теста

        public long MemoryLimit { get; } //!< Лимит по используемой памяти
        public int ProcessorTimeLimit { get; } //!< Лимит по используемому процессорному времени

        public ReleaseTest(int id, byte[] input, byte[] output, long memoryLimit, int timeLimit)
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
