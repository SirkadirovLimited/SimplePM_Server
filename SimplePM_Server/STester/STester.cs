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

using MySql.Data.MySqlClient;

namespace SimplePM_Server.STester
{

    /*
     * Класс тестирования пользовательских
     * решений   поставленных   задач   по
     * программированию.
     */

    internal partial class STester
    {

        #region Секция объявления глобальных переменных
        private SubmissionInfo.SubmissionInfo submissionInfo; // информация о запросе

        private dynamic _serverConfiguration;
        private dynamic _languageConfigurations;

        private MySqlConnection connection; // соединение с БД
        private readonly string exeFileUrl; // путь к исполняемому файлу
        #endregion

        /*
         * Основной конструктор данного класса.
         */

        public STester(
            ref MySqlConnection connection,
            ref dynamic serverConfiguration,
            ref dynamic languageConfigurations,
            string exeFileUrl,
            ref SubmissionInfo.SubmissionInfo submissionInfo
        )
        {

            this.connection = connection;

            _serverConfiguration = serverConfiguration;
            _languageConfigurations = languageConfigurations;

            this.exeFileUrl = exeFileUrl;
            this.submissionInfo = submissionInfo;

        }

    }

}
