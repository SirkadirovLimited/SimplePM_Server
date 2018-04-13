using System;
using JudgeBase;
using ProgramTesting;

namespace JudgePlugin
{
    public class Judge : IJudgePlugin
    {

        public string JudgeName => "full";

        public string JudgeAuthor => "Yurij Kadirov";

        public string JudgeSupportUrl => "https://spm.sirkadirov.com/";

        public JudgeResult GenerateJudgeResult(ref ProgramTestingResult programTestingResult)
        {

            return new JudgeResult
            {
                RatingMult = Convert.ToInt32(programTestingResult.PassedTestsCount() >= programTestingResult.TestsCount)
            };

        }

    }
}