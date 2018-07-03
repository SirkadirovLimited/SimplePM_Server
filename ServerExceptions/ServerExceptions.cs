/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server is a part of software product "Automated
 * vefification system for programming tasks "SimplePM".
 *
 * Copyright 2018 Yurij Kadirov
 *
 * Source code of the product licensed under the Apache License,
 * Version 2.0 (the "License");
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

using System;

namespace ServerExceptions
{

    [Serializable]
    public class AuthorSolutionException : Exception
    {

        public AuthorSolutionException(string message) : base(message) { }

        public AuthorSolutionException(Exception inner) : base("AuthorSolutionException", inner) { }

    }

    [Serializable]
    public class PluginLoadingException : Exception
    {

        public PluginLoadingException() { }

        public PluginLoadingException(string message) : base(message) { }

        public PluginLoadingException(string message, Exception inner) : base(message, inner) { }

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