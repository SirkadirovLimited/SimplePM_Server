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
/*! \file */

namespace SimplePM_Server.SimplePM_Tester
{

    /*!
     * \brief
     * Класс, который является примитивом единичного
     * теста заданного пользовательского решения данной
     * задачи по программированию.
     */

    internal class Test
    {

        ///////////////////////////////////////////////////
        /// Раздел объявления используемых переменных
        ///////////////////////////////////////////////////

        public readonly char Result; //!< Результат по текущему тесту

        public readonly int ExitCode; //!< Код выхода пользовательской программы

        public readonly int UsedMemory; //!< Использование памяти пользовательской программой

        public readonly int UsedProcessorTime; //!< Использование процессорного времени пользовательской программой

        public readonly string ErrorOutput; //!< Выходной поток ошибок пользовательской программы

        public bool IsSuccessful => (Result == '+' && ExitCode == 0); //!< Указывает на то, пройден ли тест или нет

        ///////////////////////////////////////////////////
        /// Метод является переопределением стандартного
        /// конструктора текущего класса.
        ///////////////////////////////////////////////////

        public Test(char Result, int ExitCode, int UsedMemory, int UsedProcessorTime, string ErrorOutput)
        {

            this.Result = Result;
            this.ExitCode = ExitCode;
            this.UsedMemory = UsedMemory;
            this.UsedProcessorTime = UsedProcessorTime;
            this.ErrorOutput = ErrorOutput;

        }

        ///////////////////////////////////////////////////

    }

}
