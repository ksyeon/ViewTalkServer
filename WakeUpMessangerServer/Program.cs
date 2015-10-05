using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using WakeUpMessangerServer.Modules;

namespace WakeUpMessangerServer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Console.WriteLine("[WakeUp! Messanger Server]\n");

            TcpServerHelper tcpServer = new TcpServerHelper();
            DatabaseHelper database = new DatabaseHelper();

            Console.WriteLine("--------------------------------------------------");

            while (true)
            {
                // Running TCP Server
            }
        }
    }
}
