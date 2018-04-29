/*
 * Copyright (C) 2018, Yurij Kadirov.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Yurij Kadirov
 * @Website: https://spm.sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

namespace SimplePM_Server
{

    /*
     * Основной класс, метод Main которого инициализирует объект
     * базового класса сервера проверки пользовательских решений
     * и вызывает метод инициализации сервера.
     */
    
    internal class SimplePM_Main
    {

        public static void Main(string[] args)
        {
            
            /*
             * Инициализируем объект типа
             * SimplePM_Worker и вызываем
             * метод запуска сервера.
             */

            new SimplePM_Worker().Run(args);

        }

    }

}
