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
            
            // Start as many threads, as specified by user
            for (sbyte i = 0; i < (sbyte) (_serverConfiguration.submission.max_threads); i++)
            {

                // Run new starter thread, that starts 1/max_threads waiter threads
                new Thread(() =>
                {

                    // ReSharper disable once TooWideLocalVariableScope
                    // Declare new thread variable
                    Thread waiterThread;
                    
                    // Execute code infinitely
                    while (true)
                    {
                        
                        // Create new waiter thread
                        waiterThread = new Thread(RunPreWaiter);
                        
                        // Start newly created thread
                        waiterThread.Start(GetNewMysqlConnection());
                        
                        // Wait for started thread to end
                        waiterThread.Join();
                        
                        // This thread must sleep for check_timeout ms to minimize CPU usage
                        Thread.Sleep((int)(_serverConfiguration.submission.check_timeout));
                        
                        // Call garbage collector
                        GC.Collect(1, GCCollectionMode.Optimized);
                        GC.Collect(2, GCCollectionMode.Optimized);
                        
                    }

                }).Start();

            }

        }
        
    }
    
}