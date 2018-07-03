/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server is a part of software product "Automated
 * vefification system for programming tasks "SimplePM".
 *
 * Copyright 2018 Yurij Kadirov
 *
 * Source code of the product licensed under the Apache License,
 * Version 2.0 (the "License");
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

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;

namespace CompilerPlugin
{
    
    public class StandardCompilationMethods
    {
        
        public static string GenerateExeFileLocation(
            string srcFileLocation,
            string currentSubmissionId
        )
        {

            /*
             * В зависимости  от  текущей  платформы,
             * устанавливаем специфическое расширение
             * запускаемого файла.
             */

            var outFileExt = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? ".exe" : string.Empty;

            // Получаем путь родительской директории файла исходного кода
            var parentDirectoryFullName = new FileInfo(srcFileLocation).DirectoryName
                                          ?? throw new DirectoryNotFoundException(srcFileLocation + " parent");

            // Формируем начальный путь исполняемого файла
            var exePath = Path.Combine(
                parentDirectoryFullName,
                's' + currentSubmissionId
            );

            /*
             * В  случае,  если  расширение исполняемого  файла в данной
             * ОС не нулевое, добавляем его к имени файла.
             */

            if (!string.IsNullOrWhiteSpace(outFileExt))
                exePath += outFileExt;

            // Возвращаем сформированный путь
            return exePath;

        }

        public static CompilationResult RunCompiler(string compilerFullName, string compilerArgs)
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

            // Передаём стартовую информацию
            cplProc.StartInfo = pStartInfo;

            // Запускаем процесс компилятора
            cplProc.Start();

            // Ожидаем завершения процесса компилятора
            cplProc.WaitForExit();

            /*
             * Осуществляем чтение выходных потоков
             * компилятора в специально созданные
             * для этого переменные.
             */

            var standartOutput = cplProc.StandardOutput.ReadToEnd();
            var standartError = cplProc.StandardError.ReadToEnd();

            // Стандартный выход компилятора
            if (standartOutput.Length == 0)
                standartOutput = "SimplePM Server v" + Assembly.GetExecutingAssembly().GetName().Version + " on " + Environment.OSVersion;

            // Объявляем и инициализируем переменную результата компиляции
            var result = new CompilationResult
            {

                /*
                 * Получаем полный текст сообщений компилятора и записываем его в
                 * специально отведенную для этого переменную.
                 */

                CompilerOutput = HttpUtility.HtmlEncode(standartOutput + "\n" + standartError)

            };

            // Возвращаем результат компиляции
            return result;

        }

        public static CompilationResult ReturnCompilerResult(CompilationResult temporaryResult)
        {

            // Проверка на предопределение наличия ошибок при компиляции
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