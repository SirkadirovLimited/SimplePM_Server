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

using NLog;
using System;
using System.IO;
using System.Text;
using CompilerBase;
using ProgramTesting;
using System.Diagnostics;
using System.Threading.Tasks;

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

        /*
         * Объявляем переменную указателя
         * на менеджер  журнала собылий и
         * присваиваем  ей  указатель  на
         * журнал событий текущего класса
         */
        private static readonly Logger logger = LogManager.GetLogger("SimplePM_Tester.ProgramTester");

        private dynamic _compilerConfiguration; // конфигурация модулей компиляции
        private ICompilerPlugin _compilerPlugin; // компиляционные плагины (модули)

        private readonly string _programPath; // путь к исполняемому файлу
        private readonly string _programArguments; // аргументы запуска
        private readonly byte[] _programInputBytes; // данные для инъекции во входной поток

        private readonly long _programMemoryLimit; // лимит по памяти в байтах
        private readonly int _programProcessorTimeLimit; // лимит по процессорному времени в миллисекундах
        private readonly int _outputCharsLimit; // лимит по количеству символов в выходном потоке

        private readonly bool _adaptOutput; // указывает, мягкая или строгая проверка требуется

        private string _programOutput = ""; // данные из выходного потока программы
        private string _programErrorOutput; // данные из выходного потока ошибок программы

        private Process _programProcess; // ссылка на дескриптор процесса
        
        private bool _testingResultReceived; // указывает, есть ли результат тестирования
        private char _testingResult = TestResult.ServerErrorResult; // результат тестирования
        
        private ProcessStartInfo programStartInfo = new ProcessStartInfo
        {

            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,

            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding  = Encoding.UTF8,
            
            UseShellExecute = false,
            LoadUserProfile = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            ErrorDialog = false,

            Arguments = "",
            FileName = "",
            
        };

        #endregion

        #region Секция объявления переменных статистики процесса

        /*
         * Переменная хранит количество
         * использованной памяти.
         */
        private long UsedMemory;
        
        /*
         * Переменная  хранит  количество
         * использованного  процессорного
         * времени.
         */
        private int UsedProcessorTime;

        #endregion

        /*
         * Основной конструктор данного класса.
         */

        public ProgramTester(
            ref dynamic compilerConfiguration,
            ref ICompilerPlugin _compilerPlugin,
            string path,
            string args,
            long memoryLimit = 0,
            int processorTimeLimit = 0,
            byte[] input = null,
            int outputCharsLimit = 0,
            bool adaptOutput = true
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

        }

        #region Чекеры на достижение лимитов

        /*
         * Метод отвечает за запуск чекера  на
         * достижение лимита по использованной
         * памяти.
         */

        private void StartMemoryLimitChecker()
        {

            /*
             * Создаём  новую  задачу и
             * ставим её на выполнение.
             */

            new Task(() => {

                /*
                 * Защищаемся от всех возможных угроз
                 */

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

                        // Получаем текущее значение свойства
                        UsedMemory = _programProcess.PeakWorkingSet64;

                        /*
                         * Проверяем  на  превышение  лимита
                         * и в случае обнаружения, "убиваем"
                         * процесс.
                         */
                        
                        if (_programMemoryLimit > 0 && UsedMemory > _programMemoryLimit)
                        {

                            // Убиваем процесс
                            _programProcess.Kill();

                            /*
                             * Записываем   преждевременный   результат
                             * тестирования пользовательской программы.
                             */

                            _testingResultReceived = true;
                            _testingResult = TestResult.MemoryLimitResult;

                        }
                        
                    }

                }
                catch (Exception)
                {

                    /* Deal with it */

                }

            }).Start();

        }

        /*
         * Метод отвечает  за  запуск чекера на
         * достижение лимита по использованному
         * процессорному времени.
         */

        private void StartProcessorTimeLimitChecker()
        {

            /*
             * Создаём  новую  задачу и
             * ставим её на выполнение.
             */

            new Task(() => {

                /*
                 * Защищаемся от всех возможных угроз
                 */
                
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

                        // Получаем текущее значение свойства
                        UsedProcessorTime = Convert.ToInt32(
                            Math.Round(
                                _programProcess.TotalProcessorTime.TotalMilliseconds
                            )
                        );


                        /*
                         * Проверяем  на  превышение  лимита
                         * и в случае обнаружения, "убиваем"
                         * процесс.
                         */

                        if (_programProcessorTimeLimit > 0 && UsedProcessorTime > _programProcessorTimeLimit)
                        {

                            // Убиваем процесс
                            _programProcess.Kill();

                            /*
                             * Записываем   преждевременный   результат
                             * тестирования пользовательской программы.
                             */

                            _testingResultReceived = true;
                            _testingResult = TestResult.TimeLimitResult;

                        }

                    }

                }
                catch (Exception)
                {

                    /* Deal with it */

                }

            }).Start();

        }

        #endregion

        /*
         * Метод, ответственный за выполнение
         * тестирования      пользовательской
         * программы.
         */

        public TestResult RunTesting()
        {

            /*
             * Защищаемся от всех возможных угроз
             */
            
            try
            {

                // Инициализация всего необходимого
                Init();

                // Запись входных данных в файл
                WriteInputFile();

                /*
                 * Продолжаем тестирование лишь  в  случае
                 * отсутствия предопределённого результата
                 * тестирования.
                 */

                if (!_testingResultReceived)
                {

                    // Запускаем пользовательский процесс
                    _programProcess.Start();

                    // Сигнализируем о готовности чтения выходного потока
                    _programProcess.BeginOutputReadLine();

                    /*
                     * Записываем  входные
                     * данные  во  входной
                     * поток.
                     */

                    WriteInputString();

                    /*
                     * Вызываем метод, запускающий
                     * слежение  за   процессорным
                     * временем.
                     */
                    
                    StartProcessorTimeLimitChecker();

                    /*
                     * Вызываем метод, запускающий
                     * слежение за памятью.
                     */

                    StartMemoryLimitChecker();

                    /*
                     * Ожидаем завершения
                     * пользовательского
                     * процесса.
                     */

                    _programProcess.WaitForExit();

                    /*
                     * Формируем     промежуточный
                     * результат      тестирования
                     * пользовательской программы.
                     */

                    FormatTestResult();

                }
                
            }
            catch (Exception ex)
            {

                /*
                 * Записываем информацию об ошибке в лог-файл
                 */

                logger.Error("An exception catched while trying to test user's submission: " + ex);

                /*
                 * Создаём псевдорезультаты
                 * тестирования   пользова-
                 * тельской программы.
                 */

                _testingResultReceived = true;
                _testingResult = TestResult.ErrorOutputNotNullResult;

                /*
                 * Записываем    информацию  об  исключении
                 * в выходной поток ошибок пользовательской
                 * программы.
                 */

                _programErrorOutput = ex.ToString();

            }
            
            /*
             * Возвращаем промежуточный
             * результат тестирования.
             */

            return GenerateTestResult();

        }

        /*
         * Метод отвечает за инициализацию всех
         * необходимых  факторов   для   начала
         * тестирования        пользовательской
         * программы.
         */

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
        
        /*
         * Метод  несёт ответственность за генерацию
         * результатов тестирования пользовательской
         * программы.
         */

        private TestResult GenerateTestResult()
        {
            
            /*
             * Генерируем результат тестирования
             * пользовательской   программы   на
             * текущем тесте.
             */

            var result = new TestResult
            {

                // Выходные данные из стандартного потока
                ErrorOutput = _programErrorOutput,
                Output = Encoding.UTF8.GetBytes(
                    (_adaptOutput)
                        ? _programOutput.TrimEnd('\r', '\n')
                        : _programOutput
                ),

                // Результаты предварительного тестирования
                ExitCode = _programProcess.ExitCode,
                Result = _testingResult,

                // Информация об использовании ресурсов
                UsedMemory = UsedMemory,
                UsedProcessorTime = UsedProcessorTime

            };
            
            /*
             * Освобождаем все связанные
             * с процессом ресурсы.
             */

            _programProcess.Close();
            _programProcess.Dispose();

            /*
             * Возвращаем сгенерированный выше результат
             */

            return result;

        }

        #region Методы записи входных данных

        /*
         * Метод  несёт ответственность за ввод
         * входных данных в стандартный входной
         * поток пользовательской программы.
         */

        private void WriteInputString()
        {

            /*
             * Для обеспечения безопасности
             * отлавливаем все исключения.
             */

            try
            {

                // Записываем входные данные во входной поток
                _programProcess.StandardInput.Write(
                    Encoding.UTF8.GetString(
                        _programInputBytes
                    )
                );

                // Очищаем буферы
                _programProcess.StandardInput.Flush();

                // Закрываем входной поток
                _programProcess.StandardInput.Close();

            }
            catch (Exception)
            {

                /*
                 * Если что-то пошло не так,
                 * как планировалось ранее.
                 */

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
                    _testingResult = TestResult.InputErrorResult;

                }

            }

        }

        /*
         * Метод несёт ответственность за
         * запись   входных   данных   во
         * входной файл.
         */

        private void WriteInputFile()
        {

            /*
             * Для обеспечения безопасности выполняем все
             * действия  в  блоке  обработки  исключений.
             */
            
            try
            {
                
                /*
                 * Получаем полный путь к файлу с входными данными
                 */

                var inputFilePath = Path.Combine(
                    new FileInfo(_programPath).DirectoryName ?? throw new DirectoryNotFoundException(),
                    "input.txt"
                );

                /*
                 * Выполняем действия над файлом в синхронизируемом
                 * блоке команд для обеспечения  безопасности и для
                 * снжения нагрузки на накопитель.
                 */

                lock (new object())
                {

                    // Записываем данные в файл input.txt
                    File.WriteAllBytes(
                        inputFilePath,
                        _programInputBytes
                    );

                    // Указываем аттрибуты этого файла
                    File.SetAttributes(
                        inputFilePath,
                        FileAttributes.Temporary | FileAttributes.NotContentIndexed
                    );

                }
                
            }
            catch (Exception)
            {

                // Указываем, что результат тестирования получен
                _testingResultReceived = true;

                // Указываем результат тестирования
                _testingResult = TestResult.InputErrorResult;

            }

        }

        #endregion

        #region Обработчики событий
        
        /*
         * Метод отвечает за обработку выходного
         * потока пользовательской программы.
         */

        private void ProgramProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            /*
             * Если результат тестирования уже
             * имеется, не стоит ничего делать
             *
             * Если данные не получены, так же
             * не стоит ничего делать.
             */
            
            if (e.Data == null || _testingResultReceived)
                return;
            
            /*
             * Проверка на превышение лимита вывода
             */

            if (_outputCharsLimit > 0 && _programOutput.Length + e.Data.Length > _outputCharsLimit)
            {

                // Указываем, что результаты проверки уже есть
                _testingResultReceived = true;

                // Указываем результат тестирования
                _testingResult = TestResult.OutputErrorResult;

                // Добавляем сообщение пояснения
                _programOutput = "=== OUTPUT CHARS LIMIT REACHED ===";

                // Завершаем выполнение метода
                return;

            }

            /*
             * В ином случае дозаписываем данные
             * в соответственную переменную.
             */

            var adaptedString = (_adaptOutput)
                ? e.Data.Trim()
                : e.Data;

            _programOutput += adaptedString + '\n';

        }

        /*
         * Метод несёт ответственность за подготовку
         * к  формированию  результата  тестирования
         * пользовательской программы.
         */

        private void FormatTestResult()
        {

            /*
             * Проверка на использованную память
             */

            var checker = !_testingResultReceived && _programMemoryLimit > 0 &&
                           UsedMemory > _programMemoryLimit;

            if (checker)
            {

                _testingResultReceived = true;
                _testingResult = TestResult.MemoryLimitResult;

            }

            /*
             * Проверка достижения лимита по процессорному времени
             */

            checker = !_testingResultReceived && _programProcessorTimeLimit > 0 &&
                      UsedProcessorTime > _programProcessorTimeLimit;

            if (checker)
            {

                _testingResultReceived = true;
                _testingResult = TestResult.TimeLimitResult;

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
                _testingResult = TestResult.RuntimeErrorResult;

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
                    _testingResult = TestResult.ErrorOutputNotNullResult;

                }

            }

            /*
             * Если всё хорошо, возвращаем временный результат
             */

            if (!_testingResultReceived)
            {

                _testingResultReceived = true;
                _testingResult = TestResult.MiddleSuccessResult;

            }

        }

        #endregion

    }

}
