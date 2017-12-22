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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CompilerBase;

namespace SimplePM_Server.SimplePM_Tester
{

    internal class ProgramTester
    {

        /* Начало секции объявления глобальных переменных */

        private readonly List<ICompilerPlugin> _compilerPlugins;               // список плагинов компиляторов
        private readonly string                _codeLanguage;                  // наименование языка программирования
        private readonly string                _programPath;                   // путь к исполняемому файлу
        private readonly string                _programArguments;              // аргументы запуска
        private readonly string                _programInputString;            // данные для инъекции во входной поток
        private readonly long                  _programMemoryLimit;            // лимит по памяти в байтах
        private readonly int                   _programProcessorTimeLimit;     // лимит по процессорному времени в миллисекундах
        private readonly int                   _outputCharsLimit;              // лимит по количеству символов в выходном потоке
        private          string                _programOutput;                 // данные из выходного потока программы
        private          string                _programErrorOutput;            // данные из выходного потока ошибок программы
        private          Process               _programProcess;                // ссылка на дескриптор процесса

        private          bool                  _TestingResultReceived = false; // указывает, есть ли результат тестирования
        private          char                  _testingResult;                 // результат тестирования
        
        private ProcessStartInfo programStartInfo = new ProcessStartInfo
        {

            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding  = Encoding.UTF8,

            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            RedirectStandardInput  = true,
            
            UseShellExecute        = false,
            LoadUserProfile        = false,
            CreateNoWindow         = true,
            WindowStyle            = ProcessWindowStyle.Hidden,
            ErrorDialog            = false,

            Arguments              = "",
            FileName               = ""
            
        };

        /* Конец секции объявления глобальных переменных */

        public ProgramTester(
            ref List<ICompilerPlugin> _compilerPlugins,
            string codeLanguage,
            string path,
            string args,
            long memoryLimit,
            int processorTimeLimit,
            string input = null,
            int outputCharsLimit = 0
        )
        {
            
            this._compilerPlugins = _compilerPlugins;

            _codeLanguage = codeLanguage;

            _programPath = path;
            _programArguments = args;

            _programMemoryLimit = memoryLimit;
            _programProcessorTimeLimit = processorTimeLimit;

            _programInputString = input;
            _outputCharsLimit = outputCharsLimit;

        }

        private void StartMemoryLimitChecker()
        {

            // Создаём новую задачу и ставим её на выполнение
            new Task(() => {

                // Защита от всевозможных исключений
                try
                {

                    // Крутим цикл пока пользовательский процесс не завершится
                    while (!_programProcess.HasExited)
                    {
                        
                        // Удаляем весь кэш, связанный с компонентом
                        _programProcess.Refresh();

                        // Проверяем на превышение лимита и в случае обнаружения, "убиваем" процесс
                        if (_programProcess.PeakWorkingSet64 > _programMemoryLimit)
                            _programProcess.Kill();

                    }

                }
                catch (Exception)
                {
                    
                    /* Deal with it */

                }

            }).Start();

        }

        private void StartProcessorTimeLimitChecker()
        {

            // Создаём новую задачу и ставим её на выполнение
            new Task(() => {

                // Защита от всевозможных исключений
                try
                {

                    // Крутим цикл пока пользовательский процесс не завершится
                    while (!_programProcess.HasExited)
                    {

                        // Удаляем весь кэш, связанный с компонентом
                        _programProcess.Refresh();

                        // Проверяем на превышение лимита и в случае обнаружения, "убиваем" процесс
                        if (Convert.ToInt32(Math.Round(_programProcess.TotalProcessorTime.TotalMilliseconds)) > _programProcessorTimeLimit)
                            _programProcess.Kill();

                    }

                }
                catch (Exception)
                {

                    /* Deal with it */

                }

            }).Start();

        }

        public Test RunTesting()
        {
            
            throw new NotImplementedException();

        }

        private void Init()
        {

            /* Инициализация необходимых для тестирования переменных */

            _programProcess = new Process
            {

                PriorityClass = ProcessPriorityClass.Normal, // устанавливаем стандартный приоритет
                PriorityBoostEnabled = false, // не даём возможности увеличивать приоритет

                StartInfo = programStartInfo, // устанавливаем информацию о программе
                EnableRaisingEvents = true // указываем, что хотим обрабатывать события

            };

            /* Добавляем обработчики для некоторых событий */
            _programProcess.OutputDataReceived += ProgramProcess_OutputDataReceived;
            _programProcess.Exited += ProgramProcess_Exited;

        }

        private Test GenerateTest()
        {

            return new Test
            {

                // Выходные данные
                ErrorOutput = _programErrorOutput,
                Output = _programOutput,

                // Результаты предварительного тестирования
                ExitCode = _programProcess.ExitCode,
                Result = _testingResult,

                // Информация об использовании ресурсов
                UsedMemory = _programProcess.PeakWorkingSet64,
                UsedProcessorTime = Convert.ToInt32(
                    Math.Round(
                        _programProcess.TotalProcessorTime.TotalMilliseconds
                    )
                )

            };

        }

        private void ProgramProcess_Exited(object sender, EventArgs e)
        {

            /* Раздел объявления необходимых переменных */

            bool checker;

            /* Проверка на использованную память */

            checker = !_TestingResultReceived && _programProcess.PeakWorkingSet64 > _programMemoryLimit;

            if (checker)
            {
                _TestingResultReceived = true;
                _testingResult = 'M';
            }

            /* Проверка достижения лимита по процессорному времени */

            checker = !_TestingResultReceived &&
                      Convert.ToInt32(
                          Math.Round(
                              _programProcess.TotalProcessorTime.TotalMilliseconds
                          )
                      ) >
                        _programProcessorTimeLimit;

            if (checker)
            {
                _TestingResultReceived = true;
                _testingResult = 'T';
            }

            /* Проверка на обнаружение Runtime-ошибок */

            checker = !_TestingResultReceived && _programProcess.ExitCode != 0;

            if (checker)
            {
                _TestingResultReceived = true;
                _testingResult = 'R';
            }

            /* Проверка на наличие текста в выходном потоке ошибок */

            if (!_TestingResultReceived)
            {

                // Читаем выходной поток ошибок
                string errorOutput = _programProcess.StandardError.ReadToEnd();

                // Проверка на наличие ошибок
                if (errorOutput.Length > 0)
                {

                    _TestingResultReceived = true;
                    _testingResult = 'E';

                }

            }

            /* Если всё хорошо, возвращаем временный результат */
            if (!_TestingResultReceived)
            {

                _TestingResultReceived = true;
                _testingResult = '*';

            }

        }

        private void ProgramProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            /* Если результат тестирования уже имеется, не стоит ничего делать */
            if (_TestingResultReceived)
                return;

            /* Проверка на превышение лимита вывода */

            if (_outputCharsLimit > 0 && _programOutput.Length + e.Data.Length > _outputCharsLimit)
            {

                // Указываем, что результаты проверки уже есть
                _TestingResultReceived = true;

                // Указываем результат тестирования
                _testingResult = 'O';

                // Добавляем сообщение пояснения
                _programOutput = "=== OUTPUT CHARS LIMIT REACHED ===";

                // Завершаем выполнение метода
                return;

            }

            /* В инном случае дозаписываем данные в соответственную переменную */

            _programOutput += e.Data;

        }

    }

}
