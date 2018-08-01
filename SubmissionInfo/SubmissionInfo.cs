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

namespace SubmissionInfo
{
    
    /// <summary>
    /// Класс, объекты которого служат для хранения информации
    /// о пользовательском запросе на тестирование.
    /// </summary>
    public class SubmissionInfo
    {
        
        /// <summary>
        /// Уникальный идентификатор пользовательского запроса на тестирование
        /// </summary>
        public uint SubmissionId { get; set; }
        
        /// <summary>
        /// Уникальный идентификатор пользователя, отправившего запрос
        /// </summary>
        public uint UserId { get; set; }
        
        /// <summary>
        /// Идентификатор связанной олимпиады
        /// </summary>
        /// <value>
        /// <c>0</c> в случае отсутствия привязки,
        /// положительное значение в другом случае.
        /// </value>
        public uint OlympId { get; set; }
        
        /// <summary>
        /// Запрошенный тип тестирования пользовательского решения
        /// </summary>
        /// <value>
        /// В большинстве случаев принимает значения
        /// <c>syntax</c>, <c>debug</c> или <c>release</c>.
        /// </value>
        public string TestType { get; set; }
        
        /// <summary>
        /// Пользовательский тест для данной задачи.
        /// Используется лишь в случае, когда значением
        /// поля <c>TestType</c> является <c>debug</c>.
        /// </summary>
        /// <seealso cref="TestType"/>
        public byte[] CustomTest { get; set; }
        
        /// <summary>
        /// Хранит информацию о пользовательском
        /// решении поставленной задачи.
        /// </summary>
        public SolutionInfo UserSolution { get; set; }

        /// <summary>
        /// Хранит информацию о задаче, для которой текущий
        /// пользователь с идентификатором <c>UserId</c>
        /// отправил собственное решение.
        /// </summary>
        public ProblemInfo ProblemInformation { get; set; }
        
        /// <summary>
        /// Тип оценивания пользовательского решения
        /// </summary>
        public string SolutionRatingType { get; set; }

    }

    public struct ProblemInfo
    {
        
        /// <summary>
        /// Уникальный идентификатор задачи
        /// </summary>
        public uint ProblemId { get; set; }
        
        /// <summary>
        /// Сложность данной задачи
        /// </summary>
        public uint Difficulty { get; set; }
        
        /// <summary>
        /// Указывает на то, следует ли очищать дополнительные символы в конце строк.
        /// </summary>
        public bool AdaptProgramOutput { get; set; }
        
        /// <summary>
        /// Хранит информацию об авторском
        /// решении поставленной задачи.
        /// </summary>
        public SolutionInfo AuthorSolution { get; set; }
        
    }

    public struct SolutionInfo
    {
        
        /// <summary>
        /// Исходный код решения
        /// </summary>
        public byte[] SourceCode { get; set; }
        
        /// <summary>
        /// Язык написания исходного кода решения
        /// </summary>
        public string ProgrammingLanguage { get; set; }
        
    }
    
}