using CommandDotNet.Attributes;

namespace ServerTester
{
    class Tester
    {

        [ApplicationMetadata(
            Description = "Get submissions list",
            Name = "submissions"
        )]
        public void GetSubmissionsList(
            [Option(
                Description = "Class work identificatior"
            )] long classworkId = 0,
            [Option(
                Description = "Programming competition identificatior"
            )] long olympId = 0,
            [Option(
                Description = "Status of the submission (waiting, processing, ready)"
            )] string status = "ready",
            [Option(
                Description = "User identificator"
            )] long userId = -1
        )
        {



        }

        [ApplicationMetadata(
            Description = "Send submission to a server",
            Name = "sendsubm"
        )]
        public void SendSubmission(
            [Argument(
                Name = "problemid",
                Description = "Problem identificatior"
            )] long problemId,

            [Argument(
                Name = "codepath",
                Description = "Submission source code path"
            )] string codePath,
            [Argument(
                Name = "codelang",
                Description = "Submission source code language name"
            )] string codeLang,

            [Argument(
                Name = "type",
                Description = "Submission type (syntax, debug, release)"
            )] string type,

            [Option(
                Description = "User identificator"
            )] long userId = 0,
            [Option(
                Description = "Class work identificator"
            )] long classworkId = 0,
            [Option(
                Description = "Programming competition identificator"
            )] long olympId = 0
        )
        {



        }

        [ApplicationMetadata(
            Description = "Active wait a period of time to the completion of submissions checking",
            Name = "waitsubm"
        )]
        public void WaitComplete(
            [Option(
                Description = "Class work identificatior"
            )] long classworkId = 0,
            [Option(
                Description = "Programming competition identificatior"
            )] long olympId = 0,
            [Option(
                Description = "Status of the submission (waiting, processing, ready)"
            )] string status = "ready",
            [Option(
                Description = "Check timeout (in ms)"
            )] long timeout = 5000
        )
        {



        }

    }
}
