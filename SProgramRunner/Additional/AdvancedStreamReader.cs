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

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SProgramRunner
{
    
    public class AdvancedStreamReader
    {
        
        private const int BufferSize = 4096;

        private Stream _source;
        private MemoryStream _outputMemoryStream;

        private CancellationTokenSource _cancellationToken;

        /// <summary>
        /// Advanced stream reader helps read data from
        /// basic stream and get it's output as bytes array.
        /// </summary>
        /// <param name="source">Source stream</param>
        public AdvancedStreamReader(Stream source)
        {
            
            // Store stream variable pointer localy
            _source = source;
            
            // Initialize new memory stream
            _outputMemoryStream = new MemoryStream();

        }

        /// <summary>
        /// Method that starts async stream reader.
        /// </summary>
        public void Run()
        {
            
            // Generate new cancellation token
            _cancellationToken = new CancellationTokenSource();
            
            // Run new output reading task
            new Task(async () =>
            {
                
                // Create and init new buffer to read output to
                var buffer = new byte[BufferSize];
                
                while (true)
                {
                    
                    // Check for pending cancellation requests
                    _cancellationToken.Token.ThrowIfCancellationRequested();
                    
                    // Get count of bytes
                    var count = await _source.ReadAsync(buffer, 0, BufferSize, _cancellationToken.Token);
                    
                    // Break cycle on stream end
                    if (count <= 0)
                        break;
                    
                    // Write data from buffer to output stream
                    await _outputMemoryStream.WriteAsync(buffer, 0, count, _cancellationToken.Token);
                    
                    // Flush output stream data
                    await _outputMemoryStream.FlushAsync(_cancellationToken.Token);
                    
                }
                
            }, _cancellationToken.Token).Start();
            
        }

        /// <summary>
        /// Cancel async stream reading if WIP and return
        /// source stream output data as bytes array.
        /// </summary>
        /// <returns>Stream output data as bytes array.</returns>
        public byte[] KillAndGet()
        {
            
            // Try to cancel reading task
            _cancellationToken.Cancel();
            
            // Return output data
            return _outputMemoryStream.ToArray();

        }
        
    }
    
}