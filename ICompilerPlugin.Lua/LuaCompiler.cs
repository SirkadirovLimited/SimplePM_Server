using System;
using System.Collections.Generic;
using System.Diagnostics;
using CompilerBase;

namespace LuaCompiler
{
    public class Lua : ICompilerPlugin
    {

        private const string _progLang = "lua";
        private const string _displayName = "SimplePM Lua";
        private const string _author = "Kadirov Yurij";
        private const string _supportUrl = "https://spm.sirkadirov.com/";

        public string CompilerPluginLanguageName => _progLang;
        public string CompilerPluginDisplayName => _displayName;
        public string CompilerPluginAuthor => _author;
        public string CompilerPluginSupportUrl => _supportUrl;

        public CompilerResult StartCompiler(Dictionary<string, string> sConfig, string submissionId, string fileLocation)
        {

            return new CompilerResult();

        }

        public void SetRunningMethod(ref ProcessStartInfo startInfo, string filePath)
        {
            
            throw new NotImplementedException();

        }

    }
}
