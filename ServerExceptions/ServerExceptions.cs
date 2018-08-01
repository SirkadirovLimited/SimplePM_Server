/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 * 
 * SimplePM Server is a part of software product "Automated
 * verification system for programming tasks "SimplePM".
 * 
 * Copyright (C) 2016-2018 Yurij Kadirov
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 * 
 * GNU Affero General Public License applied only to source code of
 * this program. More licensing information hosted on project's website.
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
        
        public PluginLoadingException(Exception inner) : base("", inner) { }
        
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