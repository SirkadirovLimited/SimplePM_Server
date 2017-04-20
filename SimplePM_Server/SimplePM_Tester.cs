using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Web;
using System.Runtime.InteropServices;
using IniParser.Model;

namespace SimplePM_Server
{
    class SimplePM_Tester
    {
        private MySqlConnection connection;
        private string exeFileUrl, customTestInput;
        private ulong problemId, submissionId, userId;
        private float problemDifficulty;
        private IniData sConfig;

        /// <summary>
        /// Класс тестирования пользовательских решений
        /// </summary>
        /// <param name="connection">Дескриптор подключения к БД</param>
        /// <param name="exeFileUrl">Адрес исполняемого файла пользовательской программы</param>
        /// <param name="problemId">Идентификатор задачи</param>
        /// <param name="submissionId">Идентификатор запроса на проверку</param>
        /// <param name="problemDifficulty">Сложность задачи (для расчёта полученных баллов и рейтинга)</param>
        /// <param name="userId">Идентификатор пользователя, отправившего запрос на проверку</param>
        /// <param name="sConfig">Дескриптор файла конфигурации SimplePM_Server</param>
        public SimplePM_Tester(MySqlConnection connection, string exeFileUrl, ulong problemId, ulong submissionId, float problemDifficulty, ulong userId, IniData sConfig = null, string customTestInput = null)
        {
            this.connection = connection;
            this.exeFileUrl = exeFileUrl;
            this.problemId = problemId;
            this.submissionId = submissionId;
            this.problemDifficulty = problemDifficulty;
            this.userId = userId;
            this.sConfig = sConfig;
            this.customTestInput = customTestInput;
        }

        #region ИСПОЛЬЗУЕМЫЕ ФУНКЦИИ

        private string getNormalizedOutputText(StreamReader outputReader)
        {
            //Создаём переменную, которая будет содержать весь выходной поток
            //авторского решения поставленной задачи
            string _output = "";
            //Создаём временную переменную текущей строки вывода
            string curLine = "";

            while (!outputReader.EndOfStream)
            {
                //Получаем содержимое текущей строки
                curLine = outputReader.ReadLine();

                //Убираем переводы на новую строку
                curLine = curLine.Replace("\n", "");
                //Убираем все начальные и конечные пробелы
                curLine = curLine.Trim(' ');

                if (curLine.Length > 0)
                {
                    //Добавляем перевод на новую строку (если, конечно, это не первая строка)
                    if (_output.Length > 0)
                        curLine = "\n" + curLine;
                }

                //Дозаписываем данные в переменную выходного потока приложения
                _output += curLine;
            }

            //Убираем конечный перевод строки, он нам не нужен.
            //if (_output.EndsWith("\n"))
            //    _output = _output.Substring(0, _output.LastIndexOf("\n"));

            return _output;
        }

        #endregion

        #region РЕГИОН ДЕБАГГЕРНЫХ ФУНКЦИЙ И МЕТОДОВ

        public void DebugTest()
        {
            //Запрос на выборку авторского решения из БД
            string querySelect = "SELECT * FROM `spm_problems_ready` WHERE `problemId` = '" + problemId.ToString() + "' ORDER BY `id` ASC LIMIT 1;";

            //Дескрипторы временных таблиц выборки из БД
            MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);
            MySqlDataReader dataReader = cmdSelect.ExecuteReader();

            //Создаём словарь значений элемента авторского решения
            Dictionary<string, string> authorCodeInfo = new Dictionary<string, string>();
            //Читаем полученные данные
            while (dataReader.Read())
            {

                //Идентификатор авторского решения
                authorCodeInfo.Add(
                    "id",
                    HttpUtility.HtmlDecode(dataReader["id"].ToString())
                );
                //Идентификатор задачи
                authorCodeInfo.Add(
                    "problemId",
                    HttpUtility.HtmlDecode(dataReader["problemId"].ToString())
                );
                //Бинарный (исполняемый) код авторского решения
                authorCodeInfo.Add(
                    "code",
                    HttpUtility.HtmlDecode(dataReader["code"].ToString())
                );
            }

            if (authorCodeInfo.Count == 3)
            {
                //Полный путь к временному (исполняемому) файлу авторского решения программы
                string authorCodePath = sConfig["Program"]["tempPath"] + "authorCode_" + submissionId + ".exe";
                //Создаём файл и перехватываем поток инъекции в файл данных
                StreamWriter writer = File.CreateText(authorCodePath);
                //Запись бинарного кода программы в файл исполняемого авторского решения
                writer.WriteLine(authorCodeInfo["code"]);
                //Записы данных в файл
                writer.Flush();
                //Закрываем поток
                writer.Close();
                
                //Запрос на выборку Debug Time Limit из базы данных MySQL
                querySelect = "SELECT `debugTimeLimit` FROM `spm_problems` WHERE `id` = '" + problemId.ToString() + "' ORDER BY `id` ASC LIMIT 1;";
                ulong debugTimeLimit = (ulong)new MySqlCommand(querySelect, connection).ExecuteScalar();

                //То же самое, только для MEMORY LIMIT
                querySelect = "SELECT `debugMemoryLimit` FROM `spm_problems` WHERE `id` = '" + problemId.ToString() + "' ORDER BY `id` ASC LIMIT 1;";
                ulong debugMemoryLimit = (ulong)new MySqlCommand(querySelect, connection).ExecuteScalar();

                #region PROCESS START INFO CONFIGURATION
                //Создаём новую конфигурацию запуска процесса
                ProcessStartInfo startInfo = new ProcessStartInfo();

                //Перенаправляем потоки
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                //Запрещаем показ приложения на экран компа
                startInfo.ErrorDialog = false;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                #endregion

                /*
                 * ЗАПУСК ПРОЦЕССА АВТОРСКОГО РЕШЕНИЯ
                 */

                //Объявляем дескриптор процесса
                Process authorProblemProc = new Process();

                //Указываем полный путь к исполняемому файлу
                startInfo.FileName = authorCodePath;

                //Указываем интересующую нас конфигурацию тестирования
                authorProblemProc.StartInfo = startInfo;

                //Запускаем процесс
                authorProblemProc.Start();

                //Инъекция входного потока
                authorProblemProc.StandardInput.WriteLine(customTestInput);
                authorProblemProc.StandardInput.Flush();
                authorProblemProc.StandardInput.Close();

                //Проверяем процесс на использованную память

                new Thread(() =>
                {
                    while (!authorProblemProc.HasExited)
                    {
                        if ((ulong)authorProblemProc.PeakWorkingSet64 > debugMemoryLimit)
                            authorProblemProc.Kill();
                    }
                }).Start();

                //Ждём завершения, максимум X миллимекунд
                authorProblemProc.WaitForExit((int)debugTimeLimit);

                if (!authorProblemProc.HasExited)
                    authorProblemProc.Kill();

                string _authorOutput = getNormalizedOutputText(authorProblemProc.StandardOutput);

                /*
                 * ЗАПУСК ПРОЦЕССА ПОЛЬЗОВАТЕЛЬСКОГО РЕШЕНИЯ
                 */

                //Объявляем дескриптор процесса
                Process userProblemProc = new Process();

                //Указываем полный путь к исполняемому файлу
                startInfo.FileName = exeFileUrl;

                //Указываем интересующую нас конфигурацию тестирования
                userProblemProc.StartInfo = startInfo;

                //Запускаем процесс
                authorProblemProc.Start();

                //Инъекция входного потока
                userProblemProc.StandardInput.WriteLine(customTestInput);
                userProblemProc.StandardInput.Flush();
                userProblemProc.StandardInput.Close();

                //Проверяем процесс на использованную память

                new Thread(() =>
                {
                    while (!userProblemProc.HasExited)
                    {
                        if ((ulong)userProblemProc.PeakWorkingSet64 > debugMemoryLimit)
                            userProblemProc.Kill();
                    }
                }).Start();

                //Ждём завершения, максимум X миллимекунд
                userProblemProc.WaitForExit((int)debugTimeLimit);

                if (!userProblemProc.HasExited)
                    userProblemProc.Kill();

                string _userOutput = getNormalizedOutputText(userProblemProc.StandardOutput);

                //Пытаемся удалить временный файл авторского решения поставленной задачи
                try
                {
                    File.Delete(authorCodePath);
                }
                catch (Exception) { }
            }
            else
            {
                //Произошла ошибка, мы не в теме; Наша хата з краю, нічого не знаю!
                string queryUpdate = "UPDATE `spm_submissions` SET `status` = 'waiting', `hasError` = false WHERE `submissionId` = '" + submissionId + "' LIMIT 1;";
                new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
            }
        }

        #endregion

        #region РЕГИОН РЕЛИЗНЫХ ФУНКЦИЙ И МЕТОДОВ

        public void ReleaseTest()
        {
            //Запрос на выборку всех тестов данной программы из БД
            string querySelect = "SELECT * FROM `spm_problems_tests` WHERE `problemId` = '" + problemId.ToString() + "' ORDER BY `id` ASC;";

            //Дескрипторы временных таблиц выборки из БД
            MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);
            MySqlDataReader dataReader = cmdSelect.ExecuteReader();

            //Переменная результата выполнения всех тестов
            string _problemTestingResult = "";
            int _problemPassedTests = 0;

            Dictionary<int, Dictionary<string, string>> testsInfo = new Dictionary<int, Dictionary<string, string>>();
            
            int i = 1;
            while (dataReader.Read())
            {
                Dictionary<string, string> tmpDict = new Dictionary<string, string>();

                //Идентификатор теста
                tmpDict.Add(
                    "testId",
                    HttpUtility.HtmlDecode(dataReader["id"].ToString())
                );
                //Входной поток
                tmpDict.Add(
                    "input",
                    HttpUtility.HtmlDecode(dataReader["input"].ToString())
                );
                //Выходной поток
                tmpDict.Add(
                    "output",
                    HttpUtility.HtmlDecode(dataReader["output"].ToString())
                );
                //Лимит по времени
                tmpDict.Add(
                    "timeLimit",
                    HttpUtility.HtmlDecode(dataReader["timeLimit"].ToString())
                );
                //Лимит по памяти
                tmpDict.Add(
                    "memoryLimit",
                    HttpUtility.HtmlDecode(dataReader["memoryLimit"].ToString())
                );

                //Добавляем в словарь
                testsInfo.Add(i, tmpDict);

                i++;
            }

            //Завершаем чтение потока
            dataReader.Close();

            #region PROCESS START INFO CONFIGURATION
            //Создаём новую конфигурацию запуска процесса
            ProcessStartInfo startInfo = new ProcessStartInfo(exeFileUrl);

            //Перенаправляем потоки
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            //Запрещаем показ приложения на экран компа
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;
            #endregion

            //Объявление необходимых переменных для тестирования
            //пользовательской программы
            ulong testId;
            string input, output;
            int timeLimit;
            long memoryLimit;
            string standartErrorOutputText = null;

            for (i=1; i<=testsInfo.Count; i++)
            {
                //Присвоение переменных данных теста для быстрого доступа
                testId = ulong.Parse(testsInfo[i]["testId"]);
                input = testsInfo[i]["input"].ToString();
                output = testsInfo[i]["output"].ToString().Replace("\r\n", "\n");
                timeLimit = int.Parse(testsInfo[i]["timeLimit"]);
                memoryLimit = long.Parse(testsInfo[i]["memoryLimit"]);

                //Объявляем дескриптор процесса
                Process problemProc = new Process();

                //Указываем интересующую нас конфигурацию тестирования
                problemProc.StartInfo = startInfo;

                //Запускаем процесс
                problemProc.Start();

                //Инъекция входного потока
                problemProc.StandardInput.WriteLine(input);
                problemProc.StandardInput.Flush();
                problemProc.StandardInput.Close();

                //Проверяем процесс на использованную память

                //Ждём завершения, максимум X миллимекунд
                problemProc.WaitForExit(timeLimit);

                if (!problemProc.HasExited)
                {
                    //Процесс не завершил свою работу
                    //Исключение: времени не хватило!
                    problemProc.Kill();
                    //Методом научного тыка было выявлено, что необходимо 10 мс чтобы программа
                    //корректно завершила свою работу
                    Thread.Sleep(10);
                    _problemTestingResult += 'T';
                }
                else
                {
                    //Проверка на "вшивость"
                    string currentErrors = problemProc.StandardError.ReadToEnd();
                    problemProc.StandardError.Close();

                    //Добавляем ошибки текущего теста в список всех ошибок
                    if (currentErrors.Length > 0)
                        standartErrorOutputText += currentErrors;

                    if (currentErrors.Length > 0)
                    {
                        //Ошибка при тесте!
                        _problemTestingResult += 'E';
                    }
                    /*else if (problemProc.PeakVirtualMemorySize64 > memoryLimit)
                    {
                        //Процесс израсходовал слишком много физической памяти!
                        _problemTestingResult += 'M';
                    }*/
                    else
                    {
                        //Ошибок при тесте не выявлено, но вы держитесь!

                        //Читаем выходной поток приложения
                        string pOut = getNormalizedOutputText(problemProc.StandardOutput);

                        //Добавляем результат
                        if (output == pOut)
                        {
                            _problemTestingResult += '+';
                            _problemPassedTests++;
                        }
                        else
                            _problemTestingResult += '-';
                    }

                }

            }

            //Объявляем переменную численного результата тестирования
            float _bResult = 0;

            try
            {
                //Тестов нет, но вы держитесь!
                if (_problemTestingResult.Length <= 0)
                {
                    _problemTestingResult = "+";
                    _problemPassedTests = 1;
                }

                //Вычисляем полученный балл за решение задачи
                //на основе количества пройденных тестов
                _bResult = (_problemPassedTests / testsInfo.Count) * problemDifficulty;
            }
            catch (Exception) { _bResult = problemDifficulty; _problemTestingResult = "+"; }
            
            //ОТПРАВКА РЕЗУЛЬТАТОВ ТЕСТИРОВАНИЯ НА СЕРВЕР БД
            //В ТАБЛИЦУ РЕЗУЛЬТАТОВ (`spm_submissions`)
            string queryUpdate = "UPDATE `spm_submissions` SET `status` = 'ready'," +
                                                              "`errorOutput` = '" + standartErrorOutputText + "'," +
                                                              "`result` = '" + _problemTestingResult + "', " +
                                                              "`b` = '" + _bResult.ToString().Replace(',', '.') + "' " +
                                 "WHERE `submissionId` = '" + submissionId.ToString() + "' LIMIT 1;";
            new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
            //Обновляем количество баллов и рейтинг пользователя
            //для этого вызываем пользовательские процедуры mysql,
            //созданные как раз для этих нужд
            new MySqlCommand("CALL updateBCount(" + userId + ")", connection).ExecuteNonQuery(); //кол-во баллов
            new MySqlCommand("CALL updateRating(" + userId + ")", connection).ExecuteNonQuery(); //кол-во рейтинга
        }

        #endregion

    }
}
