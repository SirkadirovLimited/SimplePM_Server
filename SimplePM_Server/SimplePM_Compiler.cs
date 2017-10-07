/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 license.
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
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string submissionId; //!< Идентификатор запроса
        private readonly string fileLocation; //!< Полный путь к файлу и его расширение
        private readonly IniData sConfig; //!< Дескриптор конфигурационного файла

        ///////////////////////////////////////////////////
        /// Функция-конструктор класса компиляции
        /// пользовательских решений задач по
        /// программированию.
        ///////////////////////////////////////////////////
        
        public SimplePM_Compiler(ref IniData sConfig, string submissionId, string fileLocation)
        {
            
            //Проверяем путь к исходному коду на ошибки
            if (string.IsNullOrEmpty(fileLocation) || string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
                throw new FileNotFoundException("File not found!", "fileLocation");

            //Присваиваем глобальным для класса переменным
            //значения локальных переменных конструктора класса
            this.sConfig = sConfig;
            this.submissionId = submissionId;
            this.fileLocation = fileLocation;

        }

        ///////////////////////////////////////////////////
        /// Функция, которая по enum-у выбирает и
        /// запускает определённый компилятор, а также
        /// возвращает результат компиляции.
        ///////////////////////////////////////////////////

        public static CompilerResult ChooseCompilerAndRun(SimplePM_Submission.SubmissionLanguage codeLang, SimplePM_Compiler compiler)
        {

            //В зависимости от языка программирования
            //запускаем определённый компилятор
            switch (codeLang)
            {

                /*   ДЛЯ РАБОТЫ ПРОГРАММЫ ТРЕБУЕТСЯ КОМПИЛЯЦИЯ   */
                case SimplePM_Submission.SubmissionLanguage.Freepascal:
                    //Запускаем компилятор Pascal
                    return compiler.StartFreepascalCompiler();

                case SimplePM_Submission.SubmissionLanguage.CSharp:
                    //Запускаем компилятор C#
                    return compiler.StartCSharpCompiler();

                case SimplePM_Submission.SubmissionLanguage.C:
                    //Запускаем компилятор C
                    return compiler.StartCCompiler();

                case SimplePM_Submission.SubmissionLanguage.Cpp:
                    //Запускаем компилятор C++
                    return compiler.StartCppCompiler();

                case SimplePM_Submission.SubmissionLanguage.Java:
                    //Запускаем компилятор Java
                    return compiler.StartJavaCompiler();

                /*   ДЛЯ РАБОТЫ ПРОГРАММЫ НЕ ТРЕБУЕТСЯ КОМПИЛЯЦИЯ   */
                case SimplePM_Submission.SubmissionLanguage.Lua:
                case SimplePM_Submission.SubmissionLanguage.Python:
                case SimplePM_Submission.SubmissionLanguage.PHP:

                    //Некоторым файлам не требуется компиляция
                    //но для обратной совместимости функцию вкатать нужно
                    return compiler.StartNoCompiler();

                /*   ЯЗЫК ПРОГРАММИРОВАНИЯ НЕ ПОДДЕРЖИВАЕТСЯ СИСТЕМОЙ   */
                default:

                    return new CompilerResult
                    {
                        HasErrors = true, //Хьюстон, у нас проблема!
                        CompilerMessage = "Language not supported by SimplePM!"
                    };
                
            }

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
            string exeLocation = GenerateExeFileLocation(fileLocation, submissionId, sConfig["UserProc"]["exeFileExt"]);

            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = RunCompiler(
                sConfig["Compilers"]["freepascal_location"],
                "-ve -vw -vi -vb \"" + fileLocation + "\""
            );

            //Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

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

            //Будущее местонахождение исполняемого файла
            string exeLocation = GenerateExeFileLocation(fileLocation, submissionId, "exe");

            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = RunCompiler(
                sConfig["Compilers"]["mcs_location"],
                fileLocation
            );

            //Передаём полный путь к исполняемому файлу
            result.ExeFullname = exeLocation;

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
            string exeLocation = GenerateExeFileLocation(fileLocation, submissionId, sConfig["UserProc"]["exeFileExt"]);

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
            string exeLocation = GenerateExeFileLocation(fileLocation, submissionId, sConfig["UserProc"]["exeFileExt"]);

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
                
                //Указываем полный путь к главному исполняемому файлу
                result.ExeFullname = fileInfo.DirectoryName + "\\MainClass.class";

                //Проверяем на существование главного класса
                if (!File.Exists(result.ExeFullname))
                    throw new FileNotFoundException();

                //Ошибок не найдено!
                result.HasErrors = false;

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

        ///////////////////////////////////////////////////

    }
}
