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

namespace ProgramTestingAdditions
{

    /*
     * Класс, который является примитивом
     * единичного     теста     заданного
     * пользовательского  решения  данной
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
         * Раздел объявления переменных
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