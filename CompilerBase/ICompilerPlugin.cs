/*
 * Copyright (C) 2018, Yurij Kadirov.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Yurij Kadirov
 * @Website: https://spm.sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

using System.Diagnostics;

namespace CompilerBase
{

    /*
     * Интерфейс, который предоставляет возможность
     * создания собственных модулей компиляторов
     * для различных языков программирования.
     */
    
    public interface ICompilerPlugin
    {
        
        string PluginName { get; }
        string AuthorName { get; }
        string SupportUri { get; }

        CompilerResult StartCompiler(ref dynamic languageConfiguration, string submissionId, string fileLocation);
        
        bool SetRunningMethod(ref dynamic languageConfiguration, ref ProcessStartInfo startInfo, string filePath);

    }
    
}
