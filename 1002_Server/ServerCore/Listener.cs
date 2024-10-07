using System.Net.Sockets;
using System.Net;


namespace ServerCore
{//원래는 서버에 존재해야함, 서버에는 로직만 존재해야함.
    public class Listener
    {
        Socket _listenSocket;
        //Action<Socket> _OnAcceptHandler; //매개변수가 Socket이고 반환형이 void인 대리자.
        Func<Session> _SessionFactory; //매개변수가 없고 반환형이 Session인 대리자.
        SocketAsyncEventArgs RecvArgs = new SocketAsyncEventArgs(); //비동기 Socket통신이며 콜백함수를 가질수 있고 해당 클래스는 매개변수로 사용가능하다.

        public void Init(IPEndPoint endPoint, Func<Session> SessionFactory) //초기화 함수 (추상클래스를 사용할 때는 Func, 아닐때 Action)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //Core에서 사용할 소켓 등록
            //_OnAcceptHandler += OnAcceptHandler; //Core에서 입력한 Action이 등록된다.
            _SessionFactory += SessionFactory; //Core에서 입력한 Func(=GameSession 클래스)가 등록된다.

            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(10);

            RecvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted); //해당 args가 완료되면 이벤트 발생시킴 (즉, 연결요청 수락 시 발생.)
            RegisterAccept(RecvArgs); //클라이언트의 연결요청을 비동기적으로 수락하는 함수이다.
        }
        void RegisterAccept(SocketAsyncEventArgs args)
        { //해당 함수는 메인 함수가 진행됨과 동시에 같이 실행되며, 콜백함수를 이용하여 메인함수에 값을 반환한다.

            args.AcceptSocket = null; //이벤트를 재사용 할 때 이전에 사용한 소켓을 지운다. (새로운 소켓을 받아들이기 위해)

            bool pending = _listenSocket.AcceptAsync(args); //비동기로 Connect요청을 처리중이면 true
            if (pending == false) //Connect요청을 모두 처리 했을 경우.
                OnAcceptCompleted(null, args);
        }
        void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success) //Connect요청을 아무런 오류없이 무사히 완료 했다면,
            {
                Session session = _SessionFactory.Invoke(); //Session을 상속한 GameSession클래스를 Session에 할당.
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);

                #region 추상클래스로 상속 받지 않았을 때
                //_OnAcceptHandler.Invoke(args.AcceptSocket); //서버, 클라이언트사이 데이터를 전송 시작.
                #endregion
            }
            else //Connect요청을 처리 도중 에러가 발생했다면,
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args); //계속 반복 호출된다. (지속적으로 다른 클라이언트 연결을 감지한다.)
        }
    }
}
