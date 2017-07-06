/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under CC BY-NC-SA 4.0 license.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */
/*! \file */

namespace SimplePM_Server
{
    /*
     * \brief
     * Класс содержит enum-ы и функции, которые
     * применяются к пользовательским запросам
     * на тестирование решений задач.
     */

    class SimplePM_Submission
    {
        ///////////////////////////////////////////////////
        /// Тип запроса на тестирование
        ///////////////////////////////////////////////////

        public enum SubmissionType
        {
            unset = 0,
            syntax = 2,
            debug = 4,
            release = 8
        }

        ///////////////////////////////////////////////////
        /// Язык программирования, на котором
        /// было написано решение
        ///////////////////////////////////////////////////

        public enum SubmissionLanguage
        {
            unset,
            freepascal,
            csharp,
            cpp,
            c,
            python,
            lua,
            java,
            php
        }

        ///////////////////////////////////////////////////
        /// Функция возвращает enum языка программирования
        /// по его текстовому названию
        ///////////////////////////////////////////////////

        public static SubmissionLanguage GetCodeLanguageByName(string codeLang)
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
                case "php":
                    return SubmissionLanguage.php;
                default:
                    return SubmissionLanguage.unset;
            }
        }

        ///////////////////////////////////////////////////
        /// Функция возвращает расширение файла
        /// исходного кода программы по имени
        /// языка программирования
        ///////////////////////////////////////////////////

        public static string GetExtByLang(SubmissionLanguage lang)
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
                case SubmissionLanguage.php:
                    return "php";
                case SubmissionLanguage.java:
                    return "java";
                default:
                    return "txt";
            }
        }

        ///////////////////////////////////////////////////
    }
}
