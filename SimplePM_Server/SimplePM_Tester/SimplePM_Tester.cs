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
/*! \file */

using System.Collections.Generic;
using CompilerBase;
using IniParser.Model;
using MySql.Data.MySqlClient;

using static SimplePM_Server.SimplePM_Submission;

namespace SimplePM_Server.SimplePM_Tester
{

    internal partial class SimplePM_Tester
    {

        #region Секция объявления глобальных переменных
        private List<ICompilerPlugin> _compilerPlugins ; //!< ссылка на список модулей компиляции
        private MySqlConnection       connection       ; //!< ссылка на дескриптор соединения к базе данных
        private SubmissionInfo        submissionInfo   ; //!< ссылка на объект, содержащий информацию о запросе на тестирование
        private IniData               sConfig          ; //!< ссылка на дескриптор файла конфигурации сервера проверки пользовательских решений
        private string                exeFileUrl       ; //!< переменная, содержащая полный путь к exe-файлу пользовательского решения
        #endregion

        public SimplePM_Tester(
            ref MySqlConnection connection,
            ref List<ICompilerPlugin> _compilerPlugins,
            ref string exeFileUrl,
            ref SubmissionInfo submissionInfo,
            ref IniData sConfig
        )
        {

            this.connection = connection;
            this._compilerPlugins = _compilerPlugins;
            this.exeFileUrl = exeFileUrl;
            this.submissionInfo = submissionInfo;
            this.sConfig = sConfig;

        }

    }

}
