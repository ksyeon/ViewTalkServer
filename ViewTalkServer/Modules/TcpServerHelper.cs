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
            foreach (ClientData client in clientList)
            {
                if (client.Socket.Connected == false)
                {
                    // 일반 : Logout
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
                    // Check Duplication Login
                    bool isDuplicationLogin = false;

                    foreach (ClientData client in clientList)
                    {
                        if(client.Number == receiveMessage.UserNumber)
                        {
                            isDuplicationLogin = true;
                            break;
                        }
                    }

                    // TCP Message
                    sendMessage.Command = Command.Login;

                    if (!isDuplicationLogin)
                    {
                        // JSON Parsing
                        Dictionary<string, string> loginInfo = json.GetLoginInfo(receiveMessage.Message);

                        string id = loginInfo[JsonName.ID];
                        string password = loginInfo[JsonName.Password];

                        // Database
                        bool isExistUser = database.IsExistUser(id, password);

                        if (isExistUser)
                        {
                            sendMessage.UserNumber = database.GetNumberOfId(id);
                        }
                        else
                        {
                            sendMessage.Check = 2;
                        }
                    }
                    else
                    {
                        sendMessage.Check = 1;
                    }

                    // Add Send Client
                    sendClient.Add(new SocketData(clientSocket, sendMessage));

                    break;

                case Command.Logout:
                    break;

                case Command.CreateChatting :
                    // Add Client List
                    clientList.Add(new ClientData(clientSocket, receiveMessage.UserNumber, receiveMessage.UserNumber));

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
                            // Add Client List
                            clientList.Add(new ClientData(clientSocket, receiveMessage.UserNumber, teacherNumber));

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
                    sendMessage.Command = Command.SendChat;
                    sendMessage.UserNumber = receiveMessage.UserNumber;
                    sendMessage.ChatNumber = receiveMessage.ChatNumber;

                    // Add Send Client
                    foreach (ClientData client in clientList)
                    {
                        if(client.Number == receiveMessage.UserNumber)
                        {
                            sendMessage.Message = json.SetChattingUser(clientList, receiveMessage.ChatNumber);
                            sendClient.Add(new SocketData(client.Socket, sendMessage));
                        }
                        else if (client.Group == receiveMessage.ChatNumber)
                        {
                            sendMessage.Message = database.GetNickName(receiveMessage.UserNumber);
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

                    /*
                case Command.Connect:
                    // Add Client Socket List
                    foreach (ClientInfo clientInfo in ClientList)
                    {
                        clientSocketList.Add(clientInfo.Socket);
                    }

                    // Add Client Information
                    ClientInfo receiveClient = new ClientInfo();

                    receiveClient.Socket = clientSocket;
                    receiveClient.UserNumber = receiveMessage.Number;

                    ClientList.Add(receiveClient);

                    // Message
                    sendMessage.Command = Command.Connect;
                    sendMessage.Number = receiveMessage.Number;

                    break;
                    
                case Command.Close:
                    // Remove Client Information
                    ClientList.RemoveAll(item => (item.Socket == clientSocket));

                    // Close Client Socket
                    clientSocket.Close();

                    // Add Client Socket List
                    foreach (ClientInfo clientInfo in ClientList)
                    {
                        clientSocketList.Add(clientInfo.Socket);
                    }

                    // Message
                    sendMessage.Command = Command.Close;
                    sendMessage.Number = receiveMessage.Number;

                    break;
                    */
            }

            return sendClient;
        }
    }
}
