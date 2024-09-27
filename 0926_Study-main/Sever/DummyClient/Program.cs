using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Dummy Client!");
            string host = Dns.GetHostName();

            // 호스트 이름에 해당하는 IPHostEntry 객체를 가져옵니다.
            // 이 객체는 호스트의 IP 주소와 관련된 정보를 포함합니다.
            IPHostEntry iphost = Dns.GetHostEntry(host);

            // IPHostEntry에서 첫 번째 IP 주소를 가져옵니다.
            // 일반적으로 첫 번째 주소는 IPv4 주소입니다.
            IPAddress ipAddr = iphost.AddressList[0];

            // IP 주소와 포트 번호(1111)를 사용하여 IPEndPoint 객체를 생성합니다.
            // IPEndPoint는 네트워크 끝점을 나타내며, 소켓 통신에 사용됩니다.
            IPEndPoint endpoint = new IPEndPoint(ipAddr, 1111);

            Socket socket = new Socket(endpoint.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
           
            try
            {
                socket.Connect(endpoint);
                Console.WriteLine($"{socket.RemoteEndPoint.ToString()}");// RemoteEndPoint 객체를 문자열 형태로 변환 일반적으로
                                                                         // ip주소: 포트번호 형식으로 출력됌
                                                                         //보낸다.
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome RPG");
                int sendBytes = socket.Send(sendBuff);

                byte[] recvBuff = new byte[1024];
                int recyBtyes = socket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recyBtyes);
                //받는다.
                Console.WriteLine($"[From Server] {recvData}");
                socket.Shutdown(SocketShutdown.Both);

                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
        // 추가로 C:\Users\YONSAI\Documents\09-26Study\Sever\DummyClient\bin\Debug\net8.0이런식으로 들어가
        // DummyClient 계속 클릭하면 한명식 접속하는걸 볼 수 있음
    }
}
