
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
            unset,
            freepascal,
            csharp,
            cpp,
            c,
            python,
            lua,
            java
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
                case "java":
                    return SubmissionLanguage.java;
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
                case SubmissionLanguage.java:
                    return "java";
                default:
                    return "txt";
            }
        }
    }
}
