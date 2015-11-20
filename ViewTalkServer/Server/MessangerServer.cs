using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ViewTalkServer.Modules;
using ViewTalkServer.Models;

namespace ViewTalkServer.Server
{
    public class MessangerServer
    {
        private TcpServerHelper tcpServer;

        public MessangerServer()
        {
            Console.WriteLine("[View Talk Server]\n");

            this.tcpServer = new TcpServerHelper();

            Console.WriteLine("--------------------------------------------------");
        }
    }
}
