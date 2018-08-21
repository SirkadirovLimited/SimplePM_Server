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
using System.Text;
using NLog.Config;
using JudgePlugin;
using ServerPlugin;
using CompilerPlugin;
using Newtonsoft.Json;
using SimplePM_Server.Workers.Recourse;

namespace SimplePM_Server.Workers
{
    
    public partial class SWorker
    {

        /*
         * Метод, отвечающий за загрузку серверных
         * конфигураций в память приложения.
         */
        
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
            
            try
            {

                // Загружаем глобальную конфигурацию NLog-а
                LogManager.Configuration = new XmlLoggingConfiguration(
                    "./config/logger.config"
                );

            }
            catch
            {
                /* Deal with it */
            }
            
        }

        /*
         * Метод, отвечающий за загрузку плагинов
         * сервера проверки решений в память.
         */
        
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
                    (string)(_serverConfiguration.path.IServerPlugin),
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
        
        /*
         * Метод автоконфигурации сервера. Если в конфигурационном
         * файле явно это указно, автоматически определяет рекомендуемые
         * значения некоторых параметров приложения.
         */
        
        private void RunAutoConfig()
        {

            // Максимальное количество одновлеменных проверок
            if (_serverConfiguration.submission.max_threads == "auto")
                _serverConfiguration.submission.max_threads = Environment.ProcessorCount.ToString();
            
        }
        
        /*
         * Метод выполняет очистку временной
         * директории сервера проверки решений.
         */
        
        private void PrepareTempDirectory()
        {

            try
            {

                // Удаляем все файлы в хранилище временных файлов сервера
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

                // Завершаем работу сервера проверки решений
                Environment.Exit(-1);
                
            }
            
        }

        /*
         * Метод генерирует уникальный идентификатор
         * для данного сервера проверки решений, а
         * также загружает его в память.
         *
         * Если файл с кодом уже существует, метод
         * лишь осуществляет его чтение и запись в
         * память приложения.
         */
        
        private void GenerateServerID(string idFilePath = "./config/server.id")
        {

            try
            {

                // Генерируем новый идентификатор сервера если сейчас он не существует
                if (!File.Exists(idFilePath))
                {
                    
                    // Записываем идентификатор в файл
                    File.WriteAllText(idFilePath, Guid.NewGuid().ToString(), Encoding.ASCII);
                    
                    // Устанавливаем некоторые аттрибуты файла
                    File.SetAttributes(idFilePath, FileAttributes.NotContentIndexed);
                    
                }
                
                // Получаем уникальный идентификатор сервера
                _serverId = new Guid(File.ReadAllText(idFilePath, Encoding.ASCII));
                
            }
            catch (Exception ex)
            {
                
                // Записываем информацию об исключении в лог-файл
                logger.Fatal("Something went wrong while trying to get/set unique ServerID: " + ex);
                
                // Завершаем работу сервера
                Environment.Exit(-1);
                
            }
            
        }
        
    }
    
}