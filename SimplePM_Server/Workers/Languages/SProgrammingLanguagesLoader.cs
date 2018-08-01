/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 * 
 * SimplePM Server is a part of software product "Automated
 * verification system for programming tasks "SimplePM".
 * 
 * Copyright (C) 2016-2018 Yurij Kadirov
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 * 
 * GNU Affero General Public License applied only to source code of
 * this program. More licensing information hosted on project's website.
 * 
 * Visit website for more details: https://spm.sirkadirov.com/
 */

using NLog;
using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace SimplePM_Server.Workers
{
    
    public static class SProgrammingLanguagesLoader
    {
        
        private static readonly Logger logger = LogManager.GetLogger("SimplePM_Server.Workers.ProgrammingLanguagesLoader");

        /*
         * С помощью данного метода можно с лёгкостью совершить
         * полный перебор всех поддерживаемых данным сервером
         * проверки решений языков программирования.
         */
        
        private static List<ItemType> GetLanguageInformation<ItemType>(
            Func<dynamic, ItemType> GetItemFunc
        )
        {

            // Объявляем новый список для хранения данных
            var informationList = new List<ItemType>();

            // Совершаем проход по всем конфигурациям поддерживаемых ЯП
            foreach (var compilerConfig in SWorker._compilerConfigurations)
            {
                
                // Обрабатываем только включённые компиляторы
                if (compilerConfig.enabled != "true") continue;

                // Получаем новый элемент для внесения в список
                ItemType listItem = GetItemFunc(compilerConfig);
                
                // Добавляем новый непустой элемент в список
                if (listItem != null)
                    informationList.Add(listItem);

            }

            // Возвращаем сгенерированный нами список
            return informationList;

        }
        
        /*
         * Метод отвечает за генерацию списка поддерживаемых
         * сервером проверки решений языков программирования.
         */
        
        public static string GetEnabledLanguagesString()
        {

            // Записываем сведения о начале выполнения операции в лог
            logger.Debug("Generation of enabled programming languages list started.");
            
            // Получаем список поддерживаемых сервером языков программирования в виде строки
            var enabledLanguagesString = string.Join(
                ", ",
                GetLanguageInformation(compilerConfig => "'" + (string) (compilerConfig.language_name) + "'")
            );

            // Записываем список поддерживаемых сервером языков программирования в лог
            logger.Info("Enabled compiler configurations list: " + enabledLanguagesString);
            
            // Записываем сведения об окончании выполнения операции в лог
            logger.Debug("Generation of enabled programming languages list ended.");
            
            // Возвращаем список поддерживаемых сервером языков программирования
            return enabledLanguagesString;

        }

        /*
         * Данный метод несёт ответственность за отправку списка
         * всех поддерживаемых сервером проверки решений языков
         * программирования в базу данных системы.
         */
        
        public static void SendEnabledLanguagesToServer()
        {

            // Создаём новое соединение к СУБД, получаем его дескриптор
            var conn = SWorker.GetNewMysqlConnection();

            // Удаляем все пункты списка поддерживаемых ЯП с текущим ServerID
            new MySqlCommand(
                Resources.delete_outdated_languages_query.Replace(
                    "@owner_server_id",
                    SWorker._serverId.ToString()
                ),
                conn
            ).ExecuteNonQuery();

            // Обрабатываем все доступные языки программирования
            GetLanguageInformation(compilerConfig =>
            {

                try
                {

                    // Создаём новый запрос на добавление языка программирования в список
                    var insertCmd = new MySqlCommand(Resources.send_supported_languages_query, conn);
                    
                    // Добавляем параметры запроса с экранированием
                    insertCmd.Parameters.AddWithValue("@title", (string)(compilerConfig.display_name));
                    insertCmd.Parameters.AddWithValue("@name", (string)(compilerConfig.language_name));
                    insertCmd.Parameters.AddWithValue("@syntax_name", (string)(compilerConfig.editor_mode));
                    insertCmd.Parameters.AddWithValue("@owner_server_id", SWorker._serverId.ToString());

                    // Выполняем запрос
                    insertCmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    
                    // Сигнализируем о наличии ошибки в лог-файл
                    logger.Error("An error occured while trying to send available lanuages to MySQL server: " + ex);

                    // Возвращаем результат-заглушку
                    return false;

                }
                
                // Возвращаем результат-заглушку
                return true;
                
            });

            try
            {

                // Закрываем соединение с СУБД
                conn.Close();
                
                // Проводим очистку всех связанных ресурсов
                conn.Dispose();

            }
            catch { /* Обработки исключений не предусмотрено */ }

        }

        public static void SendSupportedJudgesToServer()
        {
            
            // Создаём новое соединение к СУБД, получаем его дескриптор
            var conn = SWorker.GetNewMysqlConnection();

            // Удаляем все пункты списка поддерживаемых судей с текущим ServerID
            new MySqlCommand(
                Resources.delete_outdated_judges_query.Replace(
                    "@owner_server_id",
                    SWorker._serverId.ToString()
                ),
                conn
            ).ExecuteNonQuery();
            
            // Записываем иенформацию о всех судьях в базу данных в цикле
            foreach (var judgePlugin in SWorker._judgePlugins)
            {

                try
                {

                    // Создаём новый запрос на добавление плагина судьи в список
                    var insertCmd = new MySqlCommand(Resources.send_supported_judges_query, conn);
                    
                    // Добавляем параметры запроса с экранированием
                    insertCmd.Parameters.AddWithValue("@name", (string)(judgePlugin.PluginInformation.Name));
                    insertCmd.Parameters.AddWithValue("@owner_server_id", SWorker._serverId.ToString());

                    // Выполняем запрос
                    insertCmd.ExecuteNonQuery();
                    
                }
                catch (Exception ex)
                {
                    
                    logger.Error("An error occured while trying to send supported judges list to MySQL server: " + ex);
                    
                }
                
            }
            
        }
        
    }
    
}