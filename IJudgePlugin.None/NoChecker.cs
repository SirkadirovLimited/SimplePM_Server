using System;
using JudgeBase;
using ProgramTesting;

namespace JudgePlugin
{
    public class Judge : IJudgePlugin
    {

        public string JudgeName => "none";

        public string JudgeAuthor => "Yurij Kadirov";

        public string JudgeSupportUrl => "https://spm.sirkadirov.com/";

        public JudgeResult GenerateJudgeResult(ref ProgramTestingResult programTestingResult)
        {

            return new JudgeResult
            {
                RatingMult = 0
            };

        }

    }
}