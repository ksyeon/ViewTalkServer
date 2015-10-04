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
            Console.WriteLine("[WakeUp Messanger Server]\n");

            TcpServer tcpServer = new TcpServerHelper(8080);

            while (true)
            {
                // Running TCP Server
            }
        }
    }
}
