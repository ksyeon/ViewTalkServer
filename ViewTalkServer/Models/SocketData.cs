using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace ViewTalkServer.Models
{
    public class SocketData
    {
        public Socket Socket { get; set; }
        public byte[] Message { get; set; }

        public SocketData(Socket socket, TcpMessage message)
        {
            this.Socket = socket;
            this.Message = message.ToByteData();
        }
    }
}
