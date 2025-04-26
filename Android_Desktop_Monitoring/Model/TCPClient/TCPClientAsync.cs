using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Android_Desktop_Monitoring
{
    public class TCPClientAsync : TCPClient
    {
        private readonly object sendLock = new object();


        public TCPClientAsync(string ip, int port)
        {
            base.ip = ip;
            base.port = port;
            base.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Disconnect()
        {
            try
            {
                if (base.socket.Connected)
                {
                    base.socket.Shutdown(SocketShutdown.Both);
                    base.socket.Close();
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void Connect()
        {
            try
            {
                base.socket.BeginConnect(base.ip, base.port, new AsyncCallback(ConnectCallBack), base.socket);
                ReportEvent(ClientSocketEventType.Connectting, null, null);
            }
            catch (SocketException se)
            {
                ReportEvent(ClientSocketEventType.Error, null, se);

            }
            catch (ObjectDisposedException ex)
            {

                ReportEvent(ClientSocketEventType.Error, null, null);
            }

        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                base.socket.EndConnect(ar);
                base.socket.BeginReceive(base.receiveByte, 0, this.receiveByte.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), base.socket);
                ReportEvent(ClientSocketEventType.Connected, null, null);
            }
            catch (SocketException se)
            {
                ReportEvent(ClientSocketEventType.ConnectFail, null, se);
            }
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int read = base.socket.EndReceive(ar);
                int available = base.socket.Available;

                if (read == 0)
                {
                    ReportEvent(ClientSocketEventType.Disconnected, null, null);
                    base.nSize = 0;
                    Array.Clear(base.receiveByte, 0, base.receiveByte.Length);
                    base.socket.Close();
                    return;
                }

                ReportEvent(ClientSocketEventType.Received, base.receiveByte, null, read);

                if (available == 0)
                {
                    base.nSize = 0;
                    Array.Clear(base.receiveByte, 0, base.receiveByte.Length);

                }
                base.socket.BeginReceive(base.receiveByte, 0, this.receiveByte.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), base.socket);

            }
            catch (SocketException se)  // 클라이언트가 연결이 끊겼을경우
            {
                if (se.SocketErrorCode == SocketError.ConnectionReset)
                {
                    ReportEvent(ClientSocketEventType.Disconnected, null, se);
                    base.nSize = 0;
                    Array.Clear(base.receiveByte, 0, base.receiveByte.Length);
                    base.socket.Close();
                }
                else
                {
                    ReportEvent(ClientSocketEventType.Error, null, se);

                }

            }
            catch (ObjectDisposedException ex)
            {

                ReportEvent(ClientSocketEventType.Disconnected, null, null);
            }
        }

        public bool Send(byte[] message)
        {
            try
            {
                if (base.socket.Connected)
                {
                    //스레드 안정성이 없음. Lock사용하거나 concurrentQueue를 사용하거나...
                    //여기서는 lock를 사용.
                    lock (sendLock)
                    {
                        base.socket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendCallBack), message);
                        //base.socket.EndSend();
                    }
                    //ReportEvent(ClientSocketEventType.Sent, message, null, message.Length);
                }
                else
                {
                    return false;
                }
            }
            catch (SocketException se)
            {
                ReportEvent(ClientSocketEventType.Error, message, se);

            }

            return true;
        }

        private void SendCallBack(IAsyncResult ar)
        {
            try
            {
                byte[] message = (byte[])ar.AsyncState;

                ReportEvent(ClientSocketEventType.Sent, message, null);
            }
            catch (Exception ex)
            {

            }
        }
    }

}
