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
