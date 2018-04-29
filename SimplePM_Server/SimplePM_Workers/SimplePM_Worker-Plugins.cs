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
