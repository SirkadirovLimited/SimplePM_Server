/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 license.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */
/*! \file */

//База
using System;
//Для логгирования
using NLog;

namespace SimplePM_Server
{

    class SimplePM_Commander
    {

        /*!
            Объявляем переменную указателя на менеджер журнала собылий
            и присваиваем ей указатель на журнал событий текущего класса
        */
        public static Logger logger = LogManager.GetCurrentClassLogger();

        ///////////////////////////////////////////////////
        /// Функция, обрабатывающая аргументы запуска
        /// сервера проверки решений SimplePM.
        ///////////////////////////////////////////////////

        public void SplitArguments(string[] args)
        {

            //Если аргументы не передавали - выходим
            if (args.Length <= 0)
                return;

            //В инном случае обрабатываем все полученные аргументы
            foreach (string arg in args)
            {

                //Формируем название функции
                string command = arg.ToLower().Replace("-", "CMD");

                Console.WriteLine(@"/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\");

                try
                {

                    //Производим попытку вызова функции
                    typeof(SimplePM_Commands).GetMethod(command).Invoke(new SimplePM_Commands(), null);

                }
                catch (Exception ex)
                {

                    //Выводим ошибку на экран
                    Console.WriteLine(Properties.Resources.commanderError);

                    //Производим запись ошибки в журнал
                    logger.Error(ex);

                }

                Console.WriteLine(@"/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\");

            }

        }

    }

    class SimplePM_Commands
    {
        
        ///////////////////////////////////////////////////
        /// Функция задаёт переменные среды, необходимые
        /// для работы дополнительного ПО SimplePM_Server.
        ///////////////////////////////////////////////////
        public void CMDsetenvars()
        {

            Console.WriteLine("Please, wait. We are creating environment variables...");

            //SPMS Path
            Console.WriteLine("SPMS-Path=" + Environment.CurrentDirectory);
            Environment.SetEnvironmentVariable("SPMS-Path", Environment.CurrentDirectory, EnvironmentVariableTarget.Machine);

            Console.WriteLine("Operation completed.");

        }

        ///////////////////////////////////////////////////
        /// Функция задаёт переменные среды, необходимые
        /// для работы дополнительного ПО SimplePM_Server.
        ///////////////////////////////////////////////////
        public void CMDhelp()
        {

            Console.WriteLine(Properties.Resources.commanderHelp);

            //Pause
            Console.Write("Press RETURN (ENTER) to continue...");
            Console.ReadLine();

        }

        public void CMDbirch()
        {

            Console.WriteLine("I keep secrets.");

        }

    }

}
