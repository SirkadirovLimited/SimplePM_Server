﻿/*
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
using JudgePlugin;
using ServerPlugin;
using CompilerPlugin;
using System.Collections.Generic;

namespace SimplePM_Server.Workers
{
    
    public partial class SWorker
    {
        
        private static readonly Logger logger = LogManager.GetLogger("SimplePM_Server.Workers.SWorker");

        public static dynamic _serverConfiguration { get; private set; }
        private static dynamic _databaseConfiguration { get; set; }
        public static dynamic _securityConfiguration { get; private set; }
        public static dynamic _compilerConfigurations { get; private set; }
        
        public static Guid _serverId { get; private set; }

        public static sbyte _aliveTestersCount { get; private set; }
        private readonly string _enabledLanguagesString;

        public static List<IServerPlugin> _serverPlugins { get; private set; }
        public static List<ICompilerPlugin> _compilerPlugins { get; private set; }
        public static List<IJudgePlugin> _judgePlugins { get; private set; }
        
    }
    
}