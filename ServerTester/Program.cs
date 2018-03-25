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

using System.IO;
using CommandDotNet;
using CommandDotNet.Models;

namespace ServerTester
{

    public class Program
    {

        public const string configFilePath = "../config/database.json";

        public static int Main(string[] args)
        {

            // Проверяем на существование конфигурационного файла
            CheckConfigFileExists();

            // Формируем настройки парсера аргументов
            AppSettings settings = new AppSettings()
            {
                AllowArgumentSeparator = true,
                Case = Case.LowerCase,
                EnableVersionOption = true,
                ShowArgumentDetails = true,
                ThrowOnUnexpectedArgument = true
            };

            // Создаём экземпляр класса парсера аргументов
            AppRunner<Tester> runner = new AppRunner<Tester>(settings);

            // Запускаем парсер
            return runner.Run(args);

        }

        private static void CheckConfigFileExists()
        {

            // Проверка на существование конфигурационного файла
            if (!File.Exists(configFilePath))
                throw new FileNotFoundException("Database config file is not found!", configFilePath);

        }

    }

}
