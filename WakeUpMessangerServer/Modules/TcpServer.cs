using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using WakeUpMessangerServer.Models;

namespace WakeUpMessangerServer.Modules
{
    public abstract class TcpServer
    {
        private Socket serverSocket;
        private int serverPort;

        private byte[] byteData;

        public TcpServer(int serverPort)
        {
            this.serverPort = serverPort;
            this.byteData = new byte[1024];

            Initialize();

            Console.WriteLine("TCP Server Start (Port : " + serverPort + ")");
        }

        protected void Initialize()
        {
            try
            {
                /* [1] Socket */
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* [2] Binding */
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, serverPort);
                serverSocket.Bind(ipEndPoint);

                /* [3] Listen */
                serverSocket.Listen(4);

                /* [4] Accept */
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = serverSocket.EndAccept(ar);

                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                /* [5] Receive */
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void OnReceive(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                clientSocket.EndReceive(ar);

                Console.WriteLine("Receive OK");

                MessageData receiveMessage = new MessageData(byteData);
                MessageData sendMessage = new MessageData();

                List<Socket> clientSocketList = CheckMessage(clientSocket, receiveMessage, sendMessage);

                byte[] byteMessage = sendMessage.ToByteData();

                foreach (Socket socket in clientSocketList)
                {
                    /* [6] Send */
                    socket.BeginSend(byteMessage, 0, byteMessage.Length, SocketFlags.None, new AsyncCallback(OnSend), socket);
                }

                if (clientSocket.Connected)
                {
                    clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void CloseSocket()
        {
            try
            {
                if (serverSocket != null)
                {
                    /* [7] Close */
                    serverSocket.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public abstract List<Socket> CheckMessage(Socket clientSockect, MessageData receiveMessage, MessageData sendMessage);
    }
}
