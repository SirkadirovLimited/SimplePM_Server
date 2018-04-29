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

using System;
using System.Diagnostics;

namespace PluginBase
{

    public class ServerEvents
    {

        /* Basic server events */
        public event EventHandler<DateTime> ServerStarted;
        public event EventHandler<DateTime> ServerShutdown;

        /* Server exception events */
        public event EventHandler<Exception> UnhandledExceptionCatched;

        /* Submission checking */
        public event EventHandler<SubmissionInfo.SubmissionInfo> SubmissionChekingStarted;
        public event EventHandler<SubmissionInfo.SubmissionInfo> SubmissionChekingFinished;

        /* User process testing */
        public event EventHandler<Process> UserProcessTestingStarted;
        public event EventHandler<Process> UserProcessTestingFinished;

    }

}
