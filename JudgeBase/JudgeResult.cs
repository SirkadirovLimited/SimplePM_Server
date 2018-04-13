/*
 * Copyright (C) 2018, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

namespace JudgeBase
{

    /*
     * Структура предназначена  для  хранения
     * данных о предоставленном  рейтинге для
     * пользовательского решения поставленной
     * задачи.
     */

    public struct JudgeResult
    {

        /*
         * Предоставленный множитель рейтинга для решения
         */

        public float RatingMult { get; set; }

    }

}
