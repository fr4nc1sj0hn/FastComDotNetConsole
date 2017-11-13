using fast_com;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Fast_Com
{
    class Program
    {
        static void Main(string[] args)
        {
            
            CommandLineApplication commandLineApplication =
                new CommandLineApplication(throwOnUnexpectedArg: false);

            commandLineApplication.Command("SpeedTest", (command) =>
            {
                command.Description = "This utiliy measures Internet Speed using Fast.com API.\n No arguments are required\n";
                command.ExtendedHelpText = "Usage: dotnet FastComDotNetConsole.dll SpeedTest";
                command.HelpOption("-?|-h|--help");
                

                command.OnExecute(() =>
                {

                    Console.WriteLine("Connecting...");

                    FastCodeApp fastapp = new FastCodeApp();
                    fastapp.Run();
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();

                    return 0;
                });
            });
            commandLineApplication.Execute(args);
        }
    }
}
