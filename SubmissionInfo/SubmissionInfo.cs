namespace SubmissionInfo
{

    public class SubmissionInfo
    {

        public int SubmissionId { get; set; }
        public int UserId { get; set; }
        public int ProblemId { get; set; }

        public int ClassworkId { get; set; }
        public int OlympId { get; set; }

        public string CodeLang { get; set; }
        public string TestType { get; set; }
        public string CustomTest { get; set; }

        public byte[] ProblemCode { get; set; }

        public int ProblemDifficulty { get; set; }

        public SubmissionInfo
        (
            int _submissionId,
            int _userId,
            int _problemId,

            int _classworkId,
            int _olympId,

            string _codeLang,
            string _testType,
            string _customTest,

            byte[] _problemCode,

            int _problemDifficulty = 0
        )
        {

            SubmissionId = _submissionId;
            UserId = _userId;
            ProblemId = _problemId;

            ClassworkId = _classworkId;
            OlympId = _olympId;

            CodeLang = _codeLang;
            TestType = _testType;
            CustomTest = _customTest;

            ProblemCode = _problemCode;

            ProblemDifficulty = _problemDifficulty;

        }

    }

}
