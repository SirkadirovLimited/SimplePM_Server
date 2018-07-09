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
             * Расчёт рейтинга производится лишь при release-тестировании
             * пользовательского решения поставленной задачи.
             */
            
            if (_submissionInfo.TestType == "release")
            {

                try
                {

                    // Получаем ссылку на плагин судьи и возвращаем сгенерированный рейтинг
                    return SJudge.GetJudgePluginByName(
                        _submissionInfo.SolutionRatingType
                    ).GenerateJudgeResult(
                               ref programTestingResult
                               ).RatingMult * _submissionInfo.ProblemInformation.Difficulty;

                }
                catch (Exception ex)
                {

                    // Записываем информацию об исключении в лог-файл
                    logger.Error(
                        "Error while generating rating for submission #" +
                        _submissionInfo.SubmissionId
                        + ": " + ex
                    );

                    // Считаем, что рейтинг - 0
                    return 0;

                }

            }

            // В другом случае возвращаем 0
            return 0;

        }
        
    }
    
}