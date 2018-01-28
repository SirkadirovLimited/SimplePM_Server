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

//Основа
using System;
//Работа с файловой системой
using System.IO;
//Подключаем коллекции
using System.Collections.Generic;
//Работа с процессами
using System.Diagnostics;
//Безопасная передача паролей
using System.Security;
//Работа с потоками и задачами
using System.Threading;
using System.Threading.Tasks;
//Безопасность
using System.Web;
using CompilerBase;
//Подключение к MySQL серверу
using MySql.Data.MySqlClient;
//Парсер конфигурационного файла
using IniParser.Model;
//Журнал событий
using NLog;
//Для использования плюшек
using static SimplePM_Server.SimplePM_Submission;

namespace SimplePM_Server
{

    /*!
     * \brief
     * Класс тестирования пользовательских решений
     * задач по программированию
     */

    internal class SimplePM_Tester2
    {
        ///////////////////////////////////////////////////
        // РАЗДЕЛ ОБЪЯВЛЕНИЯ ГЛОБАЛЬНЫХ ПЕРЕМЕННЫХ
        ///////////////////////////////////////////////////

        /*!
            Объявляем переменную указателя на менеджер журнала собылий
            и присваиваем ей указатель на журнал событий текущего класса
        */
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly MySqlConnection connection; //!< Дескриптор соединения с БД
        private readonly SubmissionInfo.SubmissionInfo submissionInfo; //!< Информация о запросе на проверку
        private readonly string exeFileUrl; //!< Полный путь к исполняемому файлу
        private readonly string customTestInput; //!< Кастомный тест, введённый пользователем
        private readonly int problemId; //!< Идентификатор задачи
        private readonly int submissionId; //!< Идентификатор запроса на компиляцию
        private readonly int userId; //!< Идентификатор пользователя
        private readonly float problemDifficulty; //!<  Сложность задачи
        private IniData sConfig; //!<  Дескриптор конфигурационного файла сервера
        private IniData sCompilersConfig; //!<  Дескриптор конфигурационного файла модулей компиляции
        private List<ICompilerPlugin> _compilerPlugins; //!< Список загруженных модулей компиляторв

        ///////////////////////////////////////////////////
        /// Функция-конструктор класса тестирования
        /// пользовательских решений задач
        ///////////////////////////////////////////////////

        public SimplePM_Tester2(ref MySqlConnection connection, ref List<ICompilerPlugin> _compilerPlugins, ref string exeFileUrl, ref SubmissionInfo.SubmissionInfo submissionInfo, ref IniData sConfig, ref IniData sCompilersConfig)
        {

            // Database connection
            this.connection = connection;

            // Compiler plugins list
            this._compilerPlugins = _compilerPlugins;

            // Excutable file url
            this.exeFileUrl = exeFileUrl;

            // Submission information
            this.submissionInfo = submissionInfo;

            // Problem ID
            problemId = submissionInfo.ProblemId;

            // Submission ID
            submissionId = submissionInfo.SubmissionId;

            // Problem difficulty
            problemDifficulty = submissionInfo.ProblemDifficulty;

            // User ID
            userId = submissionInfo.UserId;

            // Configuration file reader pointer
            this.sConfig = sConfig;

            // Custom test checker
            customTestInput = submissionInfo.CustomTest;

        }

        #region ИСПОЛЬЗУЕМЫЕ ФУНКЦИИ

        ///////////////////////////////////////////////////
        /// Функция "нормализует" выходные данные потока
        /// для дальнейшего анализа.
        ///////////////////////////////////////////////////
        public static string GetNormalizedOutputText(StreamReader outputReader, bool normalize = true, int lengthLimit = 0)
        {
            
            // Создаём переменную, которая будет содержать весь выходной поток
            // авторского решения поставленной задачи
            string _output = "";

            // Объявляем переменную, которая будет так называемым
            // счётчиком количества символов в выходном потоке
            // пользовательского решения поставленной задачи
            int currentLength = 0;
            
            // Производим необходимые действия пока мы
            // не достигли конца выходного потока
            while (!outputReader.EndOfStream)
            {
                
                // Создаём временную переменную текущей строки вывода,
                // получаем содержимое текущей строки
                string curLine = outputReader.ReadLine();

                if (lengthLimit > 0)
                {

                    // Прибавляем к длине уже просмотренного
                    // входного потока длину текущей строки
                    currentLength += curLine.Length;

                    // Проверка на превышение лимита по длине выходного потока
                    if (currentLength > lengthLimit)
                        throw new SimplePM_Exceptions.OutputLengthLimitException();

                }

                // Убираем все конечные пробелы (если, конечно,
                // результат должен быть нормализирован)
                if (normalize)
                    curLine = curLine.TrimEnd(' ');
                
                // Дозаписываем данные в переменную выходного потока приложения
                _output += curLine + "\n";
                
            }

            // Удаляем последний искуственно
            // созданный перевод на новую строку
            // но только в случае, если размер строки больше 0.
            if (_output.Length > 0)
                _output = _output.Substring(0, _output.LastIndexOf('\n'));

            // Возвращаем результат нормализации
            return _output;

        }

        ///////////////////////////////////////////////////
        /// Функция определяет необходимые действия при
        /// запуске процесса пользовательского или
        /// авторского решения задачи.
        ///////////////////////////////////////////////////
        
        private void SetExecInfoByFileExt(ref ProcessStartInfo startInfo, string filePath = null, string codeLanguage = "unset")
        {

            // Проверяем путь к исполняемому файлу
            if (string.IsNullOrWhiteSpace(filePath))
                filePath = exeFileUrl;

            // Проверяем язык решения задачи
            if (codeLanguage == "unset")
                codeLanguage = submissionInfo.CodeLang;

            // Вызываем ассоциированный метод, который знает лучше, как это делать
            bool f = SimplePM_Compiler.GetCompPluginByProgLangName(
                ref _compilerPlugins,
                codeLanguage
            ).SetRunningMethod(
                ref sConfig,
                ref sCompilersConfig,
                ref startInfo,
                filePath
            );

            if (!f)
                throw new SimplePM_Exceptions.UnknownException();

        }

        ///////////////////////////////////////////////////
        /// Функция создаёт задание, которое через
        /// короткий промежуток времени проверяет
        /// процесс на превышение лимита процессорного
        /// времени, и в случае обнаружения такого
        /// превышения вызывает функцию (метод), который
        /// передаётся в аргументах данной функции.
        ///////////////////////////////////////////////////

        public static void ProcessorTimeLimitCheck(Process proc, Action doProcessorTimeLimit, int timeLimit)
        {

            //Создаём и сразу же запускаем новую задачу
            new Task(() =>
            {

                //Ловим непредвиденные исключения
                try
                {

                    //Повторяем действия пока процесс не закончил свою работу
                    while (!proc.HasExited)
                    {

                        //Очищаем кеш и получаем обновлённые значения
                        proc.Refresh();

                        //Обновляем переменную типа TimeSpan для удобства
                        TimeSpan pts = proc.TotalProcessorTime;

                        //Проверяем процесс на превышение лимита процессорного времени
                        bool checker = (int) Math.Round(pts.TotalMilliseconds) > timeLimit;

                        //В случае превышения лимита запускаем пользовательский метод
                        if (checker)
                            doProcessorTimeLimit();

                    }

                }
                catch (Exception)
                {
                    /* Deal with it */
                }

            }).Start();

        }

        ///////////////////////////////////////////////////
        /// Функция в зависимости от конфигурации сервера
        /// указывает объекту процесса, что инициатором
        /// его запуска должен являться либо другой
        /// пользователь, либо тот же, от имени которого
        /// запущен сервер проверки решений задач.
        ///////////////////////////////////////////////////

        private void SetProcessRunAs(ref Process proc)
        {

            //Проверяем, включена ли функция запуска
            //пользовательских программ от имени инного пользователя.
            //Если отключена - выходим.
            if (sConfig["RunAs"]["enabled"] != "true") return;

            /* Указываем, что будем запускать процесс от имени другого пользователя */
            proc.StartInfo.Verb = "runas";

            /* Передаём имя пользователя */
            proc.StartInfo.UserName = sConfig["RunAs"]["accountLogin"];

            /* Передаём, что необходимо вытянуть профайл из реестра */
            proc.StartInfo.LoadUserProfile = false;

            /* Передаём пароль пользователя */

            //Создаём защищённую строку
            SecureString encPassword = new SecureString();

            //Добавляем данные в защищённую строку
            foreach (char c in sConfig["RunAs"]["accountPassword"])
                encPassword.AppendChar(c);

            //Устанавливаем пароль пользователя
            proc.StartInfo.Password = encPassword;

        }
        
        #endregion

        ///////////////////////////////////////////////////
        /// Функция, отвечающая за отладочное тестирование
        /// пользовательских решений задач
        ///////////////////////////////////////////////////

        public void DebugTest()
        {

            ///////////////////////////////////////////////////
            // Выборка информации об авторском решении задачи
            ///////////////////////////////////////////////////

            // Запрос на выборку авторского решения из БД
            string querySelect = $@"
                SELECT 
                    `codeLang`, 
                    `code` 
                FROM 
                    `spm_problems_ready` 
                WHERE 
                    `problemId` = @problemId 
                ORDER BY 
                    `problemId` ASC 
                LIMIT 
                    1
                ;
            ";

            // Дескрипторы временных таблиц выборки из БД
            MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);

            // Параметры запроса
            cmdSelect.Parameters.AddWithValue("@problemId", problemId);

            // Чтение результатов запроса
            MySqlDataReader dataReader = cmdSelect.ExecuteReader();
            
            // Объявляем необходимые переменные
            byte[] authorProblemCode = null;
            string authorProblemCodeLanguage = "unset";

            // Читаем полученные данные
            while (dataReader.Read())
            {

                // Исходный код авторского решения
                authorProblemCode = (byte[])dataReader["code"];

                // Язык авторского решения
                authorProblemCodeLanguage = dataReader["codeLang"].ToString();

            }

            // Закрываем data reader
            dataReader.Close();

            /* ===== Проверка на наличие авторского решения задачи ==== */
            if (authorProblemCode.Length == 0 || authorProblemCode == null || authorProblemCodeLanguage == "unset")
                throw new SimplePM_Exceptions.AuthorSolutionNotFoundException();

            #region ПРОВЕРКА РЕШЕНИЯ

            ///////////////////////////////////////////////////
            // Скачивание и компиляция авторского решения
            ///////////////////////////////////////////////////

            // Определяем расширение файла
            string authorFileExt = "." + GetExtByLang(authorProblemCodeLanguage, ref _compilerPlugins);

            // Получаем случайный путь к директории авторского решения
            string tmpAuthorDir = sConfig["Program"]["tempPath"] + @"\author\" + Path.GetRandomFileName() + @"\";

            // Создаём папку текущего авторского решения задачи
            Directory.CreateDirectory(tmpAuthorDir);

            // Определяем путь хранения файла исходного кода вторского решения
            string tmpAuthorSrcLocation = tmpAuthorDir + "sa" + submissionId + authorFileExt;

            // Записываем исходный код авторского решения в заданный временный файл
            File.WriteAllBytes(tmpAuthorSrcLocation, authorProblemCode);

            // Устанавливаем его аттрибуты
            File.SetAttributes(tmpAuthorSrcLocation, FileAttributes.Temporary | FileAttributes.NotContentIndexed);

            // Инициализируем экземпляр класса компилятора
            SimplePM_Compiler compiler = new SimplePM_Compiler(
                ref sConfig,
                ref sCompilersConfig,
                ref _compilerPlugins,
                "a" + submissionId,
                tmpAuthorSrcLocation,
                authorProblemCodeLanguage
            );

            // Получаем структуру результата компиляции
            CompilerResult cResult = compiler.ChooseCompilerAndRun();

            // В случае возникновения ошибки при компиляции
            // авторского решения аварийно завершаем работу
            if (cResult.HasErrors)
                throw new FileLoadException(cResult.ExeFullname);

            string authorCodePath = cResult.ExeFullname;

            ///////////////////////////////////////////////////
            // Время запросов к базе данных системы
            ///////////////////////////////////////////////////

            // Запрос на выборку Debug Time Limit из базы данных MySQL
            querySelect = $@"
                    SELECT 
                        `debugTimeLimit` 
                    FROM 
                        `spm_problems` 
                    WHERE 
                        `id` = '{problemId}' 
                    ORDER BY 
                        `id` ASC 
                    LIMIT 
                        1
                    ;
                ";

            ulong debugTimeLimit = Convert.ToUInt64(new MySqlCommand(querySelect, connection).ExecuteScalar());

            // То же самое, только для MEMORY LIMIT
            querySelect = $@"
                    SELECT 
                        `debugMemoryLimit` 
                    FROM 
                        `spm_problems` 
                    WHERE 
                        `id` = '{problemId}' 
                    ORDER BY 
                        `id` ASC 
                    LIMIT 
                        1
                    ;
                ";

            ulong debugMemoryLimit = Convert.ToUInt64(new MySqlCommand(querySelect, connection).ExecuteScalar());

            #region PROCESS START INFO CONFIGURATION
            //Создаём новую конфигурацию запуска процесса
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                // Перенаправляем потоки
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,

                // Запрещаем показ приложения на экран компа
                ErrorDialog = false,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Minimized
            };

            #endregion

            ///////////////////////////////////////////////////
            // Раздел объявления необходимых переменных
            ///////////////////////////////////////////////////

            string authorOutput = "", userOutput = "";
            string userErrorOutput = null;
            char debugTestingResult = '+';
            int userProblemExitCode = 0;

            #region Запуск авторского решения

            ///////////////////////////////////////////////////
            // ЗАПУСК ПРОЦЕССА АВТОРСКОГО РЕШЕНИЯ
            ///////////////////////////////////////////////////

            // Объявляем дескриптор процесса
            Process authorProblemProc = new Process();

            // Указываем полный путь к исполняемому файлу
            startInfo.FileName = authorCodePath;

            // Устанавливаем информацию о запускаемом файле
            SetExecInfoByFileExt(ref startInfo, authorCodePath, authorProblemCodeLanguage);

            // Указываем интересующую нас конфигурацию тестирования
            authorProblemProc.StartInfo = startInfo;

            // Вызываем функцию управления, которая указывает,
            // от имени какого пользователя стоит запускать
            // пользовательский процесс.
            SetProcessRunAs(ref authorProblemProc);

            try
            {

                // Запускаем процесс
                authorProblemProc.Start();

                // Устанавливаем наивысший приоритет процесса
                authorProblemProc.PriorityClass = ProcessPriorityClass.Normal;

                // Инъекция входного потока
                authorProblemProc.StandardInput.WriteLine(customTestInput); // вставка текста
                authorProblemProc.StandardInput.Flush(); // запись в поток, очистка буфера
                authorProblemProc.StandardInput.Close(); // закрываем поток

            }
            catch (Exception)
            {

                // Пытаемся удалить папку, содержащую авторское решение задачи
                try { Directory.Delete(tmpAuthorDir, true); }
                catch (Exception) {  }

                // Выбрасываем исключение
                throw new SimplePM_Exceptions.AuthorSolutionRunningException();

            }
            
            // Ждём завершения, максимум X миллимекунд
            // Если процесс не завершил свою работу - убиваем его
            if (!authorProblemProc.WaitForExit(int.Parse(sConfig["UserProc"]["maxProcessTime"])))
                authorProblemProc.Kill();
            
            try
            {
                // Получаем обработанный выходной поток авторского решения
                authorOutput = GetNormalizedOutputText(authorProblemProc.StandardOutput);
            }
            catch (Exception)
            {
                throw new SimplePM_Exceptions.AuthorSolutionRunningException();
            }

            try
            {

                // Пытаемся удалить временные файлы
                // авторского решения поставленной задачи
                Directory.Delete(tmpAuthorDir, true);

            }
            catch (Exception)
            {
                /* Deal with it */
            }

            // Выбрасываем исключение в случае, когда авторское решение задачи
            // некорректно завершило свою работу и/или стандартный поток ошибок
            // авторского решения не пустой.
            if (authorProblemProc.ExitCode != 0 || authorProblemProc.StandardError.ReadToEnd().Length > 0)
                throw new SimplePM_Exceptions.AuthorSolutionRunningException();

            #endregion

            #region Запуск пользовательского решения

            ///////////////////////////////////////////////////
            // ЗАПУСК ПРОЦЕССА ПОЛЬЗОВАТЕЛЬСКОГО РЕШЕНИЯ
            ///////////////////////////////////////////////////
            
            // Объявляем дескриптор процесса
            Process userProblemProc = new Process();

            // Устанавливаем информацию о запускаемом файле
            SetExecInfoByFileExt(ref startInfo);

            // Указываем интересующую нас конфигурацию тестирования
            userProblemProc.StartInfo = startInfo;

            // Вызываем функцию управления, которая указывает,
            // от имени какого пользователя стоит запускать
            // пользовательский процесс.
            SetProcessRunAs(ref userProblemProc);

            try
            {

                // Запускаем процесс
                userProblemProc.Start();

                // Устанавливаем наивысший приоритет процесса
                userProblemProc.PriorityClass = ProcessPriorityClass.Normal;

                // Инъекция входного потока
                userProblemProc.StandardInput.WriteLine(customTestInput); // вставка текста
                userProblemProc.StandardInput.Flush(); // запись в поток, очистка буфера
                userProblemProc.StandardInput.Close(); // закрываем поток

            }
            catch (Exception ex)
            {

                // Произошла ошибка при выполнении программы
                // В этом, скорее всего, виноват компилятор.
                debugTestingResult = 'C';
                logger.Error(ex);

            }

            ///////////////////////////////////////////////////
            // КОНТРОЛЬ ИСПОЛЬЗУЕМОЙ ПРОЦЕССОМ ПАМЯТИ
            ///////////////////////////////////////////////////
            new Task(() =>
            {

                try
                {
                    while (!userProblemProc.HasExited)
                    {
                        // Очищаем кеш и получаем обновлённые значения
                        userProblemProc.Refresh();

                        // Проверка на превышение лимита памяти
                        if ((ulong)userProblemProc.PeakWorkingSet64 > debugMemoryLimit)
                        {
                            // Лимит памяти превышен
                            userProblemProc.Kill(); // завершаем работу процесса в принудительном порядке
                            debugTestingResult = 'M'; // устанавливаем преждевременный результат тестирования
                        }
                    }
                }
                catch (Exception) { }

            }).Start();

            ///////////////////////////////////////////////////
            // ОГРАНИЧЕНИЕ ИСПОЛЬЗУЕМОГО ПРОЦЕССОРНОГО ВРЕМЕНИ
            ///////////////////////////////////////////////////

            void OnProcessorTimeLimit()
            {
                userProblemProc.Kill();
                debugTestingResult = 'T';
                userOutput = "--- PROCESSOR TIME LIMIT ---";
            }

            ProcessorTimeLimitCheck(userProblemProc, OnProcessorTimeLimit, (int)debugTimeLimit);

            ///////////////////////////////////////////////////
            // ПОЛУЧЕНИЕ РЕЗУЛЬТАТОВ ТЕСТИРОВАНИЯ
            ///////////////////////////////////////////////////

            try
            {

                // Ждём завершения, максимум X миллимекунд
                // Если процесс не завершил свою работу - убиваем его
                if (!userProblemProc.WaitForExit(int.Parse(sConfig["UserProc"]["maxProcessTime"])))
                {
                    userProblemProc.Kill();
                    debugTestingResult = 'T';
                    userOutput = "--- PROGRAM TIME LIMIT ---";
                }

                //Console.WriteLine(userProblemProc.TotalProcessorTime);

                // Если всё хорошо, получаем обработанный выходной поток пользовательского решения
                // но только в случае, когда приложение завершило свою работу самопроизвольно.
                // Если всё плохо - делаем userOutput пустым
                try
                {
                    if (userOutput.Length == 0)
                        userOutput = GetNormalizedOutputText(userProblemProc.StandardOutput);
                }
                catch (Exception)
                {
                    userOutput = null;
                }

                // Получаем exitcode пользовательского приложения
                userProblemExitCode = userProblemProc.ExitCode;

                // Получаем выходной поток ошибок пользовательского решения
                userErrorOutput = userProblemProc.StandardError.ReadToEnd();

                // Устанавливаем результат отладочного тестирования.
                // В случае преждевременного результата ничего не делаем
                if (debugTestingResult == '+')
                {

                    debugTestingResult =
                    (
                        userProblemExitCode == 0 &&
                        authorOutput == userOutput &&
                        string.IsNullOrWhiteSpace(userErrorOutput)
                            ? '+' : '-'
                    );

                }

            }
            catch (Exception) { }

            #endregion

            ///////////////////////////////////////////////////
            // ПОЛУЧЕНИЕ ВЫХОДНОГО ПОТОКА
            // ПОЛЬЗОВАТЕЛЬСКОЙ ПРОГРАММЫ
            ///////////////////////////////////////////////////

            userOutput = HttpUtility.HtmlEncode(userOutput);

            ///////////////////////////////////////////////////
            // Отправка результатов тестирования в базу данных
            ///////////////////////////////////////////////////

            string queryUpdate = $@"
                UPDATE 
                    `spm_submissions` 
                SET 
                    `status` = 'ready', 
                    `errorOutput` = @errorOutput, 
                    `result` = @result, 
                    `b` = '0', 
                    `output` = @output, 
                    `exitcodes` = @exitcodes 
                WHERE 
                    `submissionId` = '{submissionId}' 
                LIMIT 
                    1
                ;
            ";

            // Создаём и инициализируем команду для сервера баз данных MySQL
            MySqlCommand sendResultCmd = new MySqlCommand(queryUpdate, connection);

            // Устанавливаем значения параметров
            sendResultCmd.Parameters.AddWithValue("@output", userOutput);
            sendResultCmd.Parameters.AddWithValue("@errorOutput", userErrorOutput);
            sendResultCmd.Parameters.AddWithValue("@result", debugTestingResult);
            sendResultCmd.Parameters.AddWithValue("@exitcodes", userProblemExitCode);

            // Выполняем запрос не требуя ответа
            sendResultCmd.ExecuteNonQuery();

            ///////////////////////////////////////////////////

            #endregion

        }

        ///////////////////////////////////////////////////
        /// Функция, отвечающая за релизное тестирование
        /// пользовательских решений задач
        ///////////////////////////////////////////////////

        public void ReleaseTest()
        {

            ///////////////////////////////////////////////////
            // ВРЕМЯ ИНТЕРЕСНЕНЬКИХ ЗАПРОСОВ К БАЗЕ ДАННЫХ
            // А ИМЕННО ВЫБОРКА ИЗ БД ТЕСТОВ ЗАДАЧИ
            ///////////////////////////////////////////////////

            // Запрос на выборку всех тестов данной программы из БД
            string querySelect = $@"
                SELECT 
                    * 
                FROM 
                    `spm_problems_tests`
                WHERE 
                    `problemId` = @problemId 
                ORDER BY 
                    `id` ASC
                ;
            ";

            // Дескрипторы временных таблиц выборки из БД
            MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);

            cmdSelect.Parameters.AddWithValue("@problemId", problemId);

            MySqlDataReader dataReader = cmdSelect.ExecuteReader();

            // Переменная результата выполнения всех тестов
            int _problemPassedTests = 0;

            // Объявляем словарь информации о тестах
            Dictionary<int, Dictionary<string, string>> testsInfo = new Dictionary<int, Dictionary<string, string>>();

            // Производим выборку полученных результатов
            // из временной таблицы на сервере MySQL
            int i = 1;

            while (dataReader.Read())
            {

                Dictionary<string, string> tmpDict = new Dictionary<string, string>
                {
                    {
                        "testId",
                        HttpUtility.HtmlDecode(dataReader["id"].ToString())
                    }, // Идентификатор теста

                    {
                        "input",
                        HttpUtility.HtmlDecode(dataReader["input"].ToString())
                    }, // Входной поток

                    {
                        "output",
                        HttpUtility.HtmlDecode(dataReader["output"].ToString())
                    }, // Выходной поток

                    {
                        "timeLimit",
                        HttpUtility.HtmlDecode(dataReader["timeLimit"].ToString())
                    }, // Лимит по времени

                    {
                        "memoryLimit",
                        HttpUtility.HtmlDecode(dataReader["memoryLimit"].ToString())
                    } // Лимит по памяти

                };

                // Добавляем в словарь
                testsInfo.Add(i, tmpDict);

                // Увеличиваем индекс текущего теста на единицу
                i++;

            }

            // Завершаем чтение потока
            dataReader.Close();

            ///////////////////////////////////////////////////
            // СОЗДАНИЕ ЭКЗЕМПЛЯРА ПРОЦЕССА
            ///////////////////////////////////////////////////

            #region PROCESS START INFO CONFIGURATION

            // Создаём новую конфигурацию запуска процесса
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {

                // Перенаправляем потоки
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,

                // Запрещаем исполнение приложения в графическом режиме
                ErrorDialog = false,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Minimized

            };

            // Устанавливаем информацию о запускаемом файле
            SetExecInfoByFileExt(ref startInfo);

            #endregion

            ///////////////////////////////////////////////////
            // РЕГИОН ТЕСТИРОВАНИЯ ПОЛЬЗОВАТЕЛЬСКОЙ ПРОГРАММЫ
            ///////////////////////////////////////////////////

            // Объявление необходимых переменных для тестирования
            // пользовательской программы
            // ulong testId; // идентификатор теста
            string input, output; // входной и выходной потоки теста
            int timeLimit; // лимит времени теста
            long memoryLimit; // лимит памяти теста
            string standartErrorOutputText = null; // переменная стандартного потока ошибок
            string _exitcodes = "|"; // переменная кодов выхода
            long exitcodesSum = 0;
            bool preResultGiven = false; // служебная переменная для определения предопределённого результата
            string[] testingResults = new string[testsInfo.Count]; // переменная, хранящая информацию о пройденных тестах

            for (i = 1; i <= testsInfo.Count; i++)
            {

                // Присвоение переменных данных теста для быстрого доступа
                // testId = ulong.Parse(testsInfo[i]["testId"]);
                input = testsInfo[i]["input"];
                output = testsInfo[i]["output"].Replace("\r\n", "\n");
                timeLimit = int.Parse(testsInfo[i]["timeLimit"]);
                memoryLimit = long.Parse(testsInfo[i]["memoryLimit"]);
                preResultGiven = false;
                testingResults[i - 1] = null;

                // Объявляем дескриптор процесса
                Process problemProc = new Process()
                {
                    // Указываем интересующую нас конфигурацию тестирования
                    StartInfo = startInfo
                };

                // Вызываем функцию управления, которая указывает,
                // от имени какого пользователя стоит запускать
                // пользовательский процесс.
                SetProcessRunAs(ref problemProc);

                ///////////////////////////////////////////////////
                // ЗАПУСК ПРОЦЕССА И ПЕРЕХВАТ I/O ПОТОКОВ
                ///////////////////////////////////////////////////

                try
                {

                    // Запускаем процесс
                    problemProc.Start();

                    // Устанавливаем наивысший приоритет процесса
                    problemProc.PriorityClass = ProcessPriorityClass.Normal;

                    // Инъекция входного потока
                    problemProc.StandardInput.WriteLine(input); // добавляем текст во входной поток
                    problemProc.StandardInput.Flush(); // производим запись во входной поток и последующую очистку буфера
                    problemProc.StandardInput.Close(); // закрываем запись входного потока

                }
                catch (Exception ex)
                {
                    
                    // Произошла ошибка при выполнении программы.
                    // Виноват, как всегда, компилятор!

                    testingResults[i-1] = "C";
                    preResultGiven = true;

                    logger.Error(ex);

                }
                
                ///////////////////////////////////////////////////
                // КОНТРОЛЬ ИСПОЛЬЗУЕМОЙ ПРОЦЕССОМ ПАМЯТИ
                ///////////////////////////////////////////////////
                
                new Task(() =>
                {

                    // Для обеспечения безопасности
                    try
                    {
                        while (!problemProc.HasExited)
                        {
                            // Очищаем кеш и получаем обновлённые значения
                            problemProc.Refresh();

                            // Проверка на превышение лимита памяти
                            if (problemProc.PeakWorkingSet64 <= memoryLimit) continue;

                            // Очищаем кеш и получаем обновлённые значения
                            problemProc.Refresh();

                            // Лимит памяти превышен
                            problemProc.Kill(); // завершаем работу процесса в принудительном порядке

                            testingResults[i - 1] = "M"; // устанавливаем преждевременный результат тестирования
                            preResultGiven = true; // указываем, что выдали преждевременный результат тестирования

                        }
                    }
                    catch (Exception)
                    {
                        /* Deal with it */
                    }

                }).Start();

                ///////////////////////////////////////////////////
                // ОГРАНИЧЕНИЕ ИСПОЛЬЗУЕМОГО ПРОЦЕССОРНОГО ВРЕМЕНИ
                ///////////////////////////////////////////////////

                void OnProcessorTimeLimit()
                {

                    // Убиваем процесс
                    problemProc.Kill();

                    // Предварительный результат появился
                    preResultGiven = true;

                    // Временной лимит превышен
                    testingResults[i - 1] = "T";

                }

                ProcessorTimeLimitCheck(problemProc, OnProcessorTimeLimit, timeLimit);

                ///////////////////////////////////////////////////
                // ОЖИДАНИЕ ЗАВЕРШЕНИЯ ПОЛЬЗОВАТЕЛЬСКОЙ ПРОГРАММЫ
                ///////////////////////////////////////////////////

                // Проверка на досрочный результат проверки
                if (!preResultGiven)
                {

                    // Ждём завершения, максимум X миллимекунд
                    if (!problemProc.WaitForExit(int.Parse(sConfig["UserProc"]["maxProcessTime"])))
                    {

                        // Процесс не завершил свою работу
                        // Исключение: времени не хватило!
                        problemProc.Kill();

                        // Методом научного тыка было выявлено, что необходимо 10 мс чтобы программа
                        // корректно завершила свою работу
                        Thread.Sleep(10);

                        // Устанавливаем результат теста
                        testingResults[i - 1] = "T";

                    }
                    else
                    {

                        // Проверка на "вшивость"
                        string currentErrors = problemProc.StandardError.ReadToEnd();
                        problemProc.StandardError.Close();

                        // Добавляем ошибки текущего теста в список всех ошибок
                        if (currentErrors.Length > 0)
                            standartErrorOutputText += currentErrors + '\n';

                        if (currentErrors.Length > 0)
                        {

                            // Ошибка при тесте!
                            testingResults[i - 1] = "E";

                        }
                        else
                        {

                            /* Ошибок при тесте не выявлено, но вы держитесь! */

                            // Объявляем переменную, которая будет хранить
                            // полученные выходные данные пользовательского
                            // решения поставленной задачи
                            string pOut = "";

                            // Читаем выходной поток приложения
                            try
                            {
                                pOut = GetNormalizedOutputText(problemProc.StandardOutput);
                            }
                            catch (Exception)
                            {
                                testingResults[i - 1] = "O";
                            }

                            // Устанавливаем результат прохождения теста
                            // (если, конечно, уже не вынесен предварительный результат)
                            if (testingResults[i - 1] == null)
                            {

                                if (problemProc.ExitCode != 0) // код выхода не нуль
                                    testingResults[i - 1] = "R";
                                else if (output == pOut) // выходные потоки равны
                                {
                                    testingResults[i - 1] = "+";
                                    _problemPassedTests++;
                                }
                                else // тест не пройден
                                    testingResults[i - 1] = "-";

                            }
                            
                        }

                    }

                    // Добавляем конечную палку для правильности
                    // последующих парсингов
                    _exitcodes += problemProc.ExitCode + "|";

                    // Вычисляем сумму абсолютных значений кодов выхода
                    // для последующей проверки на ошибочность решения

                    exitcodesSum += Math.Abs(problemProc.ExitCode);

                }

                ///////////////////////////////////////////////////
                // НА ВСЯКИЙ СЛУЧАЙ ПЫТАЕМСЯ ЗАВЕРШИТЬ РАБОТУ
                // ПРОЦЕССА ПОЛЬЗОВАТЕЛЬСКОЙ ПРОГРАММЫ
                ///////////////////////////////////////////////////
                try
                {
                    problemProc.Kill();
                }
                catch (Exception)
                {
                    /* Deal with it */
                }

            }

            ///////////////////////////////////////////////////
            // ГЕНЕРИРУЕМ РЕЗУЛЬТАТЫ ТЕСТИРОВАНИЯ
            ///////////////////////////////////////////////////

            // Объявляем переменную численного результата тестирования
            float _bResult;

            try
            {

                // Тестов нет, но вы держитесь!
                if (testsInfo.Count <= 0)
                {
                    testingResults = new[] {"C"};
                    _problemPassedTests = 0;
                }

                // Вычисляем полученный балл за решение задачи
                // на основе количества пройденных тестов

                int testsCount = (testsInfo.Count == 0 ? 1 : testsInfo.Count);

                // Вычисляем зачисляемые за решение задачи
                // баллы, метод вычисления выбираем в
                // зависимости от типа запроса

                if (exitcodesSum == 0)
                {

                    if (submissionInfo.OlympId > 0)
                        _bResult = (float) _problemPassedTests / testsCount * problemDifficulty;
                    else
                        _bResult = _problemPassedTests == testsCount ? problemDifficulty : 0;

                }
                else
                    _bResult = 0;

            }
            catch (Exception)
            {

                // Устанавливаем полученные пользователем баллы
                _bResult = 0;

                // Устанавливаем результаты тестирования
                testingResults = new[] { "-" };

            }

            ///////////////////////////////////////////////////
            // ОТПРАВКА РЕЗУЛЬТАТОВ ТЕСТИРОВАНИЯ В БД
            ///////////////////////////////////////////////////

            string queryUpdate = @"
                UPDATE 
                    `spm_submissions` 
                SET 
                    `status` = 'ready', 
                    `errorOutput` = @errorOutput, 
                    `result` = @result, 
                    `exitcodes` = @exitcodes, 
                    `b` = @b 
                WHERE 
                    `submissionId` = @submId 
                LIMIT 
                    1
                ;
            ";

            // Создаём запрос к базе данных MySql
            MySqlCommand cmdUpd = new MySqlCommand(queryUpdate, connection);

            // Безопасно добавляем отправляемые данные
            cmdUpd.Parameters.AddWithValue("@errorOutput", standartErrorOutputText);
            cmdUpd.Parameters.AddWithValue("@result", string.Join("", testingResults));
            cmdUpd.Parameters.AddWithValue("@exitcodes", _exitcodes);
            cmdUpd.Parameters.AddWithValue("@b", _bResult);
            cmdUpd.Parameters.AddWithValue("@submId", submissionId.ToString());

            // Выполняем запрос
            cmdUpd.ExecuteNonQuery();
            
            ///////////////////////////////////////////////////
            // РЕГИОН ГЕНЕРАЦИИ РЕЙТИНГА ПОЛЬЗОВАТЕЛЯ
            ///////////////////////////////////////////////////

            try
            {

                // Обновляем количество баллов и рейтинг пользователя
                // для этого вызываем пользовательские процедуры mysql,
                // созданные как раз для этих нужд
                // P.S. но делаем это в том случае, если попытка была
                // произведена не во время олимпиады и не во время урока
                if (submissionInfo.ClassworkId == 0 && submissionInfo.OlympId == 0)
                    new MySqlCommand($"CALL updateBCount({userId}); CALL updateRating({userId})", connection)
                        .ExecuteNonQuery();

            }
            catch (Exception)
            {
                /* Deal with it */
            }

            ///////////////////////////////////////////////////
            
        }

        ///////////////////////////////////////////////////

    }

}
