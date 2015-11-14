using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewTalkServer.Models
{
    public enum Command
    {
        Null,
        Login,
        Connect,
        Close,
        Update,
        Message
    }

    public class MessageData
    {
        public Command Command { get; set; }
        public int Auth { get; set; }
        public int Number { get; set; }
        public string Message { get; set; }

        public MessageData()
        {
            this.Command = Command.Null;
            this.Auth = 1; // True
            this.Number = 0;
            this.Message = string.Empty;
        }

        public MessageData(byte[] byteData)
        {
            this.Command = (Command)BitConverter.ToInt32(byteData, 0);
            this.Auth = BitConverter.ToInt32(byteData, 4);
            this.Number = BitConverter.ToInt32(byteData, 8);
            this.Message = string.Empty;

            int messageLenth = BitConverter.ToInt32(byteData, 12);

            if (messageLenth > 0)
            {
                this.Message = Encoding.Unicode.GetString(byteData, 16, messageLenth);
            }
        }

        public byte[] ToByteData()
        {
            List<byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes((int)Command));
            result.AddRange(BitConverter.GetBytes(Auth));
            result.AddRange(BitConverter.GetBytes(Number));
            result.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetByteCount(Message)));
            result.AddRange(Encoding.Unicode.GetBytes(Message));

            return result.ToArray();
        }
    }
}
