using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Android_Desktop_Monitoring
{
    public enum ClientSocketEventType { Connectting, Connected, Disconnected, Received, Sent, ConnectFail, Error };
    public delegate void ClientSocketHandler(object sender, ClientSocketEvent e);

    public abstract class TCPClient
    {
        public string ip { get; protected set; }
        public int port { get; protected set; } = 0;
        protected Socket socket;

        protected byte[] receiveByte;
        protected int nSize;

        public event ClientSocketHandler handler;
        ClientSocketEvent _SocketEvent;

        protected TCPClient()
        {
            this.receiveByte = new byte[10240];
            nSize = 0;

            _SocketEvent = new ClientSocketEvent((int)ClientSocketEventType.Error, null, null, 0);
        }

        public Socket GetSocket()
        {
            return socket;
        }

        public void ReportEvent(ClientSocketEventType type, byte[] buffer, SocketException se, int nSize = 0)
        {
            try
            {

                if (handler != null)
                {
                    _SocketEvent.eventType = type;
                    _SocketEvent.message = buffer;
                    _SocketEvent.nSize = nSize;
                    _SocketEvent.se = se;

                    handler(this, _SocketEvent);
                }
            }
            catch (Exception ex)
            {
                //LogExtension.LogException(ex, $"Client Sokcet ReportEvent", OPLOG.Folder.Log, OPLOG.File.Log);
                //Console.WriteLine($"Client Sokcet ReportEvent {0}, StackTrace {1}", ex.Message, ex.StackTrace);
            }

        }

        public void Close()
        {
            try
            {

                Array.Clear(this.receiveByte, 0, this.receiveByte.Length);
                this.socket.Close();
            }
            catch (Exception ex)
            {
                //LogExtension.LogException(ex, $"Client Sokcet Close", OPLOG.Folder.Log, OPLOG.File.Log);
                //Console.WriteLine($"Client Sokcet ReportEvent {0}, StackTrace {1}", ex.Message, ex.StackTrace);
            }
        }
    }


    public class ClientSocketEvent : EventArgs
    {
        public ClientSocketEventType eventType;
        public byte[] message;
        public SocketException se;
        public int nSize;

        public ClientSocketEvent(int type, byte[] buffer, SocketException se, int nSize)
        {

            this.eventType = (ClientSocketEventType)type;
            this.message = buffer;
            this.se = se;
            this.nSize = nSize;
        }
    }


}
