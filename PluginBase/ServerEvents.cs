using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
