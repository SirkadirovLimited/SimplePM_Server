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

namespace SimplePM_Server
{
    class SimplePM_Compiler
    {
        //Объявление необходимых переменных
        private ulong submissionId; //идентификатор запроса
        private string fileLocation, fileExt; //полный путь к файлу и его расширение
        private IniData sConfig; //дескриптор конфигурационного файла

        public SimplePM_Compiler(IniData sConfig, ulong submissionId, string fileExt)
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
            //Создаём новый экземпляр процесса компилятора
            Process fpcProc = new Process();
            
            //Устанавливаем информацию о старте процесса
            ProcessStartInfo pStartInfo = new ProcessStartInfo(sConfig["Compilers"]["freepascal_location"], " -ve -vw -vi -vb " + fileLocation);
            //Никаких ошибок, я сказал!
            pStartInfo.ErrorDialog = false;
            //Минимизируем его, ибо не достоен он почестей!
            pStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            //Перехватываем выходной поток
            pStartInfo.RedirectStandardOutput = true;
            //Для перехвата делаем процесс демоном
            pStartInfo.UseShellExecute = false;

            //Устанавливаем информацию о старте процесса в дескриптор процесса компилятора
            fpcProc.StartInfo = pStartInfo;
            //Запускаем процесс компилятора
            fpcProc.Start();
            
            //Получаем выходной поток компилятора
            StreamReader reader = fpcProc.StandardOutput;

            //Ожидаем завершение процесса компилятора
            fpcProc.WaitForExit();

            //Объявляем переменную результата компиляции
            CompilerResult result = new CompilerResult();
            //Получаем результат выполнения компилятора и записываем
            //его в переменную сообщения компилятора
            result.compilerMessage = HttpUtility.HtmlEncode(reader.ReadToEnd());

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
