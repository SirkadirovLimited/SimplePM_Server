using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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

        //TODO: Написать отладку приложения
        public void DebugTest() { }

        public void ReleaseTest()
        {
            //Запрос на выборку из БД
            string querySelect = "SELECT * FROM `spm_problems_tests` WHERE `problemId` = '" + problemId.ToString() + "' ORDER BY `id` ASC;";

            //throw new Exception(exeFileUrl);

            //Дескрипторы временных таблиц выборки из БД
            MySqlCommand cmdSelect = new MySqlCommand(querySelect, connection);
            MySqlDataReader dataReader = cmdSelect.ExecuteReader();

            //Переменная результата выполнения всех тестов
            string _problemTestingResult = "";

            while (dataReader.Read())
            {
                //Записываем необходимые переменные, которые получили из БД
                ulong testId = ulong.Parse( HttpUtility.HtmlDecode( dataReader["id"].ToString() ) );
                string input = HttpUtility.HtmlDecode( dataReader["input"].ToString() );
                string output = HttpUtility.HtmlDecode( dataReader["output"].ToString() );

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
                problemProc.WaitForExit(200);

                if (!problemProc.HasExited)
                {
                    //Процесс не завершил свою работу
                    //Исключение: времени не хватило!
                    problemProc.Kill();
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
                        if (output == pOut)
                            _problemTestingResult += '+';
                        else
                            _problemTestingResult += '-';
                    }

                }
                //Завершаем чтение потока
                dataReader.Close();
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
