using System.Net;
using System.Net.Sockets;

namespace ServerCore
{ //원래는 서버에 존재해야함, 서버에는 로직만 존재해야함.
    public class Connector
    {
        Func<Session> _SessionFactory;
        public void Connect(IPEndPoint endPoint, Func<Session> SessionFactory)
        {
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _SessionFactory = SessionFactory;
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectedCompleted;
            args.RemoteEndPoint = endPoint;
            args.UserToken = socket; //이벤트로 소켓(socket)을 args에 할당한다.

            RegisterConnect(args);
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket; //이벤트로 전달 받은 소켓을 할당한다.
            if (socket == null) return; //이벤트로 소켓을 전달 받지 못하면 리턴

            bool pending = socket.ConnectAsync(args);
            if (pending == false) //소켓에 더이상 접속요청을 받고 있지 않다면 (모두 접속 처리했거나 접속을 막았다면)
                OnConnectedCompleted(null, args);
        }
        void OnConnectedCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success) //에러 없이 모든 요청을 완료 했을 경우
            {
                Session session = _SessionFactory.Invoke(); //클라이언트에서 해줘야할텐ㄷ..
                session.Start(args.ConnectSocket); //args에 연결을 요청하는 소켓을 세션으로 보낸다. (그 소켓은 start함수에서 정해짐)
                session.OnConnected(args.RemoteEndPoint); //연결이 됬다면 클라이언트에 메세지 송신.
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted Fail: {args.SocketError}");
            }
        }
    }
}
