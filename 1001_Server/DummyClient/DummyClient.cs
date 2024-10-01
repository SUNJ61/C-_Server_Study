using System.Net.Sockets;
using System.Net;
using System.Text;

namespace DummyClient
{
    internal class DummyClient
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DummyClient Hello, World!");

            string host = Dns.GetHostName(); 
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList[0]; 
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 2222); //IP 주소와 접속 port를 입력.

            while (true) //일정 시간마다 계속 접속요청을 함.
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //접속 소켓 할당

                try
                {
                    socket.Connect(endPoint); //서버에 접속 요청을 한다. (서버에 있는 소켓한테 접속 요청)
                    Console.WriteLine($"Connected To: {socket.RemoteEndPoint}"); //socket의 IP주소와 포트번호가 출력된다.

                    for (int i = 0; i < 5; i++)
                    {
                        //클라이언트는 먼저 데이터를 보낸다.
                        byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello!! World!!! [{i}]\n"); //해당 string 데이터를 변환.
                        int sendByte = socket.Send(sendBuff);
                    }
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
