/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server
 * A part of SimplePM programming contests management system.
 *
 * Copyright 2017 Yurij Kadirov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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
