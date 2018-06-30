using MySql.Data.MySqlClient;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting
{
    
    internal abstract class TestingType
    {

        private MySqlConnection conn;
        private string exeFilePath;
        private SubmissionInfo.SubmissionInfo submissionInfo;
        
        
        public TestingType(
            ref MySqlConnection conn,
            string exeFilePath,
            ref SubmissionInfo.SubmissionInfo submissionInfo
        )
        {

            this.conn = conn;
            this.exeFilePath = exeFilePath;
            this.submissionInfo = submissionInfo;

        }

        public abstract ProgramTestingResult RunTesting();

    }
    
}