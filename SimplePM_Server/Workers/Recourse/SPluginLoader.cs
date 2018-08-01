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
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ServerExceptions;

namespace SimplePM_Server.Workers.Recourse
{
    
    public static class SPluginsLoader
    {
        
        private static readonly Logger logger = LogManager.GetLogger("SimplePM_Server.Workers.SPluginLoader");

        /*
         * Метод занимается  выборкой указанных
         * типов   плагинов  сервера   проверки
         * решений.
         *
         * Возвращает список найденных плагинов
         * указанного типа данных.
         */
        
        public static List<TPlugin> LoadPlugins<TPlugin>(string pluginsDirectoryPath, string typeFullName)
        {

            // Записываем информацию о начале выполнения операции в лог-файл
            logger.Debug(typeof(TPlugin).Name + " modules are being loaded...");

            // Создаём новый типизированный список плагинов
            var _pluginsList = new List<TPlugin>();

            /*
             * Производим выборку полных путей к
             * библиотекам, которые скорее всего
             * содержат  типы  плагинов, которые
             * мы в данный момент ищем.
             */
            
            var pluginFilesList = Directory.GetFiles(
                pluginsDirectoryPath,
                typeof(TPlugin).Name + ".*.dll"
            );

            /*
             * Производим полный перебор всех возможных
             * библиотек, предварительно отобранных для
             * этого  поиска,  в  случае  совпадения  с
             * искомым  типом   производим  необходимые
             * действия по добавлению найденного типа в
             * коллекцию таких типов.
             */

            foreach (var pluginFilePath in pluginFilesList)
            {

                // Начало загрузки плагина освещаем в лог-файле
                logger.Debug("Start loading plugin [" + pluginFilePath + "]...");

                try
                {

                    // Загружаем сборку из файла по указанному пути
                    var assembly = Assembly.LoadFrom(pluginFilePath);

                    // Ищем необходимую для нас реализацию интерфейса
                    foreach (var type in assembly.GetTypes())
                    {

                        /*
                         * Если мы не нашли то, что искали - переходим
                         * к следующей итерации цикла foreach,  в ином
                         * случае  продолжаем  выполнение  необходимых
                         * действий по добавлению плагина в список.
                         */

                        if (type.FullName != typeFullName) continue;

                        // Добавляем плагин в список
                        _pluginsList.Add(
                            (TPlugin)Activator.CreateInstance(type)
                        );

                        // Записываем сообщение об успехе в лог-файл
                        logger.Debug("Plugin successfully loaded [" + pluginFilePath + "]");

                        // Выходим из цикла foreach
                        break;

                    }

                }
                catch (Exception ex)
                {

                    // Записываем информацию о возникшем исключении в лог-файл
                    logger.Error("Plugin loading failed [" + pluginFilePath + "]:" + new PluginLoadingException(ex));

                }

            }

            // Информируем об успешности выполнения операциий
            logger.Debug(typeof(TPlugin).Name + " modules were loaded...");

            // Возвращаем список найденных плагинов
            return _pluginsList;

        }
        
    }
    
}