/*
 * Copyright (C) 2018, Yurij Kadirov.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Yurij Kadirov
 * @Website: https://spm.sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

using CommandDotNet.Attributes;

namespace ServerTester
{

    public class Tester
    {

        /*
         * Получение списка запросов на тестирование
         */

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

        /*
         * Отправка запроса(ов) на тестирование
         */

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
            )] long olympId = 0,
            [Option(
                Description = "Programming competition identificator"
            )] long submCount = 1
        )
        {



        }

        /*
         * Ожидание завершения тестирования
         */

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
