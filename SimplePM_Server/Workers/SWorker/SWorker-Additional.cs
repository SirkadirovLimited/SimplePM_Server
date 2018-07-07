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
using System;
using System.IO;
using NLog.Config;
using JudgePlugin;
using CompilerPlugin;
using Newtonsoft.Json;
using ServerPlugin;
using SimplePM_Server.Workers.Recourse;

namespace SimplePM_Server.Workers
{
    
    public partial class SWorker
    {

        private void LoadConfigurations()
        {
            
            // Загружаем конфигурацию сервера в память
            _serverConfiguration = JsonConvert.DeserializeObject(
                File.ReadAllText("./config/server.json")
            );
            
            // Загружаем конфигурацию подключения к БД в память
            _databaseConfiguration = JsonConvert.DeserializeObject(
                File.ReadAllText("./config/database.json")
            );
            
            // Загружаем конфигурацию безопасности сервера в память
            _securityConfiguration = JsonConvert.DeserializeObject(
                File.ReadAllText("./config/security.json")
            );
            
            // Загружаем конфигурации модулей компиляции в память
            _compilerConfigurations = JsonConvert.DeserializeObject(
                File.ReadAllText("./config/compilers.json")
            );
            
            /*
             * Конфигурируем журнал событий (библиотека NLog)
             */

            try
            {

                LogManager.Configuration = new XmlLoggingConfiguration(
                    "./config/logger.config"
                );

            }
            catch
            {
                /* Deal with it */
            }
            
        }

        private void LoadPlugins()
        {

            // Выполняем загрузку плагинов сервера
            LoadServerPlugins();
            
            // Выполняем загрузку плагинов компиляции
            LoadCompilerPlugins();
            
            // Выполняем загрузку плагинов судейства
            LoadJudgePlugins();
            
            /*
             * Метод выполняет загрузку и инициализацию
             * плагинов сервера проверки решений.
             */
            
            void LoadServerPlugins()
            {
                
                // Получаем список плагинов сервера
                _serverPlugins = SPluginsLoader.LoadPlugins<IServerPlugin>(
                    (string)(_serverConfiguration.path.IServerrPlugin),
                    "ServerPlugin.Main"
                );
                
            }
            
            /*
             * Метод выполняет загрузку и инициализацию
             * плагинов поддержки компиляции.
             */
            
            void LoadCompilerPlugins()
            {

                // Получаем список модулей компиляции
                _compilerPlugins = SPluginsLoader.LoadPlugins<ICompilerPlugin>(
                    (string)(_serverConfiguration.path.ICompilerPlugin),
                    "CompilerPlugin.Compiler"
                );

            }
            
            /*
             * Метод  выполняет  загрузку  и  инициализацию
             * плагинов судейства пользовательских решений.
             */
            
            void LoadJudgePlugins()
            {

                // Получаем список модулей оценивания
                _judgePlugins = SPluginsLoader.LoadPlugins<IJudgePlugin>(
                    (string)(_serverConfiguration.path.IJudgePlugin),
                    "JudgePlugin.Judge"
                );

            }
            
        }
        
        private void RunAutoConfig()
        {
            
            // rechecks without timeout
            if (_serverConfiguration.submission.rechecks_without_timeout == "auto")
                _serverConfiguration.submission.rechecks_without_timeout = Environment.ProcessorCount.ToString();

            // max threads
            if (_serverConfiguration.submission.max_threads == "auto")
                _serverConfiguration.submission.max_threads = Environment.ProcessorCount.ToString();
            
        }
        
        private void CleanTempDirectory()
        {

            try
            {

                // Удаляем все файлы
                foreach (var file in Directory.GetFiles((string) (_serverConfiguration.path.temp)))
                    File.Delete(file);

                // Удаляем все папки, а также их содержимое
                foreach (var dir in Directory.GetDirectories((string) (_serverConfiguration.path.temp)))
                    Directory.Delete(dir, true);

            }
            catch (Exception ex)
            {

                // Записываем информацию об исключении в лог-файл
                logger.Warn("An error occured while trying to clear temp folder: " + ex);

            }
            
        }
        
    }
    
}