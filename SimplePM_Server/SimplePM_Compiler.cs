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

namespace SimplePM_Server
{
    class SimplePM_Compiler
    {
        //Объявление необходимых переменных
        private ulong submissionId; //идентификатор запроса
        private string fileLocation; //полный путь к файлу и его расширение
        private IniData sConfig; //дескриптор конфигурационного файла

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

        /// <summary>
        /// Класс результата компиляции
        /// </summary>
        public class CompilerResult
        {
            public bool hasErrors = false;
            public string exe_fullname = null;
            public string compilerMessage = null;
        }

        /// <summary>
        /// Компилятор Free Pascal и других паскалеподобных существ
        /// </summary>
        /// <returns>Возвращает результат компиляции</returns>
        public CompilerResult startFreepascalCompiler()
        {
            //Запуск компилятора с заранее определёнными аргументами
            CompilerResult result = runCompiler(
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
            return returnCompilerResult(result);
        }

        public CompilerResult startLuaCompiler()
        {
            //Делаем преждевременные выводы
            //прям как некоторые девушки
            //ибо Lua файлы не нуждаются в компиляции
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

        public CompilerResult startCSharpCompiler()
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
            CompilerResult result = runCompiler(
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
            return returnCompilerResult(result);
        }

        private CompilerResult runCompiler(string compilerFullName, string compilerArgs)
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

        /// <summary>
        /// Функция, возвращающая информацию о результате компиляции
        /// </summary>
        /// <param name="temporaryResult">Переменная временного результата компиляции</param>
        /// <returns></returns>
        private CompilerResult returnCompilerResult(CompilerResult temporaryResult)
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
