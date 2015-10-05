using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

using WakeUpMessangerServer.Models;

namespace WakeUpMessangerServer.Modules
{
    public class TcpServerHelper : TcpServer
    {
        private const int ServerPort = 8080;

        private List<ClientInfo> ClientList;

        private DatabaseHelper database;
        private JsonHelper json;

        public TcpServerHelper() : base(ServerPort)
        {
            this.ClientList = new List<ClientInfo>();

            this.database = new DatabaseHelper();
            this.json = new JsonHelper();
        }

        public override List<Socket> CheckMessage(Socket clientSocket, MessageData receiveMessage, MessageData sendMessage)
        {
            List<Socket> clientSocketList = new List<Socket>();

            switch (receiveMessage.Command)
            {
                case Command.Login:
                    Dictionary<string, string> loginInfo = json.GetLoginInfo(receiveMessage.Message);

                    string id = loginInfo[JsonName.ID];
                    string password = loginInfo[JsonName.Password];

                    bool isExistUser = database.IsExistUser(id, password);

                    if (isExistUser)
                    {
                        sendMessage.Command = Command.Login;
                        sendMessage.UserNumber = database.GetUserNumber(id);
                    }
                    else
                    {
                        clientSocket.Close();
                    }

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

                    // Initialization Message
                    sendMessage.Command = Command.Logout;
                    sendMessage.UserNumber = receiveMessage.UserNumber;

                    break;

                case Command.Connect:
                    // Add Client Socket List
                    foreach (ClientInfo clientInfo in ClientList)
                    {
                        clientSocketList.Add(clientInfo.Socket);
                    }

                    // Add Client Information
                    ClientInfo receiveClient = new ClientInfo();

                    receiveClient.Socket = clientSocket;
                    receiveClient.UserNumber = receiveMessage.UserNumber;

                    ClientList.Add(receiveClient);

                    // Initialization Message
                    sendMessage.Command = Command.Login;
                    sendMessage.UserNumber = receiveMessage.UserNumber;

                    break;

                case Command.Update:
                    // Add Client Socket List
                    clientSocketList.Add(clientSocket);

                    // Initialization Message
                    sendMessage.Command = Command.Update;

                    foreach (ClientInfo clientInfo in ClientList)
                    {
                        sendMessage.Message += clientInfo.UserNumber + "/";
                    }

                    break;

                case Command.Message:
                    // Add Client Socket List
                    foreach (ClientInfo clientInfo in ClientList)
                    {
                        if (clientInfo.Socket != clientSocket)
                        {
                            clientSocketList.Add(clientInfo.Socket);
                        }
                    }

                    // Initialization Message
                    sendMessage.Command = Command.Message;
                    sendMessage.UserNumber = receiveMessage.UserNumber;
                    sendMessage.Message = receiveMessage.Message;

                    break;
            }

            return clientSocketList;
        }

        public struct ClientInfo
        {
            public Socket Socket;
            public ulong UserNumber;
        }
    }
}
