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

namespace SimplePM_Exceptions
{

    [Serializable]
    public class AuthorSolutionRunningException : Exception
    {

        public AuthorSolutionRunningException() { }

        public AuthorSolutionRunningException(string message) : base(message) { }

        public AuthorSolutionRunningException(string message, Exception inner) : base(message, inner) { }

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
    public class DatabaseQueryException : Exception
    {

        public DatabaseQueryException() { }

        public DatabaseQueryException(string message) : base(message) { }

        public DatabaseQueryException(string message, Exception inner) : base(message, inner) { }

    }

    [Serializable]
    public class UnknownException : Exception
    {

        public UnknownException() { }

        public UnknownException(string message) : base(message) { }

        public UnknownException(string message, Exception inner) : base(message, inner) { }

    }

}
