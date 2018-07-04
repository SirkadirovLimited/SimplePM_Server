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

using MySql.Data.MySqlClient;

namespace SimplePM_Server.Workers
{
    
    public partial class SWorker
    {
        
        private MySqlConnection StartMysqlConnection()
        {

            /*
             * Объявляем  переменную, которая  будет хранить
             * дескриптор соединения с базой данных системы.
             */

            MySqlConnection db = null;

            try
            {

                /*
                 * Подключаемся к базе данных на удалённом
                 * MySQL  сервере  и  получаем  дескриптор
                 * подключения к ней.
                 */

                db = new MySqlConnection(
                    $@"
                        server={_databaseConfiguration.hostname};
                        uid={_databaseConfiguration.username};
                        pwd={_databaseConfiguration.password};
                        database={_databaseConfiguration.basename};
                        Charset={_databaseConfiguration.mainchst};
                        SslMode={_databaseConfiguration.sslmode};
                    "
                );

                // Открываем соединение с БД
                db.Open();

            }
            catch (MySqlException ex)
            {
                
                // Записываем информацию об ошибке в лог-файл
                logger.Error("An error occured while trying to connect to MySQL server: " + ex);

            }

            // Возвращаем указатель на соединение с БД
            return db;

        }
        
    }
    
}