using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SeverCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Sever World!");
            // DWS (Domain Name Syteam) 이 없으면 오류가 뜬다.
            // 네이버 구글은 다 숫자로 파악하므로 
            //Socket listenSocket = Dns.GetHostName();
            // 현재 컴퓨터의 호스트 이름을 가져옵니다.
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

            // 지정된 주소 패밀리(AddressFamily), 소켓 유형(SocketType), 프로토콜(ProtocolType)을 사용하여
            // 소켓을 생성합니다. 여기서는 TCP 스트림 소켓을 생성합니다. 
            // 문지기: 소켓                           어드레스      // 통신 방식을 SocketType 
            Socket listenSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


            try
            {

          
            // 블로킹 vs 논블로킹 https://velog.io/@nittre/%EB%B8%94%EB%A1%9C%ED%82%B9-Vs.-%EB%85%BC%EB%B8%94%EB%A1%9C%ED%82%B9-%EB%8F%99%EA%B8%B0-Vs.-%EB%B9%84%EB%8F%99%EA%B8%B0
            // 문지기 교육 특정포트에 접속했을때 등록이 됐는지 확인
            listenSocket.Bind(endpoint);// 예시로 식당 주소인지 어떤 명령인지 주입시키는 함수
            // 영업 시작
            // backlog : 최대 대가수 지정
            listenSocket.Listen(10);
            #region GPT 요약본
            //            컴퓨터 이름을 찾는다.
            //그 이름으로 IP 주소를 얻는다.
            //첫 번째 IP 주소를 선택한다.
            //IP 주소와 포트 번호로 네트워크 끝점을 만든다.
            //TCP 소켓을 생성하여 다른 컴퓨터와 연결할 준비를 한다.
            #endregion
            while (true)
            {
                // 이해가 안되면 강사님 카톡에 예시본있어요
                Console.WriteLine("Listening.....");
                // 손님을 입장 시킨 걸 받음
                Socket clientSocket = listenSocket.Accept();
                // 수신받는다. 
                byte[] recyBuff = new byte[1024]; // 1패킷이 1024바이트
                // 클라이언트 소켓을 통해 데이터를 수신합니다.
                // 수신된 데이터는 recyBuff 배열에 저장되며, 실제로 수신된 바이트 수는 recyBytes 변수에 저장됩니다.
                int recyBytes = clientSocket.Receive(recyBuff);
                // 수신한 바이트 데이터를 UTF-8 형식의 문자열로 변환합니다.
                // recyBuff의 0번 인덱스부터 recyBytes만큼의 데이터를 사용해 문자열로 변환합니다.
                string recvData = Encoding.UTF8.GetString(recyBuff, 0, recyBytes);
                //https://namu.wiki/w/UTF-8 UTF-8이란?  가장 많이 사용되는 가변 길이 유니코드 인코딩
                Console.WriteLine($"[From Client] {recvData}");
                    // 수신한 문자열 데이터를 UTF-8 형식의 바이트 배열로 변환합니다.
                    // 이 배열은 클라이언트에게 다시 전송하기 위해 사용됩니다.
                 
                byte[] sendBuff = Encoding.UTF8.GetBytes($"Welcome to MMORPG Sever");
                
                    // 변환한 바이트 배열을 클라이언트 소켓을 통해 전송합니다.
                    clientSocket.Send(sendBuff);

                // 소켓 통신을 종료합니다.
                // 양쪽 모두(클라이언트와 서버)에서 소켓을 닫는 것을 의미합니다.
                clientSocket.Shutdown(SocketShutdown.Both);

                // 클라이언트 소켓을 완전히 닫습니다.
                // 이를 통해 소켓 리소스를 해제하고, 더 이상 사용할 수 없게 만듭니다.
                clientSocket.Close();
            }
                // 보낸다 송신
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
