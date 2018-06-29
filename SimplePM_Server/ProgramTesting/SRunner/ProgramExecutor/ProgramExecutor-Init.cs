using System.IO;
using System.Diagnostics;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        private void Init()
        {

            /*
             * Инициализация необходимых для
             * тестирования переменных
             */
            
            _programProcess = new Process
            {

                StartInfo = programStartInfo, // устанавливаем информацию о программе
                EnableRaisingEvents = true, // указываем, что хотим обрабатывать события

            };

            /*
             * Указываем текущую рабочую директорию.
             *
             * Это необходимо  по  большей части для
             * поддержки   запуска  пользовательских
             * программ от имени иного  от  текущего
             * пользователя.
             */

            _programProcess.StartInfo.WorkingDirectory =
                new FileInfo(_programPath).DirectoryName ?? throw new DirectoryNotFoundException();

            /*
             * Управление методом запуска
             * пользовательского процесса
             */

            // Устанавливаем вид запуска
            ProgramTestingFunctions.SetExecInfoByFileExt(
                ref _compilerConfiguration,
                ref _compilerPlugin,
                ref programStartInfo,
                _programPath,
                _programArguments
            );

            // Устанавливаем RunAs информацию
            ProgramTestingFunctions.SetProcessRunAs(
                ref _programProcess
            );
            
            /*
             * Добавляем обработчики для некоторых событий
             */
            _programProcess.OutputDataReceived += ProgramProcess_OutputDataReceived;

        }
        
    }
    
}