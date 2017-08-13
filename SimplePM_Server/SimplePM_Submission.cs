/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 + NON-COMMERCIALlicense.
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
        /// Язык программирования, на котором
        /// было написано решение
        ///////////////////////////////////////////////////

        public enum SubmissionLanguage
        {

            Unset,
            Freepascal,
            CSharp,
            Cpp,
            C,
            Python,
            Lua,
            Java,
            PHP
            
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
                    return SubmissionLanguage.Freepascal;
                case "csharp":
                    return SubmissionLanguage.CSharp;
                case "cpp":
                    return SubmissionLanguage.Cpp;
                case "c":
                    return SubmissionLanguage.C;
                case "python":
                    return SubmissionLanguage.Python;
                case "lua":
                    return SubmissionLanguage.Lua;
                case "java":
                    return SubmissionLanguage.Java;
                case "php":
                    return SubmissionLanguage.PHP;
                default:
                    return SubmissionLanguage.Unset;
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
                case SubmissionLanguage.Freepascal:
                    return "pas";
                case SubmissionLanguage.CSharp:
                    return "cs";
                case SubmissionLanguage.Cpp:
                    return "cpp";
                case SubmissionLanguage.C:
                    return "c";
                case SubmissionLanguage.Python:
                    return "py";
                case SubmissionLanguage.Lua:
                    return "lua";
                case SubmissionLanguage.PHP:
                    return "php";
                case SubmissionLanguage.Java:
                    return "java";
                default:
                    return "txt";
            }

        }

        ///////////////////////////////////////////////////
        
    }

}
