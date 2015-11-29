using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewTalkServer.Models
{
    public class TcpMessage
    {
        public Command Command { get; set; }
        public int Check { get; set; }
        public int UserNumber { get; set; }
        public int ChatNumber { get; set; }
        public string Message { get; set; }

        public TcpMessage()
        {
            this.Command = Command.Null;
            this.Check = 0;
            this.UserNumber = 0;
            this.ChatNumber = 0;
            this.Message = string.Empty;
        }

        public TcpMessage(byte[] byteData)
        {
            this.Command = (Command)BitConverter.ToInt32(byteData, 0);
            this.Check = BitConverter.ToInt32(byteData, 4);
            this.UserNumber = BitConverter.ToInt32(byteData, 8);
            this.ChatNumber = BitConverter.ToInt32(byteData, 12);
            this.Message = string.Empty;

            int messageLenth = BitConverter.ToInt32(byteData, 16);

            if (messageLenth > 0)
            {
                this.Message = Encoding.Unicode.GetString(byteData, 20, messageLenth);
            }
        }

        public byte[] ToByteData()
        {
            List<byte> byteData = new List<byte>();

            byteData.AddRange(BitConverter.GetBytes((int)Command));
            byteData.AddRange(BitConverter.GetBytes(Check));
            byteData.AddRange(BitConverter.GetBytes(UserNumber));
            byteData.AddRange(BitConverter.GetBytes(ChatNumber));
            byteData.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetByteCount(Message)));
            byteData.AddRange(Encoding.Unicode.GetBytes(Message));

            return byteData.ToArray();
        }
    }

    public enum Command
    {
        Null,
        Login,
        Logout,
        CreateChatting,
        JoinChatting,
        CloseChatting,
        JoinUser,
        ExitUser,
        SendChat,
        LoadPPT,
        MovePPT,
        ClosePPT
    }
}
