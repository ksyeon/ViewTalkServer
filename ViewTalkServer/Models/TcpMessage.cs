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
        public PPTData PPT { get; set; }

        public TcpMessage()
        {
            this.Command = Command.Null;
            this.Check = 0;
            this.UserNumber = 0;
            this.ChatNumber = 0;
            this.Message = string.Empty;
            this.PPT = new PPTData();
        }

        public TcpMessage(byte[] byteData)
        {
            this.Command = (Command)BitConverter.ToInt32(byteData, 0);
            this.Check = BitConverter.ToInt32(byteData, 4);
            this.UserNumber = BitConverter.ToInt32(byteData, 8);
            this.ChatNumber = BitConverter.ToInt32(byteData, 12);
            this.Message = string.Empty;
            this.PPT = new PPTData();

            int messageLenth = BitConverter.ToInt32(byteData, 16);
            if (messageLenth > 0)
            {
                this.Message = Encoding.Unicode.GetString(byteData, 20, messageLenth);
            }

            // PPT
            PPT.LastPage = BitConverter.ToInt32(byteData, messageLenth + 20);
            PPT.CurrentPage = BitConverter.ToInt32(byteData, messageLenth + 24);

            int pptLenth = BitConverter.ToInt32(byteData, messageLenth + 28);

            PPT.CurrentPPT = new byte[pptLenth];
            Array.Copy(byteData, messageLenth + 32, PPT.CurrentPPT, 0, pptLenth);
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

            // PPT
            byteData.AddRange(BitConverter.GetBytes(PPT.LastPage));
            byteData.AddRange(BitConverter.GetBytes(PPT.CurrentPage));
            byteData.AddRange(BitConverter.GetBytes(PPT.CurrentPPT.Length));
            byteData.AddRange(PPT.CurrentPPT);

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
        SendPPT,
        ClosePPT
    }
}
