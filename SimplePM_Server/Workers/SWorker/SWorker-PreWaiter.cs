/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server is a part of software product "Automated
 * vefification system for programming tasks "SimplePM".
 *
 * Copyright 2018 Yurij Kadirov
 *
 * Source code of the product licensed under the Apache License,
 * Version 2.0 (the "License");
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
using MySql.Data.MySqlClient;

namespace SimplePM_Server.Workers
{
    
    public partial class SWorker
    {

        private void RunPreWaiter(object connObject)
        {

            // Объявляем отдельную переменную для удобства
            var conn = (MySqlConnection) connObject;

            try
            {

                logger.Trace(
                    "Starting submission query; Running threads: " +
                    _aliveTestersCount + " from " +
                    (ulong) (_serverConfiguration.submission.max_threads)
                );
                
                // Выполняем запрос к БД и получаем ответ
                var dataReader = new MySqlCommand(
                    Resources.submission_query.Replace(
                        "@EnabledLanguages",
                        _enabledLanguagesString
                    ),
                    conn
                ).ExecuteReader();

                // Объявляем временную переменную, так называемый "флаг"
                bool f;

                // Проверка на наличие свободных мест и на ошибки
                lock (new object())
                {

                    f = _aliveTestersCount >= (sbyte) (_serverConfiguration.submission.max_threads) |
                        !dataReader.Read();

                }

                /*
                 * Проверка на пустоту полученного результата
                 * или на переполнение очереди проверки.
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

                    // Создаём и запускаем секундомер
                    var sw = Stopwatch.StartNew();

                    lock (new object())
                    {

                        // Увеличиваем количество "живых" тестировщиков на единицу
                        _aliveTestersCount++;

                    }

                    // Получаем подробную информацию о пользовательском запросе на тестировании
                    var submissionInfo = new SubmissionInfo.SubmissionInfo
                    {

                        // Идентификатор запроса
                        SubmissionId = uint.Parse(dataReader["submissionId"].ToString()),

                        // Идентификатор пользователя
                        UserId = uint.Parse(dataReader["userId"].ToString()),

                        // Идентификатор связанного соревнования
                        OlympId = uint.Parse(dataReader["olympId"].ToString()),

                        // Тип отправки
                        TestType = dataReader["testType"].ToString(),

                        // Пользвательский тест
                        CustomTest = (byte[]) dataReader["customTest"],

                        // Информация о пользовательском решении
                        UserSolution = new SolutionInfo
                        {

                            // Исходный код решения
                            SourceCode = (byte[]) dataReader["problemCode"],

                            // Использованный язык программирования
                            ProgrammingLanguage = dataReader["codeLang"].ToString()

                        },

                        // Тип оценивания пользовательского решения
                        SolutionRatingType = dataReader["judge"].ToString(),

                        // Информация о задаче
                        ProblemInformation = new ProblemInfo
                        {

                            // Идентификатор задачи
                            ProblemId = uint.Parse(dataReader["problemId"].ToString()),

                            // Сложность задачи
                            Difficulty = uint.Parse(dataReader["difficulty"].ToString()),

                            // Указание на то, следует ли "адаптировать" выходные данные
                            AdaptProgramOutput = bool.Parse(dataReader["adaptProgramOutput"].ToString()),

                            // Информация об авторском решении
                            AuthorSolution = new SolutionInfo
                            {

                                // Исходный код
                                SourceCode = (byte[]) dataReader["authorSolution"],

                                // Использованный язык программирования
                                ProgrammingLanguage = dataReader["authorSolutionLanguage"].ToString()

                            }

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

                    // Запускаем метод обработки пользовательского решения
                    new SWaiter(conn, submissionInfo).ServeSubmission();

                    lock (new object())
                    {

                        // Уменьшаем количество "живых" тестировщиков на единицу
                        _aliveTestersCount--;

                    }

                    // Останавливаем секундомер
                    sw.Stop();

                    // Выводим затраченное время на экран
                    logger.Trace("Submission checking time (ms): " + sw.ElapsedMilliseconds);

                }

            }
            catch (Exception ex)
            {

                // Записываем информацию об ошибке в лог-файл
                logger.Error(ex);
                
            }
            finally
            {
                
                try
                {

                    // Закрываем соединение с БД
                    conn.Close();

                    // Очищаем не управляемую память
                    conn.Dispose();

                }
                catch { /* Никаких дополнительных действий не предусмотрено */ }
                
                // Задействуем сборщик мусора
                GC.Collect(0, GCCollectionMode.Forced);
                
            }

        }

    }
    
}