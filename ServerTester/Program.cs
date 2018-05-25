/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server
 * A part of SimplePM programming contests management system.
 *
 * Copyright 2017 Yurij Kadirov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Visit website for more details: https://spm.sirkadirov.com/
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
