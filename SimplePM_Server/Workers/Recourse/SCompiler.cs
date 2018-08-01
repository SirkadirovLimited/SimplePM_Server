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