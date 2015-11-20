using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ViewTalkServer.Server;

namespace ViewTalkServer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            MessangerServer server = new MessangerServer();

            while (true)
            {
                //server
            }
        }
    }
}
