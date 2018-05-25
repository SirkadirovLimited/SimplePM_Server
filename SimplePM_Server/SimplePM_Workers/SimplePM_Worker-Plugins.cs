/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server
 * A part of SimplePM programming contests management system.
 *
 * Copyright 2017 Yurij Kadirov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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

using JudgeBase;
using CompilerBase;

namespace SimplePM_Server
{

    public partial class SimplePM_Worker
    {

        /*
         * Функция загружает в память
         * модули сервера проверки ре
         * шений, которые собирает из
         * специально   заготовленной
         * директории на диске.
         */

        private void LoadServerPlugins()
        {
            
            // Получаем список плагинов сервера
            _compilerPlugins = SimplePM_PluginsLoader.LoadPlugins<ICompilerPlugin>(
                (string)_serverConfiguration.path.ICompilerPlugin,
                "ServerPlugin"
            );
            
        }
        
        /*
         * Функция загружает в память компиляционные
         * модули,  которые  собирает  из специально
         * заготовленной директории на диске.
         */

        private void LoadCompilerPlugins()
        {

            // Получаем список модулей компиляции
            _compilerPlugins = SimplePM_PluginsLoader.LoadPlugins<ICompilerPlugin>(
                (string)_serverConfiguration.path.ICompilerPlugin,
                "CompilerPlugin.Compiler"
            );

        }

        /*
         * Функция загружает в память
         * модули оценивания пользова
         * тельского решения поставле
         * нной задачи.
         */

        private void LoadJudgePlugins()
        {

            // Получаем список модулей оценивания
            _judgePlugins = SimplePM_PluginsLoader.LoadPlugins<IJudgePlugin>(
                (string)_serverConfiguration.path.IJudgePlugin,
                "JudgePlugin.Judge"
            );

        }
        
    }

}
