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

using MySql.Data.MySqlClient;

namespace SimplePM_Server.Workers
{
    
    public partial class SWorker
    {
        
        public static MySqlConnection GetNewMysqlConnection()
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
                        SslMode={_databaseConfiguration.sslmode};
                    "
                );

                // Открываем соединение с БД
                db.Open();

                // Устанавливаем стандартную кодировку
                new MySqlCommand("SET NAMES utf8;", db).ExecuteNonQuery();

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