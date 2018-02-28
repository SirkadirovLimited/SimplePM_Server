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

namespace SubmissionInfo
{

    /*
     * Класс,   реализации   которого   являются
     * хранилищами информации о пользовательских
     * запросах на тестирование  решений  данных
     * задач по программированию.
     */
    public class SubmissionInfo
    {

        /*
         * Базовая информация о запросе
         */
        public int SubmissionId { get; set; }
        public int UserId { get; set; }

        /*
         * Информация о принадлежности
         * пользовательской  попытки к
         * указанному   уроку  и / или
         * олимпиаде.
         */
        public int ClassworkId { get; set; }
        public int OlympId { get; set; }
        
        /*
         * Информация о параметрах тестирования
         */
        public string TestType { get; set; }
        public string CustomTest { get; set; }

        /*
         * Информация  об  исходном коде  пользовательского
         * решения поставленной задачи по программированию.
         */
        public string CodeLang { get; set; }
        public byte[] ProblemCode { get; set; }

        /*
         * Дополнительная   информация,   которая
         * помогает обрабатывать пользовательский
         * запрос на тестирование решения.
         */
        public ProblemInfo ProblemInformation;
        
    }

    /*
     * Класс описывает поля и методы,
     * которые относятся к данной задаче,
     * по которой пришёл запрос на тестирование.
     */
    public class ProblemInfo
    {

        // Уникальный идентификатор задачи
        public int ProblemId { get; set; } = 0;

        // Сложность задачи
        public int ProblemDifficulty { get; set; } = 0;

        // Указание, не строгая ли проверка выхода
        public bool AdaptProgramOutput { get; set; } = true;

    }

    /*
     * Класс описывает поля и методы, которые
     * ответственны за хранение и обработку
     * результата выполнения проверки пользовательского
     * решения данной задачи по программированию на
     * правильность и соответствие всем перечисленным
     * требованиям.
     */
    public class SubmissionResult
    {

        /*
         * Выходные потоки компилятора по данному решению
         */
        public string CompilerOutput { get; set; } = string.Empty;

        /*
         * Информация  о  тестах  и  тестировании
         * пользовательского решения поставленной
         * задачи по программированию.
         */
        public int TestsCount { get; set; } = 0;
        public int PassedTestsCount { get; set; } = 0;
        public int FailedTestsCount { get; set; } = 0;

        /*
         * Тип оценивания пользовательского решения
         */
        public RatingType SubmissionRatingType { get; set; } = RatingType.FULL;

        /*
         * Данный  enum  перечисляет  все  виды
         * оценивания пользовательского решения
         * поставленной задачи.
         */
        public enum RatingType
        {
            TEST_BY_TEST = 0,
            FULL = 1
        }

    }

}