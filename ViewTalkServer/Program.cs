using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ViewTalkServer.Modules;

namespace ViewTalkServer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Console.WriteLine("[View Talk Server]\n");

            MessangerServer messanger = new MessangerServer();

            Console.WriteLine("--------------------------------------------------");

            while (true)
            {

            }
        }
    }
}
    