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

namespace SProgramRunner
{
    
    public partial class SRunner
    {

        private void ExecuteLimitsChecker()
        {

            // Create a new limits checker thread
            var limitsCheckerThread = new Thread(InfiniteCheckForLimitsReached);
            
            try
            {

                // Start newly created limits checker thread
                limitsCheckerThread.Start();
                
                // Get new values if only process is running now
                while (!_process.HasExited)
                {

                    try
                    {
                        
                        // Clear all cache associated with testing program process
                        _process.Refresh();

                        // Get new peak working set value
                        _programRunningResult.ProcessResourcesUsageStats.PeakUsedWorkingSet =
                            _process.PeakWorkingSet64;

                        // Get total value of processor time, used by testing program process
                        _programRunningResult.ProcessResourcesUsageStats.UsedProcessorTime =
                            Convert.ToInt32(
                                Math.Round(
                                    _process.TotalProcessorTime.TotalMilliseconds,
                                    MidpointRounding.ToEven
                                )
                            );
                        
                    }
                    catch { /* No catching blocks required */ }
                    
                    Thread.Sleep(50);
                    
                }
                
                // Abort limits checker thread, because we don't neeed it anymore
                limitsCheckerThread.Abort();
                
            }
            catch { /* No catching blocks required */ }

            //========================================================================================================//
            // INLINE METHOD THAT INFINITELY CHECKS FOR LIMITS REACHED BY ASSOCIATED PROGRAM PROCESS                  //
            //========================================================================================================//
            
            void InfiniteCheckForLimitsReached()
            {

                if (!_testingRequestStuct.LimitsInfo.Enable)
                    return;
                
                while (true)
                {

                    // Variable for quick access to requested process resources limits
                    var limitsInfo = _testingRequestStuct.LimitsInfo;

                    /*
                     * Processor time limit reached checker
                     */
                    
                    var ptlimreached_checker = limitsInfo.ProcessorTimeLimit != -1 &&
                                               _programRunningResult.ProcessResourcesUsageStats.UsedProcessorTime >
                                                    limitsInfo.ProcessorTimeLimit;

                    if (ptlimreached_checker)
                        SetNewTestingResult(TestingResult.TimeLimitResult);
                    
                    /*
                     * Working set limit reached checker
                     */
                    
                    var wslimreached_checker = limitsInfo.ProcessWorkingSetLimit != -1 &&
                                               _programRunningResult.ProcessResourcesUsageStats.PeakUsedWorkingSet >
                                                    limitsInfo.ProcessWorkingSetLimit;

                    if (wslimreached_checker)
                        SetNewTestingResult(TestingResult.MemoryLimitResult);
                    
                    Thread.Sleep(50);
                    
                }
                
            }
            
            //========================================================================================================//
            
        }
        
    }
    
}