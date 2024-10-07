using ServerCore; //추상클래스 Session에 접근하기 위해 할당
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace DummyClient
{
    class GameSession : Session //더미 클라이언트에서도 세션에 접속하기 위해 선언.
    {
        public override void OnConnected(EndPoint endPoint) //클라이언트에서 접속 신청을 한다. (세션클래스를 거쳐 실행)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            for (int i = 0; i < 5; i++)
            {
                //클라이언트는 먼저 데이터를 보낸다.
                byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello!! World!!! [{i}]\n"); //해당 string 데이터를 변환.
                Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint) //Session클래스에서 호출, Session이 서버와 클라이언트사이의 통신을 끊기 때문
        {//단순히 접속이 끊이면 아래의 문구 발생
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer) //Session클래스에서 호출, 서버 버퍼에 데이터가 모두 수신 되었을 경우 실행.
        {//서버에게 받은 메세지를 출력하는 코드
            string recvData =
            Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);

            Console.WriteLine($"[From Server]\n{recvData}");
        }

        public override void OnSend(int numOfBytes) //Session클래스에서 호출, 서버 버퍼에 클라이언트에게 보낼 메세지를 전부 송신했을 경우 실행.
        {//서버에게 송신한 메세지의 크기를 출력하는 코드 (보낸메세지는 OnRecv에서 출력 => 서버에서 출력)
            Console.WriteLine($"Transferred byte : {numOfBytes}"); //해당 문구는 클라이언트에서 출력
        }
    }

    internal class Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DummyClient Hello, World!");

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 2222); //IP 주소와 접속 port를 입력.

            Connector connect = new Connector();
            

            while (true) //일정 시간마다 계속 접속요청을 함.
            {
                connect.Connect(endPoint, () => { return new GameSession(); });
                #region 추상클래스로 서버통신 이전 코드
                //Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //접속 소켓 할당

                try
                {
          
                    //socket.Connect(endPoint); //서버에 접속 요청을 한다. (서버에 있는 소켓한테 접속 요청)
                    //Console.WriteLine($"Connected To: {socket.RemoteEndPoint}"); //socket의 IP주소와 포트번호가 출력된다.
                    
                    //    //클라이언트는 먼저 데이터를 보낸다.
                    //for (int i = 0; i < 5; i++)
                    //{
                    //    byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello!! World!!! [{i}]\n"); //해당 string 데이터를 변환.
                    //    int sendByte = socket.Send(sendBuff);
                    //}

                    ////서버로부터 데이터를 받는다.
                    //byte[] recvBuff = new byte[1024];
                    //int recvBytes = socket.Receive(recvBuff);
                    //string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    //Console.WriteLine($"[From Server] {recvData}");
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
                #endregion
                Thread.Sleep(1000); //접속 요청하는 쓰레드를 잠깐 쉰다. 0.1초
            }
        }
    }
}
