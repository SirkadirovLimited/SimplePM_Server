using System;

namespace SimplePM_Server
{
    class SimplePM_Exceptions
    {

        [Serializable]
        public class AuthorSolutionRunningException : Exception
        {

            public AuthorSolutionRunningException() {  }

            public AuthorSolutionRunningException(string message) : base(message) {  }

            public AuthorSolutionRunningException(string message, Exception inner) : base(message, inner) {  }

        }

        [Serializable]
        public class AuthorSolutionNotFoundException : Exception
        {

            public AuthorSolutionNotFoundException() { }

            public AuthorSolutionNotFoundException(string message) : base(message) { }

            public AuthorSolutionNotFoundException(string message, Exception inner) : base(message, inner) { }

        }

        [Serializable]
        public class PluginLoadingException : Exception
        {

            public PluginLoadingException() { }

            public PluginLoadingException(string message) : base(message) { }

            public PluginLoadingException(string message, Exception inner) : base(message, inner) { }

        }

        [Serializable]
        public class OutputLengthLimitException : Exception
        {

            public OutputLengthLimitException() { }

            public OutputLengthLimitException(string message) : base(message) { }

            public OutputLengthLimitException(string message, Exception inner) : base(message, inner) { }

        }

        [Serializable]
        public class UnknownException : Exception
        {

            public UnknownException() { }

            public UnknownException(string message) : base(message) { }

            public UnknownException(string message, Exception inner) : base(message, inner) { }

        }

    }
}
