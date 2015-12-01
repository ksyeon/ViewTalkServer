using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using ViewTalkServer.Models;

namespace ViewTalkServer.Modules
{
    public abstract class TcpServer
    {
        private Socket serverSocket;
        private int serverPort;

        private byte[] receiveData;

        public TcpServer(int serverPort)
        {
            this.serverPort = serverPort;
            this.receiveData = new byte[32768];

            ConnectSocket();

            Console.WriteLine("TCP Server Start (Port : " + serverPort + ")");
        }

        private void ConnectSocket()
        {
            try
            {
                /* [1] Socket */
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* [2] Binding */
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, serverPort);
                serverSocket.Bind(ipEndPoint);

                /* [3] Listen */
                serverSocket.Listen(5);

                /* [4] Accept */
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = serverSocket.EndAccept(ar);

                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                /* [5] Receive */
                clientSocket.BeginReceive(receiveData, 0, receiveData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected bool SendMessage(TcpMessage message)
        {
            bool isSucess = false;

            try
            {
                List<SocketData> SendClient = ResponseMessage(null, message);

                foreach (SocketData client in SendClient)
                {
                    Socket sendSocket = client.Socket;
                    byte[] sendMessage = client.Message;

                    sendSocket.BeginSend(sendMessage, 0, sendMessage.Length, SocketFlags.None, new AsyncCallback(OnSend), sendSocket);
                }

                isSucess = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return isSucess;
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                clientSocket.EndReceive(ar);

                TcpMessage receiveMessage = new TcpMessage(receiveData);
                List<SocketData> SendClient = ResponseMessage(clientSocket, receiveMessage);

                Console.WriteLine(receiveMessage.Message);

                foreach (SocketData client in SendClient)
                {
                    Socket sendSocket = client.Socket;
                    byte[] sendMessage = client.Message;

                    /* [6] Send */
                    sendSocket.BeginSend(sendMessage, 0, sendMessage.Length, SocketFlags.None, new AsyncCallback(OnSend), sendSocket);
                }

                if (clientSocket.Connected)
                {
                    clientSocket.BeginReceive(receiveData, 0, receiveData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
                }
            }
            catch (Exception ex)
            {
                CheckConnected();

                Console.WriteLine(ex.Message);
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CloseSocket()
        {
            try
            {
                /* [7] Close */
                serverSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public abstract List<SocketData> ResponseMessage(Socket clientSockect, TcpMessage receiveMessage);
        public abstract void CheckConnected();
    }
}
