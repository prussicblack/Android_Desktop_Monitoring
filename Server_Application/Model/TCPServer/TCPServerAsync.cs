using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Server_Application
{
    public class TCPServerAsync : TCPServer
    {
        public TCPServerAsync(string ip, int port)
        {
            base.ip = ip;
            base.port = port;
        }

        private AsyncCallback _listenCallBack = null;
        private AsyncCallback _recieveCallBack = null;
        private AsyncCallback _sendCallBack = null;

        private readonly object sendLock = new object();
        public bool Open()
        {

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(base.ip), base.port);
            base._dicClientSockets.Clear();

            base.server.Bind(ep);
            base.server.Listen(10);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Listen Server Socket...");

            _listenCallBack = new AsyncCallback(ListenCallBack);
            _recieveCallBack = new AsyncCallback(ReceiveCallBack);
            _sendCallBack = new AsyncCallback(SendCallBack);
            base.server.BeginAccept(_listenCallBack, base.server);

            ReportEvent(ServerSocketEventType.Opened, ep, null, null);

            base.isOpen = true;

            return true;
        }
        private void ListenCallBack(IAsyncResult ar)
        {
            try
            {
                Socket serverSocket = (Socket)ar.AsyncState;
                Socket clientSocket = server.EndAccept(ar);

                IPEndPoint ep = (IPEndPoint)clientSocket.RemoteEndPoint;

                Console.WriteLine("Accept new Client ip Info : {0}, port: {1}", ep.Address, ep.Port);


                ClientSocket client = new ClientSocket();
                client.socket = clientSocket;
                client.IP = ep.Address.ToString();
                client.Port = ep.Port.ToString();
                client.socket.BeginReceive(client.buffer, 0, 4096, SocketFlags.None, _recieveCallBack, client);
                base.AddClient(client);

                base.server.BeginAccept(new AsyncCallback(ListenCallBack), base.server);

                ReportEvent(ServerSocketEventType.ClientAccepted, ep, null, client);
            }
            catch (SocketException se)
            {
                ReportEvent(ServerSocketEventType.Error, null, se, null);

            }
        }

        private List<byte[]> _lstReceive = new List<byte[]>();
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                ClientSocket client = (ClientSocket)ar.AsyncState;
                int read = client.socket.EndReceive(ar);
                int available = client.socket.Available;

                if (read == 0)
                {
                    ReportEvent(ServerSocketEventType.ClientDisconnected, (IPEndPoint)client.socket.RemoteEndPoint, null, client);
                    DeleteClient(client.socket);

                    return;
                }

                if (available == 0)
                {
                    lock (_lstReceive)
                    {
                        byte[] temp3 = new byte[read];
                        Buffer.BlockCopy(client.buffer, 0, temp3, 0, read);
                        _lstReceive.Add(temp3);
                        Array.Clear(client.buffer, 0, ClientSocket.bufferSize);
                        for (int i = 0; i < _lstReceive.Count; i++)
                        {
                            byte[] imsi = _lstReceive[i];

                            Buffer.BlockCopy(imsi, 0, client.buffer, 0, imsi.Length);
                            ReportEvent(ServerSocketEventType.ClientReceived, (IPEndPoint)client.socket.RemoteEndPoint, null, client, imsi.Length);
                        }
                        Array.Clear(client.buffer, 0, ClientSocket.bufferSize);
                        _lstReceive.Clear();

                    }
                }
                else
                {
                    lock (_lstReceive)
                    {
                        byte[] temp = new byte[read];
                        Buffer.BlockCopy(client.buffer, 0, temp, 0, read);
                        _lstReceive.Add(temp);
                    }
                }

                client.socket.BeginReceive(client.buffer, 0, ClientSocket.bufferSize, SocketFlags.None, _recieveCallBack, client);

            }
            catch (SocketException se)  // 클라이언트가 연결이 끊겼을경우
            {
                Console.WriteLine("Receive Catch Error {0}, {1}, {2}", se.Message, se.SocketErrorCode, se.StackTrace);

                ClientSocket client = (ClientSocket)ar.AsyncState;
                client.ConnectedStatus = false;
                if (se.SocketErrorCode == SocketError.ConnectionReset)
                {
                    ReportEvent(ServerSocketEventType.ClientDisconnected, null, se, null);
                    DeleteClient(client.socket);
                }
            }

            catch (ObjectDisposedException ex)
            {
                ClientSocket client = (ClientSocket)ar.AsyncState;
                ReportEvent(ServerSocketEventType.ClientDisconnected, null, null, null);
                DeleteClient(client.socket);
                Console.WriteLine("[Exception]35->" + ex.Message + ex.StackTrace);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString() + ex.StackTrace.ToString());
            }
        }

        public void Send(Socket clientKey, byte[] message)
        {
            try
            {
                ClientSocket client = base.GetClient(clientKey);

                if (client != null)
                {
                    if (client.socket.Connected)
                    {
                        //스레드 안정성이 없음. Lock사용하거나 concurrentQueue를 사용하거나...
                        //여기서는 lock를 사용.
                        lock (sendLock)
                        {
                            client.socket.BeginSend(message, 0, message.Length, SocketFlags.None, _sendCallBack, client);
                        }

                    }
                }
            }
            catch (SocketException se)
            {
                ReportEvent(ServerSocketEventType.Error, null, se, null);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("[Exception]36->" + ex.Message + ex.StackTrace);
                ReportEvent(ServerSocketEventType.Error, null, null, null);
            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
            try
            {
                ClientSocket client = (ClientSocket)ar.AsyncState;
                ReportEvent(ServerSocketEventType.ClientSent, (IPEndPoint)client.socket.RemoteEndPoint, null, client);
            }
            catch (SocketException se)
            {
                ReportEvent(ServerSocketEventType.Error, null, se, null);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("[Exception]37->" + ex.Message + ex.StackTrace);
                ReportEvent(ServerSocketEventType.Error, null, null, null);
            }
        }





    }
}
