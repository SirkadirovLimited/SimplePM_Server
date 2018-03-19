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

using ProgramTesting;

namespace JudgeBase
{

    /*
     * Интерфейс описывает структуру
     * типичного плагина-судьи.
     */

    public interface IJudgePlugin
    {

        /*
         * Наименование плагина-судьи
         */

        string JudgeName { get; }

        /*
         * Информация об авторе плагина-судьи
         */

        string JudgeAuthor { get; }

        /*
         * Url-адрес технической поддержки
         * для текущего плагина-судьи.
         */

        string JudgeSupportUrl { get; }

        /*
         * Метод, который несёт ответственность
         * за  вынесение  вердикта  о  рейтинге
         * данного решения поставленной задачи.
         */

        JudgeResult GenerateJudgeResult(ref ProgramTestingResult programTestingResult);

    }

}
