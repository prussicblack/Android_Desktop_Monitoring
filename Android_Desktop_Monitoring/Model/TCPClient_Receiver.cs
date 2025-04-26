using HarfBuzzSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Android_Desktop_Monitoring
{
    public class TCPClient_Receiver
    {

        //메인 로직 포지션. 나중에 메인로직 만들면 그쪽으로 이동할것들은 이동할것.


        public bool ConnectionSocket { get; set; }
        public TCPClientAsync clientSocket = null;


        private const byte STX = 0x02; // STX (Start of Text)
        private const byte ETX = 0x03; // ETX (End of Text)
        private MemoryStream _buffer = new MemoryStream(); // 데이터를 임시로 저장할 버퍼

        System.Timers.Timer _timeSetTimer = null;

        //public EventHandlerAutoModeChange FeedbackStartEvent;


        public TCPClient_Receiver()
        {
            ConnectionSocket = false;


            _timeSetTimer = new System.Timers.Timer();
            _timeSetTimer.Interval = Convert.ToDouble(1000);

            _timeSetTimer.Elapsed += new ElapsedEventHandler(TrySocketConnection);
        }

        public void InitConnect()
        {
            if (clientSocket != null)
            {
                clientSocket.Disconnect();
            }


            clientSocket = new TCPClientAsync(GlobalData.ConfigData.IP, GlobalData.ConfigData.Port);

            clientSocket.handler += ClientSocketHandler;

            clientSocket.Connect();
        }
        private void TrySocketConnection(object sender, ElapsedEventArgs e)
        {
            InitConnect();
        }

        private void StartTimeSetTimer()
        {
            _timeSetTimer.Start();
        }

        private void EndTimeSetTimer()
        {
            _timeSetTimer.Stop();
        }

        public void ClientSocketHandler(object sender, ClientSocketEvent e)
        {
            try
            {
                switch (e.eventType)
                {
                    case ClientSocketEventType.Connected:
                        {
                            ConnectionSocket = true;
                            EndTimeSetTimer();

                            break;
                        }
                    case ClientSocketEventType.Disconnected:
                        {
                            ConnectionSocket = false;
                            StartTimeSetTimer();

                            break;
                        }
                    case ClientSocketEventType.Received:
                        {
                            process(e.message);

                            break;
                        }
                    case ClientSocketEventType.Sent:
                        {
                            break;
                        }
                    case ClientSocketEventType.ConnectFail:
                        {
                            ConnectionSocket = false;
                            StartTimeSetTimer();

                            break;
                        }
                    case ClientSocketEventType.Error:
                        {
                            ConnectionSocket = false;
                            break;
                        }
                    default:
                        {
                            break;
                        }

                }
            }
            catch (Exception ex)
            {

            }
        }

        public void process(byte[] Received)
        {
            foreach (var b in Received)
            {
                if (b == STX)
                {
                    // STX가 오면 버퍼를 초기화 (새 메시지가 시작됨)
                    _buffer.SetLength(0);
                }
                else if (b == ETX)
                {
                    // ETX가 오면 메시지 완료 -> 역직렬화 처리
                    HandleCompleteMessage(_buffer.ToArray());
                }
                else
                {
                    // STX와 ETX 사이의 데이터를 버퍼에 추가
                    _buffer.WriteByte(b);
                }
            }

        }

        private void HandleCompleteMessage(byte[] messageBytes)
        {
            try
            {
                var jsonString = System.Text.Encoding.UTF8.GetString(messageBytes);

                // JSON 역직렬화

                // 타입검사.
                JObject jsonObject = JObject.Parse(jsonString);
                //string type = jsonObject["name"]?.ToString();

                ReceiveDataSet buffer = JsonConvert.DeserializeObject<ReceiveDataSet>(jsonString);
                GlobalData.ReceiveData = buffer;

                //Update 이벤트 발생하고 UI에 던져줄것.

            }
            catch (JsonException ex)
            {

            }
        }

        //메세지 발송부.
        public void SendJsonString(string strMsg)
        {
            //stx etx 삽입.
            string messagebuf = '\u0002' + strMsg + '\u0003';

            //Send로 이벤트 전달.
            byte[] buf = Encoding.ASCII.GetBytes(messagebuf);

            SendMsg(buf);
        }


        public void SendMsg(byte[] vMsg)
        {
            try
            {
                if (ConnectionSocket == false)
                {
                    clientSocket = new TCPClientAsync(GlobalData.ConfigData.IP, GlobalData.ConfigData.Port);

                    clientSocket.handler += ClientSocketHandler;

                    clientSocket.Connect();

                }
                else
                {
                    clientSocket.Send(vMsg);
                }

            }
            catch (Exception e)
            {

            }
        }





    }



}

