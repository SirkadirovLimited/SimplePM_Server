/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server
 * A part of SimplePM programming contests management system.
 *
 * Copyright 2017 Yurij Kadirov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Visit website for more details: https://spm.sirkadirov.com/
 */

using System;
using SubmissionInfo;
using System.Diagnostics;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SimplePM_Server
{

    public partial class SimplePM_Worker
    {

        /*
         * Функция обработки запросов на проверку решений
         */

        private void GetSubIdAndRunCompile(MySqlConnection conn)
        {

            /*
             * Создаём  новую  задачу  для  обеспечения
             * многопоточности сервера проверки решений
             */

            new Task(() =>
            {
                
                /*
                 * Блок обработки исключительных ситуаций
                 * необходим для обработки непредвиденных
                 * исключений в работе тестирующей систе-
                 * мы и логгинга событий в журнал.
                 */

                try
                {

                    /*
                     * Записываем в лог информацию о событии
                     */

                    logger.Trace(
                        "Starting submission query; Running threads: " +
                        _aliveTestersCount + " from " +
                        (ulong)_serverConfiguration.submission.max_threads
                    );

                    /*
                     * Создаём новый запрос к базе данных на
                     * выборку из неё информации  о  запросе
                     * на тестирование.
                     */

                    var sqlCmd = new MySqlCommand(
                        Properties.Resources.submission_query.Replace(
                            "@EnabledLanguages",
                            EnabledLangs
                        ),
                        conn
                    );

                    // Выполняем запрос к БД и получаем ответ
                    var dataReader = sqlCmd.ExecuteReader();

                    // Объявляем временную переменную, так называемый "флаг"
                    bool f;

                    /*
                     * Делаем различные проверки в безопасном контексте
                     */

                    lock (new object())
                    {

                        f = _aliveTestersCount >= (ulong)_serverConfiguration.submission.max_threads | !dataReader.Read();

                    }

                    /*
                     * Проверка на пустоту полученного результата
                     */

                    if (f)
                    {

                        // Закрываем чтение пустой временной таблицы
                        dataReader.Close();

                        // Закрываем соединение с БД
                        conn.Close();

                        // Очищаем не управляемую память
                        conn.Dispose();

                    }
                    else
                    {

                        /* 
                         * Запускаем   секундомер  для  того,
                         * чтобы определить время, за которое
                         * запрос на проверку  обрабатывается
                         * сервером проверки решений задач.
                         */

                        var sw = Stopwatch.StartNew();

                        /*
                         * Увеличиваем количество текущих соединений
                         */

                        lock (new object())
                        {

                            _aliveTestersCount++;

                        }

                        /*
                         * Объявляем объект, который будет хранить
                         * всю информацию об отправке и записываем
                         * в него только что полученные данные.
                         */

                        var submissionInfo = new SubmissionInfo.SubmissionInfo
                        {

                            /*
                             * Основная информация о запросе
                             */

                            SubmissionId = int.Parse(dataReader["submissionId"].ToString()),
                            UserId = int.Parse(dataReader["userId"].ToString()),

                            /*
                             * Привязка к соревнованию
                             */

                            OlympId = int.Parse(dataReader["olympId"].ToString()),

                            /*
                             * Тип тестирования и доплнительные поля
                             */

                            TestType = dataReader["testType"].ToString(),
                            CustomTest = (byte[])dataReader["customTest"],

                            /*
                             * Исходный код решения задачи
                             * и дополнительная информация
                             * о нём.
                             */

                            ProblemCode = (byte[])dataReader["problemCode"],
                            CodeLang = dataReader["codeLang"].ToString(),

                            /*
                             * Информация о задаче
                             */

                            ProblemInformation = new ProblemInfo
                            {

                                ProblemId = int.Parse(dataReader["problemId"].ToString()),
                                ProblemDifficulty = int.Parse(dataReader["difficulty"].ToString()),
                                AdaptProgramOutput = bool.Parse(dataReader["adaptProgramOutput"].ToString()),

                                ProblemRatingType = dataReader["judge"].ToString(),

                                AuthorSolutionCode = (byte[])dataReader["authorSolution"],
                                AuthorSolutionCodeLanguage = dataReader["authorSolutionLanguage"].ToString()

                            }

                        };

                        // Закрываем чтение временной таблицы
                        dataReader.Close();

                        /*
                         * Устанавливаем статус запроса на "в обработке"
                         */

                        var queryUpdate = $@"
                            UPDATE 
                                `spm_submissions` 
                            SET 
                                `status` = 'processing' 
                            WHERE 
                                `submissionId` = '{submissionInfo.SubmissionId}'
                            LIMIT 
                                1
                            ;
                            COMMIT;
                        ";

                        // Выполняем запрос к базе данных
                        new MySqlCommand(queryUpdate, conn).ExecuteNonQuery();

                        /*
                         * Зовём официанта-шляпочника
                         * уж он знает, что делать в таких
                         * вот неожиданных ситуациях.
                         */

                        new SimplePM_Waiter(
                            conn,
                            ref _serverConfiguration,
                            ref _compilerConfigurations,
                            submissionInfo
                        ).ServeSubmission();

                        /*
                         * Уменьшаем количество текущих соединений
                         * чтобы другие соединения были возможны.
                         */

                        lock (new object())
                        {
                            _aliveTestersCount--;
                        }

                        /*
                         * Останавливаем секундомер и записываем
                         * полученное значение в Debug log поток
                         */

                        sw.Stop();

                        // Выводим затраченное время на экран
                        logger.Trace("Submission checking time (ms): " + sw.ElapsedMilliseconds);

                        // Закрываем соединение с БД
                        conn.Close();

                        // Очищаем не управляемую память
                        conn.Dispose();

                    }

                }
                catch (Exception ex)
                {

                    /*
                     * Записываем информацию об ошибке в лог-файл
                     */

                    logger.Error(ex);

                    /*
                     * Пытаемся закрыть соединение с БД
                     */

                    try
                    {

                        // Закрываем соединение с БД
                        conn.Close();

                        // Очищаем не управляемую память
                        conn.Dispose();

                    }
                    catch
                    {
                        /* Никаких действий не предвидится */
                    }

                }

            }).Start();

        }

    }

}
