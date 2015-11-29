using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

using ViewTalkServer.Models;

namespace ViewTalkServer.Modules
{
    public class TcpServerHelper : TcpServer
    {
        private const int ServerPort = 8080;

        private DatabaseHelper database;
        private JsonHelper json;

        public List<ClientData> clientList { get; set; }
        public List<int> chattingList { get; set; }

        public delegate void MessageDelegate(TcpMessage message);
        public MessageDelegate ExecuteMessage { get; set; }

        public TcpServerHelper() : base(ServerPort)
        {
            this.database = new DatabaseHelper();
            this.json = new JsonHelper();

            this.clientList = new List<ClientData>();
            this.chattingList = new List<int>();
        }

        public void CheckConnected()
        {
            for(int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].Socket != null && clientList[i].Socket.Connected == false)
                {
                    if (clientList[i].Group == 0) // 채팅방 접속 X
                    {
                        clientList.Remove(clientList[i]);
                    }

                    // 강사 : CloseChatting, Logout
                    // 학생 : ExitChatting, Logout
                }
            }
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
                    chattingList.Add(receiveMessage.UserNumber);

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
                        bool isExistChatting = chattingList.Contains(teacherNumber);

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

                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                        else if (client.Group == receiveMessage.ChatNumber)
                        {
                            sendMessage.Message = database.GetNickNameOfNumber(receiveMessage.UserNumber);

                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                    }

                    break;

                case Command.ExitUser:
                    break;

                case Command.SendChat:
                    // TCP Message
                    sendMessage.Command = Command.SendChat;
                    sendMessage.UserNumber = receiveMessage.UserNumber;
                    sendMessage.ChatNumber = receiveMessage.ChatNumber;
                    sendMessage.Message = receiveMessage.Message;

                    // Add Client Socket List
                    foreach (ClientData client in clientList)
                    {
                        if (client.Number != receiveMessage.UserNumber && client.Group == receiveMessage.ChatNumber)
                        {
                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                    }

                    break;

                case Command.LoadPPT:
                    break;

                case Command.MovePPT:
                    break;

                case Command.ClosePPT:
                    break;
            }

            return sendClient;
        }
    }
}
