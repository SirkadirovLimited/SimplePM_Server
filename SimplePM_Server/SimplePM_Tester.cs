using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Runtime.InteropServices;

namespace SimplePM_Server
{
    class SimplePM_Tester
    {
        private MySqlConnection connection;
        private string exeFileUrl;
        private ulong problemId, submissionId;

        public SimplePM_Tester(MySqlConnection connection, string exeFileUrl, ulong problemId, ulong submissionId)
        {
            this.connection = connection;
            this.exeFileUrl = exeFileUrl;
            this.problemId = problemId;
            this.submissionId = submissionId;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        //TODO: Написать отладку приложения
        public void DebugTest() { }

        public void ReleaseTest()
        {
            //throw new Exception(exeFileUrl);
            //IntPtr t = new IntPtr();
            //Wow64DisableWow64FsRedirection(ref t);

            //Запрос на выборку из БД
            string querySelect = "SELECT * FROM `spm_problems_tests` WHERE `problemId` = '" + problemId.ToString() + "' ORDER BY `id` ASC;";

            //Дескрипторы временных таблиц выборки из БД
            MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);
            MySqlDataReader dataReader = cmdSelect.ExecuteReader();

            //Переменная результата выполнения всех тестов
            string _problemTestingResult = "";

            Dictionary<int, Dictionary<string, string>> testsInfo = new Dictionary<int, Dictionary<string, string>>();
            
            int i = 1;
            while (dataReader.Read())
            {
                Dictionary<string, string> tmpDict = new Dictionary<string, string>();

                //Test id
                tmpDict.Add(
                    "testId",
                    HttpUtility.HtmlDecode(dataReader["id"].ToString())
                );
                //Input
                tmpDict.Add(
                    "input",
                    HttpUtility.HtmlDecode(dataReader["input"].ToString())
                );
                //Output
                tmpDict.Add(
                    "output",
                    HttpUtility.HtmlDecode(dataReader["output"].ToString())
                );
                //Time limit
                tmpDict.Add(
                    "timeLimit",
                    HttpUtility.HtmlDecode(dataReader["timeLimit"].ToString())
                );

                //Add to library
                testsInfo.Add(i, tmpDict);

                i++;
            }

            //Завершаем чтение потока
            dataReader.Close();

            //Объявление необходимых переменных для тестирования
            //пользовательской программы
            ulong testId;
            string input, output;
            int timeLimit;

            for (i=1; i<=testsInfo.Count; i++)
            {
                //Присвоение переменных данных теста для быстрого доступа
                testId = ulong.Parse(testsInfo[i]["testId"]);
                input = testsInfo[i]["input"].ToString();
                output = testsInfo[i]["output"].ToString();
                timeLimit = int.Parse(testsInfo[i]["timeLimit"]);

                //Объявляем дескриптор процесса и его стартовой информации
                Process problemProc = new Process();
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

                //Указываем интересующую нас конфигурацию тестирования
                problemProc.StartInfo = startInfo;

                //Запускаем процесс
                problemProc.Start();

                //Инъекция входного потока
                problemProc.StandardInput.WriteLine(input);
                problemProc.StandardInput.Flush();
                problemProc.StandardInput.Close();

                //Ждём завершения, максимум X миллимекунд
                problemProc.WaitForExit(timeLimit);

                if (!problemProc.HasExited)
                {
                    //Процесс не завершил свою работу
                    //Исключение: времени не хватило!
                    problemProc.Kill();
                    Thread.Sleep(10);
                    _problemTestingResult += 'T';
                }
                else
                {
                    //Проверка на "вшивость"
                    if (problemProc.StandardError.ReadToEnd().Length > 0)
                    {
                        //Ошибка при тесте!
                        _problemTestingResult += 'E';
                    }
                    else
                    {
                        //Ошибок при тесте не выявлено, но вы держитесь!
                        string pOut = problemProc.StandardOutput.ReadToEnd();
                        //Добавляем результат
                        //Console.WriteLine("'" + pOut + "'");
                        if (output == pOut)
                            _problemTestingResult += '+';
                        else
                            _problemTestingResult += '-';
                    }

                }

            }

            
            //Тестов нет, но вы держитесь!
            if (_problemTestingResult.Length <= 0)
                _problemTestingResult = "+";
            
            //ОТПРАВКА РЕЗУЛЬТАТОВ ТЕСТИРОВАНИЯ НА СЕРВЕР БД
            //В ТАБЛИЦУ РЕЗУЛЬТАТОВ (`spm_submissions`)
            string queryUpdate = "UPDATE `spm_submissions` SET `status` = 'ready', `result` = '" + _problemTestingResult + "' WHERE `submissionId` = '" + submissionId.ToString() + "' LIMIT 1;";
            new MySqlCommand(queryUpdate, connection).ExecuteNonQuery();
        }
    }
}
