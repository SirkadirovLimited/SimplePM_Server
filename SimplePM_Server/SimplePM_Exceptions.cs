using System;

namespace SimplePM_Server
{
    class SimplePM_Exceptions
    {

        public class AuthorSolutionRunningException : Exception
        {

            public AuthorSolutionRunningException() {  }

            public AuthorSolutionRunningException(string message) : base(message) {  }

            public AuthorSolutionRunningException(string message, Exception inner) : base(message, inner) {  }

        }

        public class AuthorSolutionNotFoundException : Exception
        {

            public AuthorSolutionNotFoundException() { }

            public AuthorSolutionNotFoundException(string message) : base(message) { }

            public AuthorSolutionNotFoundException(string message, Exception inner) : base(message, inner) { }

        }

        public class UnknownException : Exception
        {

            public UnknownException() { }

            public UnknownException(string message) : base(message) { }

            public UnknownException(string message, Exception inner) : base(message, inner) { }

        }

    }
}
