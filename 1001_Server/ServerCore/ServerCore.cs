using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    internal class ServerCore
    {
        static Listener _listener = new Listener(); //클라이언트 연결관리, 데이터 송수신(OnAcceptHandler를 이용)
        static Session session = new Session(); //데이터 수신, 데이터 송신 처리 (OnAcceptHandler안에서 사용 )
        static void OnAcceptHandler(Socket clientSocket) //대리자를 통해 코드를 Listener에 넘겼다.
        {
            try
            {
                #region 블로킹 방식의 데이터 송수신
                ////데이터 받기
                //byte[] recvBuff = new byte[1024];
                //int recvbytes = clientSocket.Receive(recvBuff); //블로킹 함수 (논블로킹으로 바꾸어야함)
                //string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvbytes);

                //Console.WriteLine($"[From Clinet] {recvData}");

                ////데이터 보내기
                //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Sever");
                //clientSocket.Send(sendBuff); //블로킹 함수 (논블로킹으로 바꾸어야함)
                //clientSocket.Shutdown(SocketShutdown.Both); 

                //clientSocket.Close(); //할당한 소켓지원을 닫는다. (해당 소켓을 비운다.)
                #endregion
                #region 논블로킹-비동기 방식의 데이터 송수신
                session.Start(clientSocket); //데이터 수신을 위한 초기화 및 수신, 수신데이터 변환 작업

                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Sever");
                session.Send(sendBuff); //데이터 송신을 위한 초기화 및 송신 작업

                Thread.Sleep(1000); //1초간 쓰레드 정지
                session.Disconnect(); //통신 종료
                session.Disconnect(); //통신 종료
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("ServerCore Hello, World!");

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host); 
            IPAddress ipAddress = ipHost.AddressList[0]; 
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 2222); //IP 주소와 접속 port를 입력.

            try
            {
                _listener.Init(endPoint, OnAcceptHandler); //소켓을 endPoint를 이용하여 초기화, 해당 소켓의 접속을 처리
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
