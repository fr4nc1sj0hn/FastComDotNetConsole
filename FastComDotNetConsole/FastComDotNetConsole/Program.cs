using fast_com;
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
        private static int[] results;
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting...");

            FastCodeApp fastapp = new FastCodeApp();
            fastapp.Run();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
