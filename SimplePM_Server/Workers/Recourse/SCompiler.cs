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

using System.IO;
using CompilerPlugin;

namespace SimplePM_Server.Workers.Recourse
{
    
    public static class SCompiler
    {
        
        //TODO: Одинаковые функции с SJudge!
        public static ICompilerPlugin FindCompilerPlugin(
            string pluginName
        )
        {

            // Производим поиск плагина
            foreach (var plugin in SWorker._compilerPlugins)
                if (plugin.PluginInformation.Name == pluginName)
                    return plugin;

            // В противном случае возвращаем null
            return null;

        }
        
        public static dynamic GetCompilerConfig(string languageName)
        {

            // Производим перебор всех конфигураций компиляторов
            foreach (var compilerConfig in SWorker._compilerConfigurations)
            {

                /*
                 * Если текущая конфигурация является
                 * искомой, возвращаем ссылку на неё.
                 */
                if ((string)compilerConfig.language_name == languageName)
                    return compilerConfig;

            }

            // В противном случае возвращаем null
            return null;

        }

        public static CompilationResult ChooseCompilerAndRun(
            ref dynamic _compilerConfig,
            ref ICompilerPlugin _compilerPlugin,
            string submissionId,
            string fileLocation
        )
        {
            
            // Проверяем путь к исходному коду на ошибки
            if (string.IsNullOrEmpty(fileLocation) || string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
                throw new FileNotFoundException("File not found!", fileLocation);
            
            // Проверка на наличие плагина, отвечающего за компиляцию
            if (_compilerPlugin == null)
            {

                // Сигнализируем об ошибке
                return new CompilationResult
                {
                    HasErrors = true, // Хьюстон, у нас проблема!
                    CompilerOutput = "Language not supported by SimplePM!"
                };

            }

            // Возвращаем результаты компиляции
            return _compilerPlugin.RunCompiler(
                ref _compilerConfig,
                submissionId,
                fileLocation
            );

        }
        
    }
    
}