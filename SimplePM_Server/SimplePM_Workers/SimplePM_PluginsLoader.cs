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

using NLog;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace SimplePM_Server
{

    public static class SimplePM_PluginsLoader
    {

        /*
         * Объявляем переменную указателя
         * на менеджер  журнала собылий и
         * присваиваем  ей  указатель  на
         * журнал событий текущего класса
         */

        private static readonly Logger logger = LogManager.GetLogger("SimplePM_PluginsLoader");

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

            /*
             * Записываем в лог-файл информацию о том,
             * что  собираемся   подгружать  сторонние
             * модули компиляции.
             */

            logger.Debug(typeof(TPlugin).Name + " modules are being loaded...");

            /*
             * Проводим инициализацию необходимых
             * для продолжения работы переменных.
             */

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

                /*
                 * Указываем в логе, что начинаем
                 * загружать  определённый модуль
                 * компиляции.
                 */

                logger.Debug("Start loading plugin [" + pluginFilePath + "]...");

                /*
                 * Выполняем все небезопасные действия
                 * в блоке  обработки  исключений  для
                 * обеспечения      отказоустойчивости
                 * сервера проверки решений задач.
                 */
                
                try
                {

                    /*
                     * Загружаем сборку из файла по указанному пути
                     */

                    var assembly = Assembly.LoadFrom(pluginFilePath);

                    /*
                     * Ищем необходимую для нас реализацию интерфейса
                     */

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

                    /*
                     * В случае возникновения ошибок
                     * записываем информацию о них в
                     * лог-файле.
                     */

                    logger.Warn("Plugin loading failed [" + pluginFilePath + "]:" + ex);

                }

            }

            /*
             * Записываем  в  лог-файл информацию о том,
             * что  мы  завершили процесс загрузки  всех
             * модулей компиляции (ну или не всех).
             */
            
            logger.Debug(typeof(TPlugin).Name + " modules were loaded...");

            /*
             * Возвращаем список найденных плагинов.
             */

            return _pluginsList;

        }

    }

}
