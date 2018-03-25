/*
 * Copyright (C) 2018, Yurij Kadirov.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Yurij Kadirov
 * @Website: https://spm.sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

using System;
using System.IO;
using System.Web;
using System.Diagnostics;
using System.Reflection;

namespace CompilerBase
{
    
    /*
     * Класс, который хранит в себе поля и методы,
     * которые необходимы для работы модульных
     * компиляторов для различных языков программирования.
     */

    public class CompilerRefs
    {

        /*
         * Метод   генерирует   полный  путь  к
         * исполняемому файлу пользовательского
         * решения    поставленной   задачи   и
         * возвращает его как строку.
         */

        public string GenerateExeFileLocation(
            string srcFileLocation,
            string currentSubmissionId
        )
        {

            /*
             * Определяем платформу
             */

            int platform = (int)Environment.OSVersion.Platform;

            /*
             * В зависимости  от  текущей  платформы,
             * устанавливаем специфическое расширение
             * запускаемого файла.
             */

            string outFileExt = (platform == 4) || (platform == 6) || (platform == 128)
                ? string.Empty : ".exe";

            /*
             * Получаем путь родительской
             * директории файла исходного
             * кода.
             */

            var parentDirectoryFullName = new FileInfo(srcFileLocation).DirectoryName
                                          ?? throw new DirectoryNotFoundException(srcFileLocation + " parent");

            /*
             * Формируем начальный путь
             * исполняемого файла.
             */

            var exePath = Path.Combine(
                parentDirectoryFullName,
                's' + currentSubmissionId
            );

            /*
             * В  случае,  если  расширение
             * исполняемого  файла в данной
             * ОС не нулевое, добавляем его
             * к имени файла.
             */

            if (!string.IsNullOrWhiteSpace(outFileExt))
                exePath += outFileExt;

            /*
             * Возвращаем сформированный путь
             * к искомому исполняемому файлу.
             */

            return exePath;

        }

        /*
         * Метод запускает указанный компилятор
         * или интерпритатор с указанными аргу-
         * ментами   и   возвращает   результат
         * компиляции пользовательского решения
         * поставленной задачи.
         */

        public CompilerResult RunCompiler(string compilerFullName, string compilerArgs)
        {

            /*
             * Создаём новый экземпляр процесса компилятора
             */
            var cplProc = new Process();

            /*
             * Устанавливаем информацию о старте процесса
             */
            var pStartInfo = new ProcessStartInfo(compilerFullName, compilerArgs)
            {

                // Никаких ошибок, я сказал!
                ErrorDialog = false,

                // Минимизируем его, ибо не достоен он почестей!
                WindowStyle = ProcessWindowStyle.Minimized,

                // Перехватываем выходной поток
                RedirectStandardOutput = true,

                // Перехватываем поток ошибок
                RedirectStandardError = true,

                // Для перехвата делаем процесс демоном
                UseShellExecute = false

            };

            /*
             * Устанавливаем информацию о старте
             * процесса  в  дескриптор  процесса
             * компилятора.
             */
            cplProc.StartInfo = pStartInfo;

            // Запускаем процесс компилятора
            cplProc.Start();

            // Ожидаем завершение процесса компилятора
            cplProc.WaitForExit();

            /*
             * Осуществляем чтение выходных потоков
             * компилятора в специально созданные
             * для этого переменные.
             */

            var standartOutput = cplProc.StandardOutput.ReadToEnd();
            var standartError = cplProc.StandardError.ReadToEnd();

            /*
             * Если выходной поток компилятора
             * пуст,   заполняем его не нужным
             * барахлом.
             */

            if (standartOutput.Length == 0)
                standartOutput = "SimplePM Server v" + Assembly.GetExecutingAssembly().GetName().Version + " on " + Environment.OSVersion;

            /*
             * Объявляем переменную результата компиляции
             */

            var result = new CompilerResult
            {

                /*
                 * Получаем полный текст сообщений
                 * компилятора и записываем его в
                 * специально отведенную для этого
                 * переменную.
                 */

                CompilerMessage = HttpUtility.HtmlEncode(standartOutput + "\n" + standartError)

            };

            // Возвращаем результат компиляции
            return result;

        }

        /*
         * Метод, завершающий генерацию результата
         * компиляции пользовательского решения
         * поставленной задачи по программированию.
         */

        public CompilerResult ReturnCompilerResult(CompilerResult temporaryResult)
        {

            /*
             * Проверка  на   предопределение
             * наличия ошибок при компиляции.
             */

            if (!temporaryResult.HasErrors)
            {
                // Проверяем на наличие исполняемого файла
                temporaryResult.HasErrors = !File.Exists(temporaryResult.ExeFullname);
            }

            // Возвращаем результат компиляции
            return temporaryResult;

        }

    }

}