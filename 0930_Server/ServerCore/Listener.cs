using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    internal class Listener //메인에 있는 서버를 클래스로 분리하여 제작한다.
    {
        Socket _listenSocket;
        Action<Socket> _OnAcceptHandler; //매개변수가 Socket이고 반환형이 void인 대리자.
        SocketAsyncEventArgs RecvArgs = new SocketAsyncEventArgs(); //비동기 Socket통신이며 콜백함수를 가질수 있고 해당 클래스는 매개변수로 사용가능하다.

        public void Init(IPEndPoint endPoint, Action<Socket> OnAcceptHandler) //초기화 함수
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //Core에서 사용할 소켓 등록
            _OnAcceptHandler += OnAcceptHandler; //Core에서 입력한 Action이 등록된다.

            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(10);

            RecvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted); //해당 args가 완료되면 이벤트 발생시킴 (즉, 연결요청 수락 시 발생.)
            RegisterAccept(RecvArgs); //클라이언트의 연결요청을 비동기적으로 수락하는 함수이다.
        }
        void RegisterAccept(SocketAsyncEventArgs args) //매개변수는 비동기 - 논블로킹을 이용하기 위해 사용.
        { //해당 함수는 메인 함수가 진행됨과 동시에 같이 실행되며, 콜백함수를 이용하여 메인함수에 값을 반환한다.
            
            args.AcceptSocket = null; //이벤트를 재사용 할 때 이전에 사용한 소켓을 지운다. (새로운 소켓을 받아들이기 위해)
            
            bool pending =  _listenSocket.AcceptAsync(args); //비동기로 Connect요청을 처리중이면 true
            if(pending == false) //Connect요청을 모두 처리 했을 경우.
                OnAcceptCompleted(null, args);
        }
        void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success) //Connect요청을 아무런 오류없이 무사히 완료 했다면,
            {
                _OnAcceptHandler.Invoke(args.AcceptSocket); //클라이언트에게 데이터를 전송한다.
            }
            else //Connect요청을 처리 도중 에러가 발생했다면,
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args); //계속 반복 호출된다. (지속적으로 다른 클라이언트 연결을 감지한다.)
        }

        //public Socket Accept() // 동기로 connect요청이 있는지 확인함.
        //{
        //    return _listenSocket.Accept();
        //}
    }
}
