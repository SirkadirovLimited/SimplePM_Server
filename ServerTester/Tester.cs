/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server
 * A part of SimplePM programming contests management system.
 *
 * Copyright 2017 Yurij Kadirov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Visit website for more details: https://spm.sirkadirov.com/
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
