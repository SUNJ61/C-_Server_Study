using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    internal class Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DummyClient Hello, World!");
            string host = Dns.GetHostName(); //Dns서버에 저장된 본인의 컴퓨터 이름을 반환하여 문자열에 저장
            IPHostEntry ipHost = Dns.GetHostEntry(host); //Dns서버에 본인의 컴퓨터 이름을 통하여 호스트로 등록한다. (여기서 해당 호스트의 IP주소등을 가져온다.)
            IPAddress ipAddress = ipHost.AddressList[0]; //IP주소를 가져온다. (ipHost는 IPv4 및 IPv6 주소가 포함, 여기서는 첫번째 IP 주소를 사용한다.)
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 2222); //IP 주소와 접속 port를 입력.

            while (true) //일정 시간마다 계속 접속요청을 함.
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //접속 소켓 할당

                try //서버 요청중 오류가 발생 할 수 있으므로 try catch문 안에 넣는다.
                {
                    socket.Connect(endPoint); //서버에 접속 요청을 한다. (서버에 있는 소켓한테 접속 요청)
                    Console.WriteLine($"Connected To: {socket.RemoteEndPoint}"); //socket의 IP주소와 포트번호가 출력된다.

                    //클라이언트는 먼저 데이터를 보낸다.
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Hello!! World!!!\n"); //해당 string 데이터를 변환. (코딩에서 인코딩 : string -> byte / 디코딩 : byte -> string)
                    int sendBytes = socket.Send(sendBuff);

                    //서버로부터 데이터를 받는다.
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Server] {recvData}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
                Thread.Sleep(100); //접속 요청하는 쓰레드를 잠깐 쉰다. 0.1초
            }
        }
    }
}
