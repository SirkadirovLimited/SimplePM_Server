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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using CompilerBase;
using IniParser.Model;

namespace SimplePM_Server.SimplePM_Tester
{

    /*
     * Класс,  содержащий  методы для выполнения
     * единичного тестирования пользовательского
     * и авторского  решений  поставленных задач
     * по программированию.
     */

    internal class ProgramTester
    {

        #region Секция объявления глобальных переменных

        private          IniData               sConfig;                        // дескриптор конфигурационного файла
        private          IniData               sCompilersConfig;               // дескриптор конфигурационного файла модулей компиляции
        private          List<ICompilerPlugin> _compilerPlugins;               // список плагинов компиляторов
        private readonly string                _codeLanguage;                  // наименование языка программирования
        private readonly string                _programPath;                   // путь к исполняемому файлу
        private readonly string                _programArguments;              // аргументы запуска
        private readonly string                _programInputString;            // данные для инъекции во входной поток
        private readonly long                  _programMemoryLimit;            // лимит по памяти в байтах
        private readonly int                   _programProcessorTimeLimit;     // лимит по процессорному времени в миллисекундах
        private readonly int                   _outputCharsLimit;              // лимит по количеству символов в выходном потоке
        private          string                _programOutput = "";            // данные из выходного потока программы
        private          string                _programErrorOutput;            // данные из выходного потока ошибок программы
        private          Process               _programProcess;                // ссылка на дескриптор процесса
        private readonly bool                  _adaptOutput;                   // указывает, мягкая или строгая проверка требуется

        private          bool                  _testingResultReceived;         // указывает, есть ли результат тестирования
        private          char                  _testingResult;                 // результат тестирования
        
        private ProcessStartInfo programStartInfo = new ProcessStartInfo
        {

            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,

            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding  = Encoding.UTF8,
            
            UseShellExecute        = false,
            LoadUserProfile        = false,
            CreateNoWindow         = true,
            WindowStyle            = ProcessWindowStyle.Hidden,
            ErrorDialog            = false,

            Arguments              = "",
            FileName               = ""
            
        };

        #endregion

        public ProgramTester(
            ref IniData sConfig,
            ref IniData sCompilersConfig,
            ref List<ICompilerPlugin> _compilerPlugins,
            string codeLanguage,
            string path,
            string args,
            long memoryLimit = 0,
            int processorTimeLimit = 0,
            string input = null,
            int outputCharsLimit = 0,
            bool adaptOutput = true
        )
        {

            this.sConfig = sConfig;
            this.sCompilersConfig = sCompilersConfig;

            this._compilerPlugins = _compilerPlugins;

            _codeLanguage = codeLanguage;

            _programPath = path;
            _programArguments = args;

            _programMemoryLimit = memoryLimit;
            _programProcessorTimeLimit = processorTimeLimit;

            _programInputString = input;
            _outputCharsLimit = outputCharsLimit;

            _adaptOutput = adaptOutput;

        }

        #region Чекеры на достижение лимитов

        private void StartMemoryLimitChecker()
        {

            /*
             * Выполняем необходимые действия
             * лишь в том случае, когда лимит
             * по памяти активирован
             */
            if (_programMemoryLimit > 0)
            {

                /*
                 * Создаём  новую  задачу и
                 * ставим её на выполнение.
                 */
                new Task(() => {

                    // Защита от всевозможных исключений
                    try
                    {

                        /*
                         * Крутим цикл пока пользовательский
                         * процесс не завершится.
                         */
                        while (!_programProcess.HasExited)
                        {

                            // Удаляем весь кэш, связанный с компонентом
                            _programProcess.Refresh();

                            /*
                             * Проверяем  на  превышение  лимита
                             * и в случае обнаружения, "убиваем"
                             * процесс.
                             */
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

        }

        private void StartProcessorTimeLimitChecker()
        {

            /*
             * Выполняем необходимые действия
             * лишь  в  том   случае,   когда
             * лимит по процессорному времени
             * активирован
             */
            if (_programProcessorTimeLimit > 0)
            {

                /*
                 * Создаём  новую  задачу и
                 * ставим её на выполнение.
                 */
                new Task(() => {

                    // Защита от всевозможных исключений
                    try
                    {

                        /*
                         * Крутим цикл пока пользовательский
                         * процесс не завершится.
                         */
                        while (!_programProcess.HasExited)
                        {

                            // Удаляем весь кэш, связанный с компонентом
                            _programProcess.Refresh();

                            /*
                             * Проверяем  на  превышение  лимита
                             * и в случае обнаружения, "убиваем"
                             * процесс.
                             */
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

        }

        #endregion

        public Test RunTesting()
        {

            // Инициализация всего необходимого
            Init();

            // Запускаем пользовательский процесс
            _programProcess.Start();
            
            /*
             * Записываем входные
             * данные во  входной
             * поток.
             */
            WriteInputString();

            /*
             * Вызываем метод, запускающий
             * слежение за памятью.
             */
            StartProcessorTimeLimitChecker();

            /*
             * Вызываем метод, запускающий
             * слежение  за   процессорным
             * временем.
             */
            StartMemoryLimitChecker();

            /*
             * Ожидаем завершения
             * пользовательского
             * процесса.
             */
            _programProcess.WaitForExit();

            /*
             * Освобождаем все связанные
             * с процессом ресурсы.
             */
            _programProcess.Close();

            /*
             * Возвращаем промежуточный
             * результат тестирования.
             */
            return GenerateTestResult();

        }

        private void Init()
        {

            /*
             * Управление методом запуска
             * пользовательского процесса
             */

            // Устанавливаем вид запуска
            ProgramTestingFunctions.SetExecInfoByFileExt(
                ref sConfig,
                ref sCompilersConfig,
                ref _compilerPlugins,
                ref programStartInfo,
                _programPath,
                _programArguments,
                _codeLanguage
            );

            // Устанавливаем RunAs информацию
            ProgramTestingFunctions.SetProcessRunAs(
                ref sConfig,
                ref _programProcess
            );

            /*
             * Инициализация необходимых для
             * тестирования переменных
             */

            _programProcess = new Process
            {

                PriorityClass = ProcessPriorityClass.BelowNormal, // устанавливаем низкий приоритет
                PriorityBoostEnabled = false, // не даём возможности увеличивать приоритет

                StartInfo = programStartInfo, // устанавливаем информацию о программе
                EnableRaisingEvents = true, // указываем, что хотим обрабатывать события

            };
            
            /*
             * Добавляем обработчики для некоторых событий
             */
            _programProcess.OutputDataReceived += ProgramProcess_OutputDataReceived;
            _programProcess.Exited += ProgramProcess_Exited;

        }

        private void WriteInputString()
        {
            
            /*
             * Для обеспечения безопасности
             * отлавливаем все исключения.
             */
            try
            {

                // Записываем входные данные во входной поток
                _programProcess.StandardInput.Write(_programInputString);
                
                // Очищаем буферы
                _programProcess.StandardInput.Flush();

                // Закрываем входной поток
                _programProcess.StandardInput.Close();

            }
            catch (Exception)
            {

                try
                {

                    // Убиваем процесс
                    _programProcess.Kill();

                }
                catch (Exception)
                {

                    /* Deal with it */

                }
                finally
                {

                    // Указываем, что результат тестирования получен
                    _testingResultReceived = true;

                    // Указываем результат тестирования
                    _testingResult = 'I';

                }

            }

        }

        private Test GenerateTestResult()
        {

            return new Test
            {

                // Выходные данные
                ErrorOutput = _programErrorOutput,
                Output = (_adaptOutput) ? _programOutput.TrimEnd('\r', '\n') : _programOutput,

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

        #region Обработчики событий

        private void ProgramProcess_Exited(object sender, EventArgs e)
        {

            /*
             * Раздел объявления необходимых переменных
             */

            bool checker;

            /*
             * Проверка на использованную память
             */

            checker = !_testingResultReceived && _programMemoryLimit > 0 &&
                _programProcess.PeakWorkingSet64 > _programMemoryLimit;

            if (checker)
            {
                _testingResultReceived = true;
                _testingResult = 'M';
            }

            /*
             * Проверка достижения лимита по процессорному времени
             */

            checker = !_testingResultReceived && _programProcessorTimeLimit > 0 &&
                      Convert.ToInt32(
                          Math.Round(
                              _programProcess.TotalProcessorTime.TotalMilliseconds
                          )
                      ) >
                        _programProcessorTimeLimit;

            if (checker)
            {
                _testingResultReceived = true;
                _testingResult = 'T';
            }

            /*
             * Проверка на обнаружение Runtime-ошибок
             */

            checker = !_testingResultReceived &&
                _programProcess.ExitCode != 0 &&
                _programProcess.ExitCode != -1;

            if (checker)
            {
                _testingResultReceived = true;
                _testingResult = 'R';
            }

            /*
             * Проверка на наличие текста в выходном потоке ошибок
             */

            if (!_testingResultReceived)
            {

                // Читаем выходной поток ошибок
                _programErrorOutput = _programProcess.StandardError.ReadToEnd();

                // Проверка на наличие ошибок
                if (_programErrorOutput.Length > 0)
                {

                    _testingResultReceived = true;
                    _testingResult = 'E';

                }

            }

            /*
             * Если всё хорошо, возвращаем временный результат
             */
            if (!_testingResultReceived)
            {

                _testingResultReceived = true;
                _testingResult = Test.MiddleSuccessResult;

            }

        }

        private void ProgramProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            /*
             * Если результат тестирования уже
             * имеется, не стоит ничего делать
             */
            if (_testingResultReceived)
                return;
            
            /*
             * Проверка на превышение лимита вывода
             */

            if (_outputCharsLimit > 0 && _programOutput.Length + e.Data.Length > _outputCharsLimit)
            {

                // Указываем, что результаты проверки уже есть
                _testingResultReceived = true;

                // Указываем результат тестирования
                _testingResult = 'O';

                // Добавляем сообщение пояснения
                _programOutput = "=== OUTPUT CHARS LIMIT REACHED ===";

                // Завершаем выполнение метода
                return;

            }
            
            /*
             * В ином случае дозаписываем данные
             * в соответственную переменную
             */

            var adaptedString = (_adaptOutput) ? e.Data.Trim() : e.Data;

            _programOutput += (_programOutput.Length > 0) ? adaptedString : adaptedString + '\n';

        }

        #endregion

    }

}
