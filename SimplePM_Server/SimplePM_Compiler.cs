﻿/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under CC BY-NC-SA 4.0 license.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */
/*! \file */

//Основа
using System;
//Работа с процессами
using System.Diagnostics;
//Конфигурационный файл
using IniParser.Model;
//Работа с файлами
using System.IO;
//Безопасность
using System.Web;
using System.Text;
//Журнал событий
using NLog;

namespace SimplePM_Server
{
    /*!
     * \brief
     * Класс компиляции пользовательских решений
     * задач по программированию. Функции в классе
     * вызывается функциями класса-официанта
     */

    class SimplePM_Compiler
    {
        ///////////////////////////////////////////////////
        // РАЗДЕЛ ОБЪЯВЛЕНИЯ ГЛОБАЛЬНЫХ ПЕРЕМЕННЫХ
        ///////////////////////////////////////////////////

        /*!
            Объявляем переменную указателя на менеджер журнала собылий
            и присваиваем ей указатель на журнал событий текущего класса
        */
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public ulong submissionId; //!< Идентификатор запроса
        public string fileLocation; //!< Полный путь к файлу и его расширение
        public IniData sConfig; //!< Дескриптор конфигурационного файла

        ///////////////////////////////////////////////////
        /// Функция-конструктор класса компиляции
        /// пользовательских решений задач по
        /// программированию.
        ///////////////////////////////////////////////////
        public SimplePM_Compiler(ref IniData sConfig, ulong submissionId, string fileLocation)
        {

            //Ещё кое-что проверяем на ошибки
            if (submissionId <= 0)
                throw new ArgumentNullException(nameof(submissionId));

            if (string.IsNullOrEmpty(fileLocation) || string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
                throw new FileNotFoundException("File not found!", "fileLocation");

            //Присваиваем глобальным для класса переменным
            //значения локальных переменных конструктора класса
            this.sConfig = sConfig;
            this.submissionId = submissionId;
            this.fileLocation = fileLocation;

        }

        ///////////////////////////////////////////////////
        /// \brief Класс результата компиляции.
        /// Используется для хранения и передачи
        /// информации о результате компиляции
        /// пользовательского решения поставленной задачи.
        ///////////////////////////////////////////////////

        public class CompilerResult
        {
            public bool HasErrors; //!< Найдены ли ошибки в синтаксисе
            public string ExeFullname; //!< Полное имя исполняемого файла
            public string CompilerMessage; //!< Сообщение компилятора
        }

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
        /// Компилятор языка программирования Pascal
        /// и его диалектов (Free Pascal, Object Pascal,
        /// Delphi, etc.)
        ///////////////////////////////////////////////////

        public CompilerResult StartFreepascalCompiler()
        {

            //Будущее местонахождение исполняемого файла
            string exeLocation = GenerateExeFileLocation(fileLocation, submissionId.ToString(), sConfig["UserProc"]["exeFileExt"]);

            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = RunCompiler(
                sConfig["Compilers"]["freepascal_location"],
                "-ve -vw -vi -vb \"" + fileLocation + "\""
            );

            //Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

            //Получаем полный путь к временному файлу, созданному при компиляции
            string oFileLocation = sConfig["Program"]["tempPath"] + submissionId + ".o";
            try
            {
                //Удаляем временный файл
                File.Delete(oFileLocation);
            }
            catch (Exception) { }

            //Возвращаем результат компиляции
            return ReturnCompilerResult(result);

        }

        ///////////////////////////////////////////////////
        /// Функция для некомпилируемых скриптов.
        /// Сама по себе является заглушкой-безделушкой.
        ///////////////////////////////////////////////////

        public CompilerResult StartNoCompiler()
        {
            //Делаем преждевременные выводы
            //прям как некоторые девушки
            //(по крайней мере на данный момент)

            CompilerResult result = new CompilerResult()
            {
                //ошибок нет - но вы держитесь
                HasErrors = false,
                //что дали - то и скинул
                ExeFullname = fileLocation,
                //хз зачем, но надо
                CompilerMessage = Properties.Resources.noCompilerRequired
            };

            //Возвращаем результат фальш-компиляции
            return result;
        }

        ///////////////////////////////////////////////////
        /// Функция, вызывающая компилятор языка
        /// программирования C# либо Mono/C# и
        /// возвращает результат компиляции
        /// пользовательской программы.
        ///////////////////////////////////////////////////

        public CompilerResult StartCSharpCompiler()
        {
            //Генерируем файл конфигурации MSBuild
            string msbuildConfigFileLocation = sConfig["Program"]["tempPath"] + submissionId + ".csproj";

            File.WriteAllText(
                //путь к записываемому файлу
                msbuildConfigFileLocation,
                //записываемые данные
                Properties.Resources.msbuild_csharp_tpl
                .Replace(
                    "[SPM|SUBMISSION_FILE_NAME]",
                    submissionId.ToString()
                )
                .Replace(
                    "[SPM|TMP_PATH]",
                    sConfig["Program"]["tempPath"]
                ),
                //Юра любит UTF8. Будь как Юра.
                Encoding.UTF8
            );

            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = RunCompiler(
                sConfig["Compilers"]["msbuild_location"],
                msbuildConfigFileLocation
            );

            //Удаляем временные файлы
            try
            {
                File.Delete(msbuildConfigFileLocation);
            }
            catch (Exception) {  }

            //Возвращаем результат компиляции
            return ReturnCompilerResult(result);
        }

        ///////////////////////////////////////////////////
        /// Функция вызывает компилятор языка
        /// программирования C++ и возвращает результат
        /// компиляции пользовательской программы.
        ///////////////////////////////////////////////////

        public CompilerResult StartCppCompiler()
        {
            //Будущее местонахождение исполняемого файла
            string exeLocation = GenerateExeFileLocation(fileLocation, submissionId.ToString(), sConfig["UserProc"]["exeFileExt"]);

            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = RunCompiler(
                sConfig["Compilers"]["gpp_location"],
                fileLocation + " -o " + exeLocation
            );

            //Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

            //Возвращаем результат компиляции
            return ReturnCompilerResult(result);
        }

        ///////////////////////////////////////////////////
        /// Функция вызывает компилятор языка
        /// программирования C и возвращает результат
        /// компиляции пользовательской программы.
        ///////////////////////////////////////////////////

        public CompilerResult StartCCompiler()
        {
            //Будущее местонахождение исполняемого файла
            string exeLocation = GenerateExeFileLocation(fileLocation, submissionId.ToString(), sConfig["UserProc"]["exeFileExt"]);

            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = RunCompiler(
                sConfig["Compilers"]["gcc_location"],
                fileLocation + " -o " + exeLocation
            );

            //Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

            //Возвращаем результат компиляции
            return ReturnCompilerResult(result);
        }

        ///////////////////////////////////////////////////
        /// Функция вызывает компилятор языка
        /// программирования Java и возвращает результат
        /// компиляции пользовательской программы.
        /// (Находится на стадии реализации)
        ///////////////////////////////////////////////////

        public CompilerResult StartJavaCompiler()
        {
            //Получаем полный исходный код решения
            string fullSrc = File.ReadAllText(fileLocation);

            //Производим замену имён главного класса на технические
            fullSrc = fullSrc.Replace("MainClass", "s" + submissionId);

            //Записываем изменённый исходный код решения
            File.WriteAllText(fileLocation, fullSrc);

            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = RunCompiler(
                sConfig["Compilers"]["javac_location"],
                fileLocation
            );

            //Для отлавливания всевозможных ошибок
            //создаём их улавливатель.
            //Он также поможет отловить пользовательские
            //ошибки в связи с незнанием правил использования
            //автоматизированной системы проверки решений SimplePM.
            try
            {
                //Получаем информацию о файле исходного кода
                FileInfo fileInfo = new FileInfo(fileLocation);

                //Ищем все файлы с расширением class в текущей директории
                FileInfo[] filesSearch = fileInfo.Directory.GetFiles("*.class");

                //Если классов больше одного - значит пользователь
                //не читал правила работы с системой. Воспринимаем
                //это как ошибку при компиляции, выбрасывая исключение
                if (filesSearch.Length > 1)
                    throw new OverflowException();

                //Получаем полный путь к исполняемому файлу
                string exeFullName = filesSearch[0].FullName;

                //Присваиваем переменным класса результата компиляции
                //необходимые для тестирования значения.
                result.ExeFullname = exeFullName; // полный путь к исполняемому файлу
                result.HasErrors = false; // ошибок при компиляции не выявлено
            }
            catch (Exception)
            {
                //В случае любой ошибки считаем что она
                //произошла по прямой вине пользователя.
                result.HasErrors = true;
            }

            //Возвращаем результат компиляции
            return ReturnCompilerResult(result);

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
                standartOutput = Properties.Resources.consoleHeader;

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
    }
}
