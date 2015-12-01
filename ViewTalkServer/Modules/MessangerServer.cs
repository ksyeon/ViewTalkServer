using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

using ViewTalkServer.Models;

namespace ViewTalkServer.Modules
{
    public class MessangerServer : TcpServer
    {
        private const int ServerPort = 8080;

        private DatabaseHelper database;
        private JsonHelper json;

        public List<ClientData> clientList { get; set; }
        public List<ChattingData> chattingList { get; set; }

        public delegate void MessageDelegate(TcpMessage message);
        public MessageDelegate ExecuteMessage { get; set; }

        public MessangerServer() : base(ServerPort)
        {
            this.database = new DatabaseHelper();
            this.json = new JsonHelper();

            this.clientList = new List<ClientData>();
            this.chattingList = new List<ChattingData>();
        }

        public override List<SocketData> ResponseMessage(Socket clientSocket, TcpMessage receiveMessage)
        {
            List<SocketData> sendClient = new List<SocketData>();
            TcpMessage sendMessage = new TcpMessage();

            switch (receiveMessage.Command)
            {
                case Command.Login:
                    // JSON Parsing
                    Dictionary<string, string> loginInfo = json.GetLoginInfo(receiveMessage.Message);

                    string id = loginInfo[JsonName.ID];
                    string password = loginInfo[JsonName.Password];

                    // Database
                    bool isExistUser = database.IsExistUser(id, password);

                    // TCP Message
                    sendMessage.Command = Command.Login;

                    if (isExistUser)
                    {
                        // Check Duplication Login
                        bool isDuplicationLogin = false;

                        sendMessage.UserNumber = database.GetNumberOfId(id);
                        sendMessage.Message = database.GetNickNameOfNumber(sendMessage.UserNumber);

                        foreach (ClientData client in clientList)
                        {
                            if (client.Number == sendMessage.UserNumber)
                            {
                                isDuplicationLogin = true;
                                break;
                            }
                        }

                        if (!isDuplicationLogin)
                        {
                            // Add Client List
                            sendMessage.Check = 0;
                            clientList.Add(new ClientData(clientSocket, sendMessage.UserNumber, 0));
                        }
                        else
                        {
                            sendMessage.Check = 1;
                        }
                    }
                    else
                    {
                        sendMessage.Check = 2;
                    }

                    // Add Send Client
                    sendClient.Add(new SocketData(clientSocket, sendMessage));

                    break;

                case Command.Logout:
                    break;

                case Command.CreateChatting :
                    // Update Client List
                    foreach (ClientData client in clientList)
                    {
                        if (client.Number == receiveMessage.UserNumber)
                        {
                            client.Group = receiveMessage.UserNumber;
                            break;
                        }
                    }

                    // Add Chatting List
                    chattingList.Add(new ChattingData(receiveMessage.UserNumber));

                    // TCP Message
                    sendMessage.Command = Command.CreateChatting;

                    // Add Send Client
                    sendClient.Add(new SocketData(clientSocket, sendMessage));

                    break;

                case Command.JoinChatting:
                    // Check Existed Nickname
                    bool isExistNickname = database.IsExistNickname(receiveMessage.Message);

                    // TCP Message
                    sendMessage.Command = Command.JoinChatting;

                    if (isExistNickname)
                    {
                        // Check Existed Chatting
                        int teacherNumber = database.GetNumberOfNickname(receiveMessage.Message);
                        bool isExistChatting = chattingList.Exists(x => (x.ChatNumber == teacherNumber));

                        if (isExistChatting)
                        {
                            // Update Client List
                            foreach (ClientData client in clientList)
                            {
                                if (client.Number == receiveMessage.UserNumber)
                                {
                                    client.Group = teacherNumber;
                                    break;
                                }
                            }

                            sendMessage.Check = 0;
                            sendMessage.ChatNumber = teacherNumber;
                        }
                        else
                        {
                            sendMessage.Check = 1;
                        }
                    }
                    else
                    {
                        sendMessage.Check = 2;
                    }
                    
                    // Add Send Client
                    sendClient.Add(new SocketData(clientSocket, sendMessage));

                    break;

                case Command.CloseChatting:
                    // TCP Message
                    sendMessage.Command = Command.CloseChatting;
                    sendMessage.UserNumber = receiveMessage.UserNumber;
                    sendMessage.ChatNumber = receiveMessage.ChatNumber;

                    // Add Send List
                    foreach (ClientData client in clientList)
                    {
                        if (client.Number != receiveMessage.UserNumber && client.Group == receiveMessage.ChatNumber)
                        {
                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                    }

                    break;

                case Command.JoinUser:
                    // TCP Message
                    sendMessage.Command = Command.JoinUser;
                    sendMessage.UserNumber = receiveMessage.UserNumber;
                    sendMessage.ChatNumber = receiveMessage.ChatNumber;

                    // Add Send Client
                    foreach (ClientData client in clientList)
                    {
                        if(client.Number == receiveMessage.UserNumber)
                        {
                            sendMessage.Check = 1;
                            sendMessage.Message = json.SetChattingUser(clientList, receiveMessage.ChatNumber);

                            ChattingData joinPPT = chattingList.Find(x => (x.ChatNumber == receiveMessage.ChatNumber)); // ArgumentNullException
                            sendMessage.PPT = joinPPT.PPT;

                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                        else if (client.Group == receiveMessage.ChatNumber)
                        {
                            sendMessage.Check = 0;
                            sendMessage.Message = database.GetNickNameOfNumber(receiveMessage.UserNumber);

                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                    }

                    break;

                case Command.ExitUser:
                    // TCP Message
                    sendMessage.Command = Command.ExitUser;
                    sendMessage.UserNumber = receiveMessage.UserNumber;
                    sendMessage.ChatNumber = receiveMessage.ChatNumber;

                    // Add Send List
                    foreach (ClientData client in clientList)
                    {
                        if (client.Number != receiveMessage.UserNumber && client.Group == receiveMessage.ChatNumber)
                        {
                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                    }

                    break;

                case Command.SendChat:
                    // TCP Message
                    sendMessage.Command = Command.SendChat;
                    sendMessage.UserNumber = receiveMessage.UserNumber;
                    sendMessage.ChatNumber = receiveMessage.ChatNumber;
                    sendMessage.Message = receiveMessage.Message;

                    // Add Send List
                    foreach (ClientData client in clientList)
                    {
                        if (client.Number != receiveMessage.UserNumber && client.Group == receiveMessage.ChatNumber)
                        {
                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                    }

                    break;

                case Command.SendPPT:
                    // TCP Message
                    sendMessage.Command = Command.SendPPT;
                    sendMessage.UserNumber = receiveMessage.UserNumber;
                    sendMessage.ChatNumber = receiveMessage.ChatNumber;
                    sendMessage.PPT = receiveMessage.PPT;

                    // Add Chatting List
                    ChattingData SendPPT = chattingList.Find(x => (x.ChatNumber == receiveMessage.ChatNumber)); // ArgumentNullException
                    SendPPT.PPT = receiveMessage.PPT;

                    // Add Send List
                    foreach (ClientData client in clientList)
                    {
                        if (client.Number != receiveMessage.UserNumber && client.Group == receiveMessage.ChatNumber)
                        {
                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                    }

                    break;

                case Command.ClosePPT:
                    // TCP Message
                    sendMessage.Command = Command.ClosePPT;
                    sendMessage.UserNumber = receiveMessage.UserNumber;
                    sendMessage.ChatNumber = receiveMessage.ChatNumber;

                    // Add Chatting List
                    ChattingData closePPT = chattingList.Find(x => (x.ChatNumber == receiveMessage.ChatNumber)); // ArgumentNullException
                    closePPT.PPT.ResetPPT();

                    // Add Send List
                    foreach (ClientData client in clientList)
                    {
                        if (client.Number != receiveMessage.UserNumber && client.Group == receiveMessage.ChatNumber)
                        {
                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                    }

                    break;
            }

            return sendClient;
        }

        public override void CheckConnected()
        {
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].Socket != null && clientList[i].Socket.Connected == false)
                {
                    if (clientList[i].Group == 0) // 채팅방 X
                    {
                        clientList.Remove(clientList[i]);
                    }
                    else if (clientList[i].Number == clientList[i].Group) // 채팅방, 강사
                    {
                        TcpMessage sendMessage = new TcpMessage();

                        sendMessage.Command = Command.CloseChatting;
                        sendMessage.UserNumber = clientList[i].Number;
                        sendMessage.ChatNumber = clientList[i].Group;

                        clientList.Remove(clientList[i]);

                        SendMessage(sendMessage);
                    }
                    else // 채팅방, 학생
                    {
                        TcpMessage sendMessage = new TcpMessage();

                        sendMessage.Command = Command.ExitUser;
                        sendMessage.UserNumber = clientList[i].Number;
                        sendMessage.ChatNumber = clientList[i].Group;

                        clientList.Remove(clientList[i]);

                        SendMessage(sendMessage);
                    }
                }
            }
        }
    }
}
