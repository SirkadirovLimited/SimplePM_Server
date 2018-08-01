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
using System.Threading;

namespace SimplePM_Server.Workers
{
    
    public partial class SWorker
    {

        public void RunServer()
        {
            
            /*
             * Объявляем переменную,  которая хранит количество
             * произведенных проверок на наличие необработанных
             * запросов  на  тестирование,   после  которых  не
             * производилась задержка.
             */

            uint rechecksCount = 0;
            
            /*
             * В бесконечном цикле опрашиваем базу данных
             * на наличие новых не обработанных  запросов
             * на тестирование решений задач.
             */

            while (true)
            {
                
                // Выполняем единственный шаг цикла
                ServerLoopStep();
                
                /*
                 * Проверяем, необходимо ли установить
                 * таймаут для ослабления  нагрузки на
                 * процессор, или нет.
                 */

                var tmpCheck = rechecksCount >= uint.Parse(
                    (string)(_serverConfiguration.submission.rechecks_without_timeout)
                );

                if (_aliveTestersCount < (sbyte)(_serverConfiguration.submission.max_threads) && tmpCheck)
                {

                    // Ожидание для уменьшения нагрузки на сервер
                    Thread.Sleep((int)(_serverConfiguration.submission.check_timeout));

                    // Обнуляем итератор
                    rechecksCount = 0;

                }
                else
                    rechecksCount++;

            }
            
            // ReSharper disable once FunctionNeverReturns

        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void ServerLoopStep()
        {
            
            // Продолжаем лишь тогда, когда имеется свободные места
            if (_aliveTestersCount >= (sbyte) (_serverConfiguration.submission.max_threads)) return;
            
            /*
             * Действия  необходимо   выполнять  в  блоке
             * обработки    непредвиденных    исключений,
             * так   как   при   выполнении   операций  с
             * удалённой  базой  данных  могут  возникать
             * непредвиденные ошибки,   которые не должны
             * повлиять   на    общую    стабильность   и
             * работоспособность сервер проверки решений.
             */
                
            try
            {
                    
                /*
                 * Инициализируем   новое   уникальное
                 * соединение с базой данных для того,
                 * чтобы не мешать остальным потокам.
                 */
                    
                var conn = GetNewMysqlConnection();
                    
                /*
                 * В случае успешного подключения к базе данных
                 * SimplePM  Server,  вызываем  метод,  который
                 * занимается поиском  и  дальнейшей обработкой
                 * пользовательских запросов на тестирование.
                 */
                
                if (conn != null)
                    new Thread(RunPreWaiter).Start(conn);
            }
                
            /*
             * В случае  обнаружения  каких-либо
             * ошибок, записываем их в лог-файл.
             */
                
            catch (Exception ex)
            {
                    
                // Записываем все исключения как ошибки в лог
                logger.Error(ex);
                    
            }

        }
        
    }
    
}