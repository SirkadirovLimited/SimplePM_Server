/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

using CompilerBase;

namespace SimplePM_Server.SimplePM_Tester
{
    
    internal partial class SimplePM_Tester
    {

        public ProgramTestingResult Syntax(CompilerResult compilingResult)
        {

            /*
             * Генерируем результат тестирования
             * и возвращаем его  как объект типа
             * ProgramTestingResult.
             */
            return new ProgramTestingResult(1)
            {

                /*
                 * Указываем информацию об единственном тесте,
                 * который сигнализирует об успешности выполнения
                 * процесса компиляции пользовательского решения.
                 */
                
                TestingResults =
                {
                    
                    [0] = new TestResult
                    {
                        
                        Output = compilingResult.CompilerMessage,
                        Result = (!compilingResult.HasErrors) ? '+' : '-',
                        UsedMemory = 0,
                        UsedProcessorTime = 0,
                        ExitCode = 0

                    }

                }

            };


        }

    }

}