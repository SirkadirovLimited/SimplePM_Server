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

// Для работы с процессами
using System.Diagnostics;
// Для парсинга конфигурационных файлов
using IniParser.Model;

namespace CompilerBase
{

    /*!
     * \brief
     * Интерфейс, который предоставляет возможность
     * создания собственных модулей компиляторов
     * для различных языков программирования.
     */
    
    public interface ICompilerPlugin
    {

        string CompilerPluginLanguageName { get; } //!< Имя языка программирования, поддержка которого осуществляется модулем
        string CompilerPluginLanguageExt { get; } //!< Расширение файла исходного кода языка программирования, поддержка которого осуществляется модулем
        string CompilerPluginDisplayName { get; } //!< Отображаемое имя модуля
        string CompilerPluginAuthor { get; } //!< Имя автора или организации, которая создала модуль
        string CompilerPluginSupportUrl { get; } //!< URL адрес сайта поддержки для пользователей модуля

        ///////////////////////////////////////////////////
        /// Метод, который занимается запуском компилятора
        /// для данного пользовательского решения
        /// поставленной задачи, а также обработкой
        /// результата компиляции данной программы.
        ///////////////////////////////////////////////////

        CompilerResult StartCompiler(ref IniData sConfig, string submissionId, string fileLocation);

        ///////////////////////////////////////////////////
        /// Метод, который вызывается перед запуском
        /// пользовательского решения поставленной задачи
        /// и выполняет роль выборщика метода запуска
        /// пользовательской программы.
        ///////////////////////////////////////////////////

        bool SetRunningMethod(ref IniData sConfig, ref ProcessStartInfo startInfo, string filePath);

    }
    
}
