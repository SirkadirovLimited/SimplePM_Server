using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePM_Server
{
    class Submission
    {
        public enum SubmissionType
        {
            unset = 0,
            syntax = 2,
            debug = 4,
            release = 8
        }
        public enum SubmissionLanguage
        {
            unset = 0,
            freepascal = 1,
            csharp = 2,
            cpp = 3,
            c = 4,
            python = 5,
            lua = 6
        }
        public enum SubmissionStatus
        {
            unset = 0,
            operating = 1,
            compiling = 2,
            compilationError = 3,
            compiled = 4,
            testing = 5,
            finished = 6
        }

        public static SubmissionLanguage getCodeLanguageByName(string codeLang)
        {
            switch (codeLang)
            {
                case "freepascal":
                    return SubmissionLanguage.freepascal;
                case "csharp":
                    return SubmissionLanguage.csharp;
                case "cpp":
                    return SubmissionLanguage.cpp;
                case "c":
                    return SubmissionLanguage.c;
                case "python":
                    return SubmissionLanguage.python;
                case "lua":
                    return SubmissionLanguage.lua;
                default:
                    return SubmissionLanguage.unset;
            }
        }

        public static string getExtByLang(SubmissionLanguage lang)
        {
            switch (lang)
            {
                case SubmissionLanguage.freepascal:
                    return "pas";
                case SubmissionLanguage.csharp:
                    return "cs";
                case SubmissionLanguage.cpp:
                    return "cpp";
                case SubmissionLanguage.c:
                    return "c";
                case SubmissionLanguage.python:
                    return "py";
                case SubmissionLanguage.lua:
                    return "lua";
                default:
                    return "txt";
            }
        }
    }
}
