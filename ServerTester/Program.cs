using System;
using CommandDotNet;
using CommandDotNet.Models;

namespace ServerTester
{

    public class Program
    {

        public static int Main(string[] args)
        {

            AppSettings settings = new AppSettings()
            {
                AllowArgumentSeparator = true,
                Case = Case.LowerCase,
                EnableVersionOption = true,
                ShowArgumentDetails = true,
                ThrowOnUnexpectedArgument = true
            };

            AppRunner<Tester> runner = new AppRunner<Tester>(settings);

            return runner.Run(args);

        }

    }

}
