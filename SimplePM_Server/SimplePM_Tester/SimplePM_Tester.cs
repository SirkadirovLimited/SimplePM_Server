/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

using CompilerBase;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace SimplePM_Server.SimplePM_Tester
{

    /*
     * Класс тестирования пользовательских
     * решений   поставленных   задач   по
     * программированию.
     */

    internal partial class SimplePM_Tester
    {

        #region Секция объявления глобальных переменных
        private SubmissionInfo.SubmissionInfo submissionInfo; // информация о запросе

        private List<ICompilerPlugin> _compilerPlugins; // ссылка на список модулей компиляции

        private dynamic _serverConfiguration;
        private dynamic _languageConfigurations;

        private MySqlConnection connection; // соединение с БД
        private readonly string exeFileUrl; // путь к исполняемому файлу
        #endregion

        /*
         * Основной конструктор данного класса.
         */

        public SimplePM_Tester(
            ref MySqlConnection connection,
            ref dynamic serverConfiguration,
            ref dynamic languageConfigurations,
            ref List<ICompilerPlugin> _compilerPlugins,
            string exeFileUrl,
            ref SubmissionInfo.SubmissionInfo submissionInfo
        )
        {

            this.connection = connection;

            _serverConfiguration = serverConfiguration;
            _languageConfigurations = languageConfigurations;

            this._compilerPlugins = _compilerPlugins;

            this.exeFileUrl = exeFileUrl;
            this.submissionInfo = submissionInfo;

        }

    }

}
