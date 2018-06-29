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

using System.Collections.Generic;

namespace SimplePM_Server.Workers
{
    
    public partial class SWorker
    {
        
        private string GetEnabledLangsAsString()
        {

            /*
             * Записываем  в  лог, что генерация списка
             * доступных   на   этом  сервере  проверки
             * пользовательских   решений    задач   по
             * программированию   скриптовых  языков  а
             * также языков программирования начинается
             * с этого великолепного момента.
             */

            logger.Debug("Generation of enabled programming languages list started.");

            // Инициализируем список строк
            var enabledLangsList = new List<string>();
            
            /*
             * В цикле перебираем все поддерживаемые языки
             * программирования  подключаемыми  модулями и
             * приводим  список   поддерживаемых  системой
             * языков к требуемому виду.
             */

            foreach (var compilerConfig in _compilerConfigurations)
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
            
            /*
             * Выводим список доступных языков программирования
             * на данной сессии работы сервера в лог-файл.  Это
             * возможно  поможет  системному  администратору  с
             * решением  проблем,   связанных  с  подключаемыми
             * модулями и другими  изменяемыми частями  сервера
             * проверки решений SimplePM Server.
             */
            
            logger.Debug("Enabled compiler configurations list: " + resultStr);

            // Возвращаем результат выполнения метода
            return resultStr;

        }
        
    }
    
}