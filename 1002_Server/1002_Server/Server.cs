using ServerCore; //ServerCore 프로젝트 아래 클래스를 참조 가능
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _1002_Server
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint) //Listener클래스에서 호출, Listener가 연결을 요청하기 때문
        {//즉, 연결이 완료 되었을 때 아래 함수 진행 (클라이언트에 메세지를 보내고 접속을 끊음.)
            Console.WriteLine($"OnConnected : {endPoint}");
            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Sever");
            Send(sendBuff); //데이터 송신을 위한 초기화 및 송신 작업, 상속된 클래스안에 구현되어 있음

            Thread.Sleep(1000); //1초간 쓰레드 정지
            Disconnect(); //통신 종료, 상속된 클래스안에 구현되어 있음.
        }

        public override void OnDisconnected(EndPoint endPoint) //Session클래스에서 호출, Session이 서버와 클라이언트사이의 통신을 끊기 때문
        {//단순히 접속이 끊이면 아래의 문구 발생
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer) //Session클래스에서 호출, 서버 버퍼에 데이터가 모두 수신 되었을 경우 실행.
        {//클라이언트에게 받은 메세지를 출력하는 코드
            string recvData =
            Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);

            Console.WriteLine($"[From Client]\n{recvData}");
        }

        public override void OnSend(int numOfBytes) //Session클래스에서 호출, 서버 버퍼에 클라이언트에게 보낼 메세지를 전부 송신했을 경우 실행.
        {//클라이언트에게 송신한 메세지의 크기를 출력하는 코드 (보낸메세지는 OnRecv에서 출력 => 클라이언트에서 출력)
            Console.WriteLine($"Transferred byte : {numOfBytes}"); //해당 문구는 서버에서 출력
        }
    }

    internal class Core
    {
        static Listener _listener = new Listener(); //클라이언트 연결관리, 데이터 송수신(OnAcceptHandler를 이용)
        #region 세션을 추상클래스에 추상메서드화 한것을 상속하지 않았을 때        
        static void OnAcceptHandler(Socket clientSocket) //대리자를 통해 코드를 Listener에 넘겼다.
        {
            try
            {
                #region 논블로킹-비동기 방식의 데이터 송수신 (추상클래스 사용하지 않은 방식)
                //Session session = new Session(); //데이터 수신, 데이터 송신 처리 (OnAcceptHandler안에서 사용)
                //session.Start(clientSocket); //데이터 수신을 위한 초기화 및 수신, 수신데이터 변환 작업

                //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Sever");
                //session.Send(sendBuff); //데이터 송신을 위한 초기화 및 송신 작업

                //Thread.Sleep(1000); //1초간 쓰레드 정지
                //session.Disconnect(); //통신 종료
                //session.Disconnect(); //통신 종료 Session의 알고리즘에서 2번 진입을 막아줌.
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion
        static void Main(string[] args)
        {
            Console.WriteLine("I'm ServerCore Hello, World!");

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 2222); //IP 주소와 접속 port를 입력.

            try
            {
                //_listener.Init(endPoint, OnAcceptHandler); //소켓을 endPoint를 이용하여 초기화, 해당 소켓의 접속을 처리, 추상클래스가 아닐때
                _listener.Init(endPoint, () => { return new GameSession(); });
                Console.WriteLine("Listening......");

                while (true) // 서버는 항상 클라이언트의 접속을 대비하기 위해 무한 루프를 돌려야한다.
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}
