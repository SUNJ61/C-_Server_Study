using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _1008_Server
{
    class Knight //다음과 같은 클래스 정보를 전송
    {
        public int hp; //4byte 데이터 전송
        public int attack;
        //public string name; //가변길이의 변수 (버퍼의 크기를 한정할 수 없음.)
        public List<int> skills = new List<int>();
    }
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint) //Listener클래스에서 호출, Listener가 연결을 요청하기 때문
        {//즉, 연결이 완료 되었을 때 아래 함수 진행 (클라이언트에 메세지를 보내고 접속을 끊음.)
            Console.WriteLine($"OnConnected : {endPoint}");

            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Sever"); //string이 아닌 데이터 전송을 위해 주석처리
            //byte[] openSegment = new byte[4096]; //보통 4096의 크기를 할당한다, 미리 큰 공간을 할당하고 데이터를 짤라서 보내는 형식으로 진행.  TCP형태로 변경하면서 주석처리
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

            Knight knight = new Knight() { hp = 100, attack = 10 }; //int 데이터 다수를 전송
            byte[] buffer1 = BitConverter.GetBytes(knight.hp); //int값 1개를 byte로 변환
            byte[] buffer2 = BitConverter.GetBytes(knight.attack); //int값 1개를 byte로 변환
            Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length); //buffer1의 0인덱스 데이터부터 4개를  0(openSegment.Offset)인덱스부터 저장
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length); //buffer2의 0인덱스 데이터부터 4개를 openSegment.Array의 4인덱스부터 저장
            ArraySegment<byte> sendBuffer = SendBufferHelper.Close(buffer1.Length + buffer2.Length); //서버 버퍼에 저장된 데이터를 반환 받고, 서버 버퍼 사용 크기 증가.

            //Send(sendBuff); //데이터 송신을 위한 초기화 및 송신 작업, 상속된 클래스안에 구현되어 있음, TCP작업을 하면서 주석처리
            Send(sendBuffer); //위에서 반환된 데이터를 전송한다.

            Thread.Sleep(1000); //1초간 쓰레드 정지
            Disconnect(); //통신 종료, 상속된 클래스안에 구현되어 있음.
        }

        public override void OnDisconnected(EndPoint endPoint) //Session클래스에서 호출, Session이 서버와 클라이언트사이의 통신을 끊기 때문
        {//단순히 접속이 끊이면 아래의 문구 발생
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        //public override void OnRecv(ArraySegment<byte> buffer) //Session클래스에서 호출, 서버 버퍼에 데이터가 모두 수신 되었을 경우 실행.
        public override int OnRecv(ArraySegment<byte> buffer) //Session클래스에서 호출, 수신된 데이터의 크기를 검사하기 위해 int 반환형 적용
        {//클라이언트에게 받은 메세지를 출력하는 코드
            string recvData =
            Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);

            Console.WriteLine($"[From Client]\n{recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes) //Session클래스에서 호출, 서버 버퍼에 클라이언트에게 보낼 메세지를 전부 송신했을 경우 실행.
        {//클라이언트에게 송신한 메세지의 크기를 출력하는 코드 (보낸메세지는 OnRecv에서 출력 => 클라이언트에서 출력)
            Console.WriteLine($"Transferred byte : {numOfBytes}"); //해당 문구는 서버에서 출력
        }
    }

    internal class Server
    {
        static Listener _listener = new Listener(); //클라이언트 연결관리

        static void Main(string[] args)
        {
            Console.WriteLine("I'm ServerCore Hello, World!");

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 2222); //IP 주소와 접속 port를 입력.

            try
            {
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
