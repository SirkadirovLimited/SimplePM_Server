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

namespace SimplePM_Server.Workers
{
    
    public partial class SWorker
    {

        public SWorker()
        {

            // Устанавливаем обработчик исключений
            SetExceptionHandler();

            // Запрашваем генерацию/открыие уникального идентификатора сервера
            GenerateServerID();
            
            // Выполняем загрузку конфигурационных файлов сервера
            LoadConfigurations();
            
            // Автодоконфигурация компонентов сервера
            RunAutoConfig();
            
            // Очищаем директорию временных файлов
            PrepareTempDirectory();
            
            // Выполняем загрузку всех плагинов
            LoadPlugins();
            
            // Получаем список поддерживаемых сервером ЯП
            _enabledLanguagesString = SProgrammingLanguagesLoader.GetEnabledLanguagesString();

            // Записываем список всех поддерживаемых сервером ЯП в базу данных
            SProgrammingLanguagesLoader.SendEnabledLanguagesToServer();
            
            // Записываем список всех поддерживаемых сервером плагинов-судей в базу данных
            SProgrammingLanguagesLoader.SendSupportedJudgesToServer();

        }

    }
    
}