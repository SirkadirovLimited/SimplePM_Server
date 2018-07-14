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