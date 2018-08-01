/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 * 
 * SimplePM Server is a part of software product "Automated
 * verification system for programming tasks "SimplePM".
 * 
 * Copyright (C) 2016-2018 Yurij Kadirov
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 * 
 * GNU Affero General Public License applied only to source code of
 * this program. More licensing information hosted on project's website.
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