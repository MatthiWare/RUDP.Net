using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ServerForm());

            TestServer server = new TestServer(43594);
            server.Start();

            while (Console.ReadKey().Key != ConsoleKey.C)
                Thread.Sleep(0);

            Console.WriteLine("'C' pressed exiting..");

            server.Stop();
        }
    }
}
