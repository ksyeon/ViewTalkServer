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
                    // Add Client Socket List
                    clientSocketList.Add(clientSocket);

                    // JSON Parsing
                    Dictionary<string, string> loginInfo = json.GetLoginInfo(receiveMessage.Message);

                    string id = loginInfo[JsonName.ID];
                    string password = loginInfo[JsonName.Password];

                    // Database
                    bool isExistUser = database.IsExistUser(id, password);

                    // Message
                    sendMessage.Command = Command.Login;

                    if (isExistUser)
                    {
                        sendMessage.Number = database.GetUserNumber(id);
                    }
                    else
                    {
                        sendMessage.Auth = 0; // False
                    }

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

                case Command.Update:
                    // Add Client Socket List
                    clientSocketList.Add(clientSocket);

                    // Message
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
                    sendMessage.Number = receiveMessage.Number;
                    sendMessage.Message = receiveMessage.Message;

                    break;
            }

            return clientSocketList;
        }

        public struct ClientInfo
        {
            public Socket Socket;
            public int UserNumber;
        }
    }
}
