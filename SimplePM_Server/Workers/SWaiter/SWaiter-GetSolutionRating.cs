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
using ProgramTestingAdditions;
using SimplePM_Server.Workers.Recourse;

namespace SimplePM_Server.Workers
{
    
    public partial class SWaiter
    {
        
        private float GetSolutionRating(ref ProgramTestingResult programTestingResult)
        {
            
            /*
             * Выполняем расчёт рейтинга
             * пользовательского решения
             * лишь  в  том случае, если
             * типом тестирования выбран
             * старый добрый "release".
             */
            
            if (_submissionInfo.TestType == "release")
            {

                /*
                 * Выполняем все действия в блоке
                 * обработки происходящих исключе
                 * ний,  чтобы избежать возможных
                 * "вылетов" сервера проверки реш
                 * ений во время проверки пользов
                 * ательских решений поставленных
                 * задач по программированию.
                 */

                try
                {

                    /*
                     * Получаем ссылку на объект
                     * модуля оценивания пользов
                     * ательских решений поставл
                     * енных задач по программир
                     * ованию.
                     */

                    var currentJudgePlugin = SJudge.GetJudgePluginByName(
                        _submissionInfo.ProblemInformation.ProblemRatingType
                    );

                    /*
                     * Выполняем генерацию оценочного
                     * рейтинга пользовательского реш
                     * ения  и  возвращаем полученный
                     * рейтинг.
                     */

                    return currentJudgePlugin.GenerateJudgeResult(
                               ref programTestingResult
                           ).RatingMult * _submissionInfo.ProblemInformation.ProblemDifficulty;

                }
                catch (Exception ex)
                {

                    /*
                     * В случае возникновения ошибки
                     * записываем информацию о ней в
                     * лог-файл,  дабы администратор
                     * системы мог узнать её причину
                     */

                    logger.Error(
                        "Error while generating rating for submission #" +
                        _submissionInfo.SubmissionId
                        + ": " + ex
                    );

                    // Считаем, что рейтинг - 0
                    return 0;

                }

            }

            /*
             * В других случаях низко оцениваем
             * старания пользователя решить эту
             * задачу, ведь нельзя же так!
             */

            return 0;

        }
        
    }
    
}