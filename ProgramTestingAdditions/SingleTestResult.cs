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

namespace ProgramTestingAdditions
{

    /*
     * Класс, который является примитивом
     * единичного теста заданного
     * пользовательского решения данной
     * задачи по программированию.
     */

    public class SingleTestResult
    {

        /*
         * Класс содержит все возможные результаты
         * тестирования пользовательского  решения
         * поставленной задачи на едином тесте.
         */

        public static class PossibleResult
        {
            
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
            public const char WaitErrorResult = 'W'; // Слишком длинное выполнение программы
            
        }

        /*
         * Раздел объявления переменных0
         */

        public string ErrorOutput { get; set; } // Выходной поток ошибок пользовательской программы
        public byte[] Output { get; set; } // Выходной поток пользовательской программы

        public int ExitCode { get; set; } // Код выхода пользовательской программы
        public char Result { get; set; } // Результат по текущему тесту

        public int UsedProcessorTime { get; set; } // Использование процессорного времени пользовательской программой
        public long UsedMemory { get; set; } // Использование памяти пользовательской программой

        public bool IsSuccessful => (
            Result == PossibleResult.FullSuccessResult && ExitCode == 0
        ); // Указывает на то, пройден ли тест или нет

    }

}