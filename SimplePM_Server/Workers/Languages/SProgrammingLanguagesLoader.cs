/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server is a part of software product "Automated
 * vefification system for programming tasks "SimplePM".
 *
 * Copyright 2018 Yurij Kadirov
 *
 * Source code of the product licensed under the Apache License,
 * Version 2.0 (the "License");
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

using NLog;
using System.Collections.Generic;

namespace SimplePM_Server.Workers
{
    
    public static class SProgrammingLanguagesLoader
    {
        
        private static readonly Logger logger = LogManager.GetLogger("SimplePM_Server.Workers.ProgrammingLanguagesLoader");
        
        /*
         * Метод отвечает за генерацию списка поддерживаемых
         * сервером проверки решений языков программирования.
         */
        
        public static string GetEnabledLangsAsString()
        {

            // Записываем в лог-файл информацию о начале выполнения операции
            logger.Debug("Generation of enabled programming languages list started.");

            // Инициализируем список строк
            var enabledLangsList = new List<string>();
            
            // Обрабатываем все предоставленные конфигурации
            foreach (var compilerConfig in SWorker._compilerConfigurations)
            {

                // Добавляем в список только включённые конфигурации
                if (compilerConfig.enabled != "true") continue;

                // Добавляем текущую конфигурацию в список
                enabledLangsList.Add("'" + (string)(compilerConfig.language_name) + "'");

                // Записываем информацию об этом в лог-файл
                logger.Debug(
                    "Compiler configuration '" +
                    (string)(compilerConfig.language_name) +
                    "' loaded!"
                );

            }

            // Формируем список доступных языков в виде строки
            var resultStr = string.Join(", ", enabledLangsList);
            
            // Записываем список поддерживаемых языков программирования в лог
            logger.Debug("Enabled compiler configurations list: " + resultStr);

            // Возвращаем результат выполнения метода
            return resultStr;

        }
        
    }
    
}