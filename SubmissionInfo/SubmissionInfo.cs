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