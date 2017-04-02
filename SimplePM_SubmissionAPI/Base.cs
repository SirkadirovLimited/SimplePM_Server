using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimplePM_SubmissionAPI
{
    class Submission
    {
        public enum SubmissionType
        {
            unset = 0,
            syntax = 2,
            debug = 4,
            release = 8
        }
        public enum SubmissionLanguage
        {
            unset = 0,
            freepascal = 1,
            csharp = 2,
            cpp = 3,
            c = 4,
            python = 5,
            lua = 6
        }
        public enum SubmissionStatus
        {
            unset = 0,
            operating = 1,
            compiling = 2,
            compilationError = 3,
            compiled = 4,
            testing = 5,
            finished = 6
        }

        public static SubmissionInfo getSubmissionInfo(string webApiDomain = "37.57.143.185:80")
        {

        }
    }

    class SubmissionInfo
    {
        public SubmissionInfo(
            int pId,
            int uId,
            Submission.SubmissionType sType,
            string sCode,
            string sCustTest,
            Submission.SubmissionLanguage sLang,
            Submission.SubmissionStatus sStatus,
            string tResults
        )
        {
            this.problemId = pId;
            this.userId = uId;
            this.submissionType = sType;
            this.submissionCode = sCode;
            this.customTest = sCustTest;
            this.submissionLanguage = sLang;
            this.submissionStatus = sStatus;
            this.testResults = tResults;
        }

        public int problemId = 0;
        public int userId = 0;
        public Submission.SubmissionType submissionType = Submission.SubmissionType.unset;
        public string submissionCode = null;
        public string customTest = null;
        public Submission.SubmissionLanguage submissionLanguage = Submission.SubmissionLanguage.unset;
        public Submission.SubmissionStatus submissionStatus = Submission.SubmissionStatus.unset;
        public string testResults = null;
    }
}
