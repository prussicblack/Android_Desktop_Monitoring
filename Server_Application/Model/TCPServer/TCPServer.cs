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
    public enum ServerSocketEventType { Opened, Closed, ClientAccepted, ClientDisconnected, ClientReceived, ClientSent, Error };

    public delegate void ServerSocketHandler(object sender, ServerSocketEvent e);

    public class TCPServer
    {
        public string ip { get; set; }
        public int port { get; set; } = 0;

        protected bool isOpen;

        protected Socket server;
        protected Dictionary<Socket, ClientSocket> _dicClientSockets; // key ip:port
        public ServerSocketHandler handler;

        object socketlock = new object();

        public TCPServer()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _dicClientSockets = new Dictionary<Socket, ClientSocket>();
        }

        public void Close()
        {
            lock (socketlock)
            {
                _dicClientSockets.Clear();
            }

            //this.server.Shutdown(SocketShutdown.Both);
            server.Close();

            isOpen = false;
            ReportEvent(ServerSocketEventType.Closed, null, null, null);
        }




        public void ReportEvent(ServerSocketEventType type, IPEndPoint point, SocketException se, ClientSocket clientSocket, int read = 0)
        {
            if (handler != null)
            {
                try
                {
                    if (read > 0)
                    {

                    }
                    //LogExtension.Log(OPLogger.eLogLevel.DEBUG, $"ReportEvent read size : [{read}]");

                }
                catch
                {

                }
                handler(this, new ServerSocketEvent((int)type, point, se, clientSocket, read));
            }
        }
        public bool IsOpen()
        {
            return isOpen;
        }

        public ClientSocket GetClientFromIP(string Socketip) // ip가 unique 하다는 전제하에서만 사용
        {
            lock (socketlock)
            {
                foreach (var item in _dicClientSockets)
                {
                    if (item.Value.GetIpString().Equals(Socketip))
                        return item.Value;
                }
            }


            return null;
        }

        public Socket GetClientFromIPSocket(string Socketip) // ip가 unique 하다는 전제하에서만 사용
        {
            lock (socketlock)
            {
                foreach (var item in _dicClientSockets)
                {
                    if (item.Value.GetIpString().Equals(Socketip))
                        return item.Key;
                }
            }


            return null;
        }


        public int GetClientCount()
        {
            lock (socketlock)
            {
                return _dicClientSockets.Count();
            }

        }
        protected ClientSocket GetClient(Socket Socketkey)
        {
            lock (socketlock)
            {
                if (_dicClientSockets.ContainsKey(Socketkey))
                {
                    return _dicClientSockets[Socketkey];
                }
                return null;
            }

        }
        protected void DeleteClient(Socket client)
        {
            lock (socketlock)
            {
                try
                {
                    if (_dicClientSockets.ContainsKey(client))
                    {
                        if (_dicClientSockets[client].socket.Connected)
                        {
                            _dicClientSockets[client].socket.Shutdown(SocketShutdown.Both);
                            _dicClientSockets[client].socket.Close();
                        }

                        _dicClientSockets.Remove(client);

                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("DeleteClient Error = {0} error {1} StackTrace", "", ex.Message, ex.StackTrace);
                    Console.ForegroundColor = ConsoleColor.White;

                }
            }

        }
        protected void AddClient(ClientSocket client)
        {
            lock (socketlock)
            {
                try
                {
                    if (null != client.GetIpAndPort())
                    {

                        foreach (var item in _dicClientSockets)
                        {
                            if (client.GetIpString() == item.Value.GetIpString())
                            {
                                if (item.Value.socket.Connected)
                                {
                                    item.Value.socket.Shutdown(SocketShutdown.Both);
                                    item.Value.socket.Close();
                                }

                                _dicClientSockets.Remove(item.Key);
                                break;
                            }
                        }

                        _dicClientSockets[client.socket] = client;

                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    //Console.WriteLine("AddClient Error = {0} error {1} StackTrace {2}", client.socket.RemoteEndPoint.ToString(), ex.Message, ex.StackTrace);

                    Console.WriteLine("AddClient Error = {0} error {1} StackTrace {2}", "", ex.Message, ex.StackTrace);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

        }
    }


    public class ServerSocketEvent : EventArgs
    {
        public ServerSocketEventType eventType;
        public byte[] message;
        public int read;
        public IPEndPoint endPoint;
        public SocketException se;
        public ClientSocket socket;
        public ServerSocketEvent(int type, IPEndPoint point, SocketException se, ClientSocket sc, int nRead)
        {
            eventType = (ServerSocketEventType)type;
            endPoint = point;
            socket = sc;
            read = nRead;
            this.se = se;
        }
    }

    public class ClientSocket
    {
        // Client  socket.
        public Socket socket = null;
        // Size of receive buffer.
        public const int bufferSize = 10240;
        // Receive buffer.
        public byte[] buffer = new byte[bufferSize];

        public bool ConnectedStatus = false;

        public string IP = string.Empty;

        public string Port = string.Empty;

        public byte[] ReceiveByte = null;
        // Received data string.
        public StringBuilder ReceiveString = new StringBuilder();

        public string GetIpAndPort()
        {
            if (socket != null)
            {
                Console.WriteLine($"**********************************************");
                Console.WriteLine($"DEBUG - {socket.RemoteEndPoint.ToString()}");
                return socket.RemoteEndPoint.ToString();
            }
            else
                return null;
        }

        public string GetIpString()
        {
            try
            {
                string address = socket.RemoteEndPoint.ToString();

                string[] array = address.Split(new char[] { ':' });

                return array[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());

                return null;
            }
        }

        string GetPortString()
        {
            try
            {
                string address = socket.RemoteEndPoint.ToString();

                string[] array = address.Split(new char[] { ':' });

                return array[1];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());

                return null;
            }

        }

    }


}

