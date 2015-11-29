using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace ViewTalkServer.Models
{
    public class ClientData
    {
        public Socket Socket { get; set; }
        public int Number { get; set; }
        public int Group { get; set; }

        public ClientData(Socket socket, int number, int group)
        {
            this.Socket = socket;
            this.Number = number;
            this.Group = group;
        }
    }
}
