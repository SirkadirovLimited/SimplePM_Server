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

        /*
         * Энумератор   описывает   возможные
         * виды тестирования пользовательских
         * решений поставленных задач.
         */

        public enum RatingType
        {
            DEFAULT = 0,
            FULL = 0,
            TEST_BY_TEST = 1
        }

        // Уникальный идентификатор задачи
        public int ProblemId { get; set; }

        // Сложность задачи
        public int ProblemDifficulty { get; set; }

        // Тип оценивания решения задачи
        public RatingType ProblemRatingType { get; set; } = RatingType.DEFAULT;

        // Указание, не строгая ли проверка выхода
        public bool AdaptProgramOutput { get; set; } = true;

        /*
         * Информация об авторском решении данной задачи
         */
        public byte[] AuthorSolutionCode { get; set; } = null;
        public string AuthorSolutionCodeLanguage { get; set; } = "unset";

    }

}