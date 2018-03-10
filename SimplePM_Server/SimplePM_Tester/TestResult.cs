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

namespace SimplePM_Server.SimplePM_Tester
{

    /*
     * Класс, который является примитивом
     * единичного     теста     заданного
     * пользовательского  решения  данной
     * задачи по программированию.
     */

    internal class TestResult
    {

        /*
         * Раздел объявления важных констант
         */
        
        public const char MiddleSuccessResult = '*'; // Предварительный успешный результат
        public const char FullSuccessResult = '+'; // Успешный результат
        public const char FullNoSuccessResult = '-'; // Не успешный результат
        public const char TimeLimitResult = 'T'; // Превышен лимит по использованному процессорному времени
        public const char MemoryLimitResult = 'M'; // Превышен лимит по использованной памяти
        public const char RuntimeErrorResult = 'R'; // При выполнении произошла Runtime-ошибка
        public const char ErrorOutputNotNullResult = 'E'; // Поток ошибок не пустой
        public const char InputErrorResult = 'I'; // Ошибка записи входного потока
        public const char OutputErrorResult = 'O'; // Ошибка в формате выходного потока
        public const char ServerErrorResult = 'C'; // Ошибка сервера проверки решений
        
        /*
         * Раздел объявления переменных
         */

        public string ErrorOutput { get; set; } // Выходной поток ошибок пользовательской программы
        public byte[] Output { get; set; } // Выходной поток пользовательской программы

        public int ExitCode { get; set; } // Код выхода пользовательской программы
        public char Result { get; set; } // Результат по текущему тесту

        public int UsedProcessorTime { get; set; } // Использование процессорного времени пользовательской программой
        public long UsedMemory { get; set; } // Использование памяти пользовательской программой
        
        public bool IsSuccessful => (
            Result == '+' && ExitCode == 0
        ); // Указывает на то, пройден ли тест или нет
        
    }

}
