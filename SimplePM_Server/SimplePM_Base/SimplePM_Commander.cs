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

//База
using System;
//Для логгирования
using NLog;

namespace SimplePM_Server
{

    /*
     * Класс  описывает  функционал,
     * необходимый   для   обработки
     * аргументов коммандной строки.
     */
    class SimplePM_Commander
    {

        /*
            Объявляем переменную указателя на менеджер журнала собылий
            и присваиваем ей указатель на журнал событий текущего класса
        */
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        ///////////////////////////////////////////////////
        /// Функция, обрабатывающая аргументы запуска
        /// сервера проверки решений SimplePM.
        ///////////////////////////////////////////////////

        public void SplitArguments(string[] args)
        {

            // Если аргументы не передавали - выходим
            if (args.Length <= 0)
                return;

            // В инном случае обрабатываем все полученные аргументы
            foreach (var arg in args)
            {

                //Формируем название функции
                var command = arg.ToLower().Replace("-", "CMD");

                Console.WriteLine(@"/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\");

                try
                {

                    // Производим попытку вызова функции
                    typeof(SimplePM_Commands)
                        .GetMethod(command)
                        .Invoke(new SimplePM_Commands(), null);

                }
                catch (Exception ex)
                {

                    // Выводим ошибку на экран
                    Console.WriteLine(Properties.Resources.commanderError);

                    // Производим запись ошибки в журнал
                    logger.Error(ex);

                }

                Console.WriteLine(@"/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\");

            }

        }

    }

    /*
     * Класс описывает методы, которые
     * доступны  для  вызова с помощью
     * аргументов  коммандной  строки.
     */
    class SimplePM_Commands
    {
        
        /*
         * Функция задаёт переменные среды,   необходимые
         * для работы дополнительного ПО SimplePM_Server.
         */
        public void CMDsetenvars()
        {

            Console.WriteLine("Please, wait. We are creating environment variables...");

            //SPMS Path
            Console.WriteLine("SPMS-Path=" + Environment.CurrentDirectory);
            Environment.SetEnvironmentVariable("SPMS-Path", Environment.CurrentDirectory, EnvironmentVariableTarget.Machine);

            Console.WriteLine("Operation completed.");

        }
        
        /*
         * Функция   предоставляет  справку
         * по функционалу коммандной строки
         * SimplePM_Server.
         */
        public void CMDhelp()
        {

            Console.WriteLine(Properties.Resources.commanderHelp);

            //Pause
            Console.Write("Press RETURN (ENTER) to continue...");
            Console.ReadLine();

        }

        /*
         * Очень интересная и познвательная
         * функция только для избранных.
         */
        public void CMDbirch()
        {

            Console.WriteLine("I keep secrets.");

        }

    }

}
