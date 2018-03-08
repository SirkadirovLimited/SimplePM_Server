/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
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
