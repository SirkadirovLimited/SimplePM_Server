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
                    
                var conn = StartMysqlConnection();
                    
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