using Plugin;
using ProgramTestingAdditions;

namespace JudgePlugin
{
    
    public class Judge : IJudgePlugin
    {
        
        public PluginInfo PluginInformation => new PluginInfo(
            "none",
            "Yurij Kadirov (Sirkadirov)",
            "https://spm.sirkadirov.com/"
        );

        public JudgingResult GenerateJudgeResult(ref ProgramTestingResult programTestingResult)
        {

            return new JudgingResult
            {
                RatingMult = 0
            };

        }

    }
    
}