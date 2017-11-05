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

// База для всего
using System;
//Безопасность
using System.Web;
// Для работы с процессами
using System.Diagnostics;
// Для работы с файловой системой
using System.IO;
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

        string CompilerPluginLanguageName { get; }
        string CompilerPluginDisplayName { get; }
        string CompilerPluginAuthor { get; }
        string CompilerPluginSupportUrl { get; }

        CompilerResult StartCompiler(ref IniData sConfig, string submissionId, string fileLocation);

        bool SetRunningMethod(ref IniData sConfig, ref ProcessStartInfo startInfo, string filePath);

    }

    /*!
     * \brief
     * Структура, которая позволяет хранить
     * информацию о результате компиляции
     * пользовательского решения поставленной
     * задачи по программированию.
     */

    public struct CompilerResult
    {

        // Поле указывает, возникли ли ошибки при компиляции
        // пользовательской программы
        public bool HasErrors;

        // Поле хранит полный путь к запускаемому файлу
        // пользовательского решения поставленной задачи
        public string ExeFullname;
        
        // Поле хранит выходной поток компилятора для
        // пользовательского решения поставленной задачи
        public string CompilerMessage;

    }

    /*!
     * \brief
     * Класс, который хранит в себе поля и методы,
     * которые необходимы для работы модульных
     * компиляторов для различных языков программирования.
     */

    public class CompilerRefs
    {

        ///////////////////////////////////////////////////
        /// Функция, возвращающая сгенерированный путь
        /// к исполняемому файлу решения задачи.
        ///////////////////////////////////////////////////

        public string GenerateExeFileLocation(string srcFileLocation, string currentSubmissionId, string outFileExt = null)
        {

            //Получаем путь родительской директории файла исходного кода
            string parentDirectoryFullName = new FileInfo(srcFileLocation).DirectoryName + @"\";

            //Формируем начальный путь исполняемого файла
            string exePath = parentDirectoryFullName + 's' + currentSubmissionId;

            //В случае, если расширение исполняемого
            //файла в данной ОС не нулевое,
            //добавляем его к имени файла.
            if (!String.IsNullOrWhiteSpace(outFileExt))
                exePath += '.' + outFileExt;

            //Возвращаем результат
            return exePath;

        }

        ///////////////////////////////////////////////////
        /// Функция запускает компилятор определённого
        /// языка программирования с указанными
        /// параметрами. В аргументах получает полный
        /// путь к компилятору а также аргументы.
        /// Возвращает информацию о результате компиляции.
        ///////////////////////////////////////////////////

        public CompilerResult RunCompiler(string compilerFullName, string compilerArgs)
        {
            //Создаём новый экземпляр процесса компилятора
            Process cplProc = new Process();

            //Устанавливаем информацию о старте процесса
            ProcessStartInfo pStartInfo = new ProcessStartInfo(compilerFullName, compilerArgs)
            {
                //Никаких ошибок, я сказал!
                ErrorDialog = false,
                //Минимизируем его, ибо не достоен он почестей!
                WindowStyle = ProcessWindowStyle.Minimized,
                //Перехватываем выходной поток
                RedirectStandardOutput = true,
                //Перехватываем поток ошибок
                RedirectStandardError = true,
                //Для перехвата делаем процесс демоном
                UseShellExecute = false
            };

            //Устанавливаем информацию о старте процесса в дескриптор процесса компилятора
            cplProc.StartInfo = pStartInfo;

            //Запускаем процесс компилятора
            cplProc.Start();

            //Ожидаем завершение процесса компилятора
            cplProc.WaitForExit();

            //Получаем выходной поток компилятора
            string standartOutput = cplProc.StandardOutput.ReadToEnd();
            string standartError = cplProc.StandardError.ReadToEnd();

            //Если выходной поток компилятора пуст, заполняем его не нужным барахлом
            if (standartOutput.Length == 0)
                standartOutput = "SimplePM_Server";

            //Объявляем переменную результата компиляции
            CompilerResult result = new CompilerResult()
            {
                //Получаем результат выполнения компилятора и записываем
                //его в переменную сообщения компилятора
                CompilerMessage = HttpUtility.HtmlEncode(standartOutput + "\n" + standartError)
            };

            //Возвращаем результат компиляции
            return result;
        }

        ///////////////////////////////////////////////////
        /// Функция, завершающая генерацию результата
        /// компиляции пользовательского приложения.
        ///////////////////////////////////////////////////

        public CompilerResult ReturnCompilerResult(CompilerResult temporaryResult)
        {

            //Проверяем результат компиляции
            //на предопределённость наличия ошибки
            if (!temporaryResult.HasErrors)
            {
                //Проверяем на наличие исполняемого файла
                temporaryResult.HasErrors = !File.Exists(temporaryResult.ExeFullname);
            }

            //Возвращаем результат компиляции
            return temporaryResult;

        }

        ///////////////////////////////////////////////////

    }

}
