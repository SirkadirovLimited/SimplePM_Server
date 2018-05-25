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

using JudgeBase;

namespace SimplePM_Server
{

    internal class SimplePM_Judge
    {

        /*
         * Метод осуществляет поиск
         * модуля оценивания пользо
         * вательского решения пост
         * авленной задачи  по  его
         * названию.
         */

        public IJudgePlugin GetJudgePluginByName(string judgeName)
        {

            /*
             * В цикле производим поиск
             * по списку плагинов оцени
             * вания  пользовательского
             * решения.
             */

            foreach (IJudgePlugin judgePlugin in SimplePM_Worker._judgePlugins)
            {

                // Если мы нашли то, что искали, то это хорошо
                if (judgePlugin.JudgeName == judgeName)
                    return judgePlugin;

            }

            // Всё плохо, мы ничего не нашли!
            return null;

        }

    }

}
