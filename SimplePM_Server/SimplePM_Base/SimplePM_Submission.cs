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
/*! \file */

using System.Collections.Generic;
using System.Linq;
using CompilerBase;

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

        public class SubmissionInfo
        {

            public int SubmissionId { get; set; }
            public int UserId { get; set; }
            public int ProblemId { get; set; }

            public int ClassworkId { get; set; }
            public int OlympId { get; set; }

            public string CodeLang { get; set; }
            public string TestType { get; set; }
            public string CustomTest { get; set; }

            public byte[] ProblemCode { get; set; }

            public int ProblemDifficulty { get; set; }

            public SubmissionInfo
                (
                    int _submissionId,
                    int _userId,
                    int _problemId,

                    int _classworkId,
                    int _olympId,

                    string _codeLang,
                    string _testType,
                    string _customTest,
                    
                    byte[] _problemCode,

                    int _problemDifficulty = 0
                )
            {

                SubmissionId = _submissionId;
                UserId = _userId;
                ProblemId = _problemId;

                ClassworkId = _classworkId;
                OlympId = _olympId;

                CodeLang = _codeLang;
                TestType = _testType;
                CustomTest = _customTest;

                ProblemCode = _problemCode;

                ProblemDifficulty = _problemDifficulty;

            }

        }

        ///////////////////////////////////////////////////
        /// Функция возвращает расширение файла
        /// исходного кода программы по имени
        /// языка программирования
        ///////////////////////////////////////////////////

        public static string GetExtByLang(string lang, ref List<ICompilerPlugin> _compilerPlugins)
        {

            return (
                from compilerPlugin
                in _compilerPlugins
                where compilerPlugin.CompilerPluginLanguageName == lang
                select compilerPlugin.CompilerPluginLanguageExt
            ).FirstOrDefault();

        }

        ///////////////////////////////////////////////////
        
    }

}
