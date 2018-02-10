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

namespace SimplePM_Server
{

    /*
     * Основной класс, метод Main которого инициализирует объект
     * базового класса сервера проверки пользовательских решений
     * и вызывает метод инициализации сервера.
     */
    
    class SimplePM_Main
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
