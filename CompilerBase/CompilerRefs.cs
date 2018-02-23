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

using System.IO;
using System.Web;
using System.Diagnostics;

namespace CompilerBase
{
    
    /*!
     * \brief
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

        public string GenerateExeFileLocation(string srcFileLocation, string currentSubmissionId, string outFileExt = null)
        {

            /*
             * Получаем путь родительской
             * директории файла исходного
             * кода.
             */
            var parentDirectoryFullName = new FileInfo(srcFileLocation).DirectoryName + @"\";

            /*
             * Формируем начальный путь
             * исполняемого файла.
             */
            var exePath = parentDirectoryFullName + 's' + currentSubmissionId;

            /*
             * В  случае,  если  расширение
             * исполняемого  файла в данной
             * ОС не нулевое, добавляем его
             * к имени файла.
             */
            if (!string.IsNullOrWhiteSpace(outFileExt))
                exePath += '.' + outFileExt;

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
            // Создаём новый экземпляр процесса компилятора
            var cplProc = new Process();

            // Устанавливаем информацию о старте процесса
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

            // Устанавливаем информацию о старте процесса в дескриптор процесса компилятора
            cplProc.StartInfo = pStartInfo;

            // Запускаем процесс компилятора
            cplProc.Start();

            // Ожидаем завершение процесса компилятора
            cplProc.WaitForExit();

            // Получаем выходной поток компилятора
            var standartOutput = cplProc.StandardOutput.ReadToEnd();
            var standartError = cplProc.StandardError.ReadToEnd();

            // Если выходной поток компилятора пуст, заполняем его не нужным барахлом
            if (standartOutput.Length == 0)
                standartOutput = "SimplePM_Server";

            // Объявляем переменную результата компиляции
            var result = new CompilerResult()
            {
                // Получаем результат выполнения компилятора и записываем
                // его в переменную сообщения компилятора
                CompilerMessage = HttpUtility.HtmlEncode(standartOutput + "\n" + standartError)
            };

            // Возвращаем результат компиляции
            return result;
        }

        ///////////////////////////////////////////////////
        /// Функция, завершающая генерацию результата
        /// компиляции пользовательского приложения.
        ///////////////////////////////////////////////////

        public CompilerResult ReturnCompilerResult(CompilerResult temporaryResult)
        {

            // Проверяем результат компиляции
            // на предопределённость наличия ошибки
            if (!temporaryResult.HasErrors)
            {
                // Проверяем на наличие исполняемого файла
                temporaryResult.HasErrors = !File.Exists(temporaryResult.ExeFullname);
            }

            // Возвращаем результат компиляции
            return temporaryResult;

        }

        ///////////////////////////////////////////////////

    }

}