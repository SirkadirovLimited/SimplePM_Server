using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CompilerPlugin;
using NLog;
using ProgramTestingAdditions;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public partial class ProgramExecutor
    {

        public ProgramExecutor(
            ref dynamic compilerConfiguration,
            ref ICompilerPlugin _compilerPlugin,
            string path,
            string args,
            long memoryLimit = 0,
            int processorTimeLimit = 0,
            byte[] input = null,
            int outputCharsLimit = 0,
            bool adaptOutput = true,
            int programRuntimeLimit = 0
        )
        {
            
            // Получаем конфигурации компиляторов
            _compilerConfiguration = compilerConfiguration;

            // Получаем список модулей компиляции
            this._compilerPlugin = _compilerPlugin;

            // Получаем полный путь к программе
            _programPath = path;

            // Получаем аргументы запуска программы
            _programArguments = args;

            // Получаем лимит по памяти
            _programMemoryLimit = memoryLimit;

            // Получаем лимит по процессорному времени
            _programProcessorTimeLimit = processorTimeLimit;

            // Получаем данные для входного потока
            _programInputBytes = input;

            // Получаем лимит по количеству данных в выходном потоке
            _outputCharsLimit = outputCharsLimit;

            // Узнаём, необходимо ли упрощать выходной поток
            _adaptOutput = adaptOutput;

            // Получаем лимит на время исполнения программы в миллисекундах
            _programRuntimeLimit = (programRuntimeLimit <= 0) ? processorTimeLimit * 5 : programRuntimeLimit;

        }

    }
    
}