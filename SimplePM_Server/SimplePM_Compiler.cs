/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under CC BY-NC-SA 4.0 license.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

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
    class SimplePM_Compiler
    {
        ///////////////////////////////////////////////////
        // РАЗДЕЛ ОБЪЯВЛЕНИЯ ГЛОБАЛЬНЫХ ПЕРЕМЕННЫХ
        ///////////////////////////////////////////////////

        //Объявляем переменную указателя на менеджер журнала собылий
        //и присваиваем ей указатель на журнал событий текущего класса
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ulong submissionId; //идентификатор запроса
        private string fileLocation; //полный путь к файлу и его расширение
        private IniData sConfig; //дескриптор конфигурационного файла

        ///////////////////////////////////////////////////
        // КОНСТРУКТОР КЛАССА КОМПИЛЯТОРОВ
        ///////////////////////////////////////////////////

        public SimplePM_Compiler(ref IniData sConfig, ulong submissionId, string fileExt)
        {
            //Проверяем на ошибки
            if (string.IsNullOrEmpty(fileExt) || string.IsNullOrWhiteSpace(fileExt))
                throw new ArgumentNullException("fileExt", "File extension error!");

            //Устанавливаем полный путь программы
            fileLocation = sConfig["Program"]["tempPath"] + submissionId.ToString() + fileExt;

            //Ещё кое-что проверяем на ошибки
            if (submissionId <= 0)
                throw new ArgumentNullException("submissionId", "Submission ID invalid!");
            if (string.IsNullOrEmpty(fileLocation) || string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
                throw new ArgumentNullException("fileLocation", "File not found!");

            //Присваиваем глобальным для класса переменным
            //значения локальных переменных конструктора класса
            this.sConfig = sConfig;
            this.submissionId = submissionId;
        }

        ///////////////////////////////////////////////////
        // КЛАСС РЕЗУЛЬТАТА КОМПИЛЯЦИИ
        ///////////////////////////////////////////////////

        public class CompilerResult
        {
            public bool hasErrors = false;
            public string exe_fullname = null;
            public string compilerMessage = null;
        }

        ///////////////////////////////////////////////////
        // PASCAL COMPILER (MIXED)
        ///////////////////////////////////////////////////

        public CompilerResult StartFreepascalCompiler()
        {
            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = RunCompiler(
                sConfig["Compilers"]["freepascal_location"],
                "-Twin64 -ve -vw -vi -vb " + fileLocation
            );

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
        // СПЕЦИАЛЬНО ДЛЯ НЕКОМПИЛИРУЕМЫХ ЯЗЫКОВ
        // ПРОГРАММИРОВАНИЯ (ТАКИХ КАК LUA, PYTHON И ДР.)
        ///////////////////////////////////////////////////

        public CompilerResult StartNoCompiler()
        {
            //Делаем преждевременные выводы
            //прям как некоторые девушки
            //(по крайней мере на данный момент)

            CompilerResult result = new CompilerResult()
            {
                //ошибок нет - но вы держитесь
                hasErrors = false,
                //что дали - то и скинул
                exe_fullname = fileLocation,
                //хз зачем, но надо
                compilerMessage = Properties.Resources.noCompilerRequired
            };

            //Возвращаем результат фальш-компиляции
            return result;
        }

        ///////////////////////////////////////////////////
        // КОМПИЛЯТОР ДЛЯ ЯЗЫКА ПРОГРАММИРОВАНИЯ C#
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
                msbuildConfigFileLocation + ""
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
        // ФУНКЦИЯ ЗАПУСКА ПРОЦЕССА СТАНДАРТИЗИРОВАННОГО
        // КОНСОЛЬНОГО КОМПИЛЯТОРА (МУЛЬТИЮЗ)
        ///////////////////////////////////////////////////

        private CompilerResult RunCompiler(string compilerFullName, string compilerArgs)
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
                //Для перехвата делаем процесс демоном
                UseShellExecute = false
            };

            //Устанавливаем информацию о старте процесса в дескриптор процесса компилятора
            cplProc.StartInfo = pStartInfo;
            //Запускаем процесс компилятора
            cplProc.Start();

            //Получаем выходной поток компилятора
            StreamReader reader = cplProc.StandardOutput;

            //Ожидаем завершение процесса компилятора
            cplProc.WaitForExit();

            //Объявляем переменную результата компиляции
            CompilerResult result = new CompilerResult()
            {
                //Получаем результат выполнения компилятора и записываем
                //его в переменную сообщения компилятора
                compilerMessage = HttpUtility.HtmlEncode(reader.ReadToEnd())
            };

            return result;
        }

        ///////////////////////////////////////////////////
        // ОКОНЧАТЕЛЬНАЯ ВЫДАЧА РЕЗУЛЬТАТОВ КОМПИЛЯЦИИ
        ///////////////////////////////////////////////////

        private CompilerResult ReturnCompilerResult(CompilerResult temporaryResult)
        {
            //Получаем полный путь к исполняемому файлу
            string exeLocation = sConfig["Program"]["tempPath"] + submissionId.ToString() + ".exe";
            temporaryResult.exe_fullname = exeLocation;

            //Проверяем на наличие исполняемого файла
            if (File.Exists(exeLocation))
                temporaryResult.hasErrors = false;
            else
                temporaryResult.hasErrors = true;

            //Возвращаем результат компиляции
            return temporaryResult;
        }
    }
}
