using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CompilerBase
{

    public interface ICompilerPlugin
    {

        string CompilerPluginLanguageName { get; }
        string CompilerPluginDisplayName { get; }
        string CompilerPluginAuthor { get; }
        string CompilerPluginSupportUrl { get; }

        CompilerResult StartCompiler(Dictionary<string, string> sConfig, string submissionId, string fileLocation);

        void SetRunningMethod(ref ProcessStartInfo startInfo, string filePath);

    }

    public struct CompilerResult
    {

        public bool HasErrors;
        public string ExeFullname;
        public string CompilerMessage;

    }

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

    }

}
