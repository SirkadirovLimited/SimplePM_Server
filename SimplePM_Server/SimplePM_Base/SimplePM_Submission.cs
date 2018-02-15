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
using System.Collections.Generic;
using System.Linq;
using CompilerBase;

namespace SimplePM_Server
{

    /*
     * Класс содержит enum-ы и функции, которые
     * применяются к пользовательским запросам
     * на тестирование решений задач.
     */
    
    class SimplePM_Submission
    {
        
        /*
         * Функция возвращает расширение файла
         * исходного  кода  программы по имени
         * языка программирования.
         */
        public static string GetExtByLang(string lang, ref List<ICompilerPlugin> _compilerPlugins)
        {

            throw new Exception();

        }
        
    }

}
