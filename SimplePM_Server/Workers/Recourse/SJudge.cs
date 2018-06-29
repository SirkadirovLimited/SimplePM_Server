using JudgePlugin;

namespace SimplePM_Server.Workers.Recourse
{
    
    public static class SJudge
    {
        
        //TODO: Одинаковые функции с SCompiler!
        public static IJudgePlugin GetJudgePluginByName(string judgeName)
        {

            // Производим посик плагина
            foreach (var judgePlugin in SWorker._judgePlugins)
                if (judgePlugin.PluginInformation.Name == judgeName)
                    return judgePlugin;

            // Всё плохо, мы ничего не нашли!
            return null;

        }
        
    }
    
}