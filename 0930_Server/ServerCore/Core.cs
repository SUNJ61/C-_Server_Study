using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    internal class Core
    {
        static Listener _listener = new Listener(); //소켓 초기화를 하는 클래스
        static void OnAcceptHandler(Socket clientSocket) //대리자를 통해 코드를 Listener에 넘겼다.
        {
            try
            {
                //데이터 받기
                byte[] recvBuff = new byte[1024]; //데이터를 받는다. (1패킷 = 1024 byte) #서버는 항상 connet요청을 받은 후 요청이 확인되면 데이터를 받는 작업을 해야한다.
                int recvbytes = clientSocket.Receive(recvBuff); //단일 쓰레드 방식, 데이터를 모두 받을 때 까지 멈춰있음 (다음 코드로 넘어가지 않음?)
                                                                //Receive()함수를 통해 임시로 클라이언트에서 전송한 데이터를 recvBuff에 저장한다.
                                                                //저장한 데이터의 크기를 recvbytes에 저장한다.
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvbytes); //변환 방식, 전세계 모든 문자의 유니코드를 포함하고 있음.
                                                                                   //recvBuff에 저장된 데이터를 recvbytes만큼 읽어와서 문자열로 변환하며, 변환된 데이터는 recvData에 저장됩니다.

                Console.WriteLine($"[From Clinet] {recvData}"); //클라이언트가 보낸 메세지 출력

                //데이터 보내기
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Sever"); //string 값을 byte형태로 바꾸어 sendBuff에 저장.
                clientSocket.Send(sendBuff); //sendBuff를 클라리언트에게 전송
                clientSocket.Shutdown(SocketShutdown.Both); //서버와 클라이언트 사이의 통신을 끊는다.

                clientSocket.Close(); //할당한 소켓지원을 닫는다. (해당 소켓을 비운다.)
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("ServerCore Hello, World!");

            //DNS : (Domain Name System) 주소, 데이터가 향하는 목적지의 이름
            string host = Dns.GetHostName(); //Dns서버에 저장된 본인의 컴퓨터 이름을 반환하여 문자열에 저장
            IPHostEntry ipHost = Dns.GetHostEntry(host); //Dns서버에 본인의 컴퓨터 이름을 통하여 호스트로 등록한다. (여기서 해당 호스트의 IP주소등을 가져온다.)
            IPAddress ipAddress = ipHost.AddressList[0]; //IP주소를 가져온다. (ipHost는 IPv4 및 IPv6 주소가 포함, 여기서는 첫번째 IP 주소를 사용한다.)
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 2222); //IP 주소와 접속 port를 입력.

            #region Listener 클래스에서 선언됨
            //Socket listenSocket = //버퍼 역할을 한다. (데이터가 들어오는 것을 관리) = 문지기
            //new Socket(endPoint.AddressFamily,SocketType.Stream,ProtocolType.Tcp); //endPoint.AddressFamily는 endPoint의 주소와 port를 가져오는 프로퍼티
            //SocketType.Stream는 데이터 중복이나 경계 유지 없이 신뢰성 있는 양방향 연결 기반의 바이트 스트림을 지원합니다. (직렬화, 역직렬화)

            //listenSocket.Bind(endPoint); //해당 버퍼가 해당 서버 주소와 데이터가 들어오는 입구를 관리하도록 지정 (endPoint의 IP주소와 port를 가지고 있어야 접속이 가능하다.)
            //listenSocket.Listen(10); //최대 10개의 클라이언트 접속요청만 기다리게 한다.
            #endregion
            try
            {
                _listener.Init(endPoint, OnAcceptHandler); //소켓을 endPoint를 이용하여 초기화
                Console.WriteLine("Listening......");

                while (true) // 서버는 항상 클라이언트의 접속을 대비하기 위해 무한 루프를 돌려야한다.
                {
                    //Console.WriteLine("Listening......"); //대기중을 의미하는 문구

                    //Socket clientSocket = listenSocket.Accept(); //서버에 접속하려는 클라이언트를 listenSocket이 감지하고 감지되면 clientSocket에 저장한다. (클라이언트가 Connet 요청을 하면 해당 클라이언트를 저장)
                    //Socket clientSocket = _listener.Accept(); //클래스에서 선언된 함수를 통해 클라이언트 접속 감지 (클래스 분리후 Accept()를 이용하여 할당)

                    #region 단일 쓰레드를 이용한 통신
                    ////데이터 받기
                    //byte[] recvBuff = new byte[1024]; //데이터를 받는다. (1패킷 = 1024 byte) #서버는 항상 connet요청을 받은 후 요청이 확인되면 데이터를 받는 작업을 해야한다.
                    //int recvbytes = clientSocket.Receive(recvBuff); //단일 쓰레드 방식, 데이터를 모두 받을 때 까지 멈춰있음 (다음 코드로 넘어가지 않음?)
                    //                                                //Receive()함수를 통해 임시로 클라이언트에서 전송한 데이터를 recvBuff에 저장한다.
                    //                                                //저장한 데이터의 크기를 recvbytes에 저장한다.
                    //string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvbytes); //변환 방식, 전세계 모든 문자의 유니코드를 포함하고 있음.
                    //                                                                   //recvBuff에 저장된 데이터를 recvbytes만큼 읽어와서 문자열로 변환하며, 변환된 데이터는 recvData에 저장됩니다.

                    //Console.WriteLine($"[From Clinet] {recvData}"); //클라이언트가 보낸 메세지 출력

                    ////데이터 보내기
                    //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Sever"); //string 값을 byte형태로 바꾸어 sendBuff에 저장.
                    //clientSocket.Send(sendBuff); //sendBuff를 클라리언트에게 전송
                    //clientSocket.Shutdown(SocketShutdown.Both); //서버와 클라이언트 사이의 통신을 끊는다.

                    //clientSocket.Close(); //할당한 소켓지원을 닫는다. (해당 소켓을 비운다.)
                    #endregion
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}
