﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeUpMessangerServer.Models
{
    public enum Command
    {
        Null,
        Login,
        Logout,
        Update,
        Message
    }

    public class MessageData
    {
        public Command Command { get; set; }
        public ulong UserNumber { get; set; }
        public ulong ChatNumber { get; set; }
        public string Message { get; set; }

        public MessageData()
        {
            this.Command = Command.Null;
            this.UserNumber = 0;
            this.ChatNumber = 0;
            this.Message = string.Empty;
        }

        public MessageData(byte[] byteData)
        {
            this.Command = (Command)BitConverter.ToInt32(byteData, 0);
            this.UserNumber = BitConverter.ToUInt64(byteData, 4);
            this.ChatNumber = BitConverter.ToUInt64(byteData, 12);
            this.Message = string.Empty;

            int messageLenth = BitConverter.ToInt32(byteData, 20);

            if (messageLenth > 0)
            {
                this.Message = Encoding.UTF8.GetString(byteData, 24, messageLenth);
            }
        }

        public byte[] ToByteData()
        {
            List<byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes((int)Command));
            result.AddRange(BitConverter.GetBytes(UserNumber));
            result.AddRange(BitConverter.GetBytes(ChatNumber));
            result.AddRange(BitConverter.GetBytes(Message.Length));
            result.AddRange(Encoding.UTF8.GetBytes(Message));

            return result.ToArray();
        }
    }
}
