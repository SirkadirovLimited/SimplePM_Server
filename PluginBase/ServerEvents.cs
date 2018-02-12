/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
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
        public event EventHandler ServerStarted;
        public event EventHandler ServerShutdown;

        /* Server exception events */
        public event EventHandler<UnhandledExceptionCatchedEventArgs> UnhandledExceptionCatched;

        public class UnhandledExceptionCatchedEventArgs : EventArgs
        {
            public Exception ex;
            public DateTime errorTime;
        }

        /* Submission checking */
        public event EventHandler SubmissionChekingStarted;
        public event EventHandler SubmissionChekingFinished;

        /* User process testing */
        public event EventHandler UserProcessTestingStarted;
        public event EventHandler UserProcessTestingFinished;

        public class UserProcessTestingEventArgs : EventArgs
        {
            public Process userProcess;
        }

    }

}
