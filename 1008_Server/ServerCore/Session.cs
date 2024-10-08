using System.Net.Sockets;
using System.Net;

namespace ServerCore
{
    public abstract class Session
    {
        Socket _socket;
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        //Queue<byte[]> _sendQueue = new Queue<byte[]>(); //보낼 데이터를 관리하는 방법으로 큐를 선택, Tcp로 교체하면서 변수 ArraySegment<byte> 변경
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>(); //보낼 데이터를 관리하는 방법으로 큐를 선택
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>(); //데이터를 한번에 보낼 리스트 선언

        ReceiveBuffer _receiveBuffer = new ReceiveBuffer(1024); //서버가 데이터를 수신할 버퍼 설정

        object _lock = new object();

        int _disConnected;

        #region 추상클래스로 만들어 상태를 체크하는 방법
        //Session 클래스를 추상클래스로 만들어 아래의 함수를 상속시켜 만든다.
        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        //public abstract void OnRecv(ArraySegment<Byte> buffer); //Tcp방식으로 데이터를 검사해야해서 반환형 int로 교체
        public abstract int OnRecv(ArraySegment<Byte> buffer);
        public abstract void OnSend(int numOfBytes);
        #endregion

        public void Start(Socket socket) //데이터를 받았을 때 변환하는 함수
        {
            _disConnected = 0; //socket을 재사용할 때 계속 초기화 (없으면 2번째 사용부터 소켓 종료가 되지 않음)
            _socket = socket;
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted); //버퍼에 데이터가 모두 송신됬다면 발생
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted); //버퍼에 데이터 수신이 완료되면 발생
            //recvArgs.SetBuffer(new byte[1024], 0, 1024); //TCP를 사용하기 위해 수신 버퍼를 따로 만들었음
            RegisterRecv(recvArgs);
        }
        //public void Send(byte[] sendBuff) //데이터를 보낼 때 변환하는 함수 (보내는 시점이 정해져 있지 않음) TCP로 교체하면서 바뀜
        public void Send(ArraySegment<byte> sendBuff) //데이터를 보낼 때 변환하는 함수 (보내는 시점이 정해져 있지 않음)
        {
            lock (_lock) //send에서 쓰레드는 데이터를 서로 먼저 보내려고 하므로 lock을 걸어 보낸다. (클라이언트가 여러개라 일어나는 일)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }
        public void Disconnect() //강제로 통신을 끊어야 할때 호출하는 함수
        {
            if (Interlocked.Exchange(ref _disConnected, 1) == 1) return;

            OnDisconnected(_socket.RemoteEndPoint); //접속을 끊어야하는 소켓의 endPoint를 넘긴다. (추상클래스에서 이용)

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        #region 네트워크 통신
        void RegisterSend() //논블로킹 - 비동기적으로 작동
        {

            while (_sendQueue.Count > 0) //버퍼리스트는 버퍼보다 한번에 많은 데이터를 보낼수 있는 방식으로 활용 가능하다.
            {
                //byte[] buff = _sendQueue.Dequeue(); //Tcp정보 교환으로 바뀌면서 ArraySegment<byte> 사용.
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                //힙영역이 아닌 스텍 영역에 복사되는 형태이다. 
                //_pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length)); //버퍼의 0인덱스부터 버퍼 크기의 인덱스까지의 데이터를 리스트에 저장,Tcp로 교체하면서 주석
                _pendingList.Add(buff); //리스트에 전달받은 ArraySegment<byte> 데이터 저장
            }

            sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(sendArgs); //전송과 동시에 전송 상태를 bool에 저장
            if (pending == false) //데이터가 모두 송신 되었을 경우
                OnSendCompleted(null, sendArgs);
        }
        void OnSendCompleted(object? sender, SocketAsyncEventArgs args) //데이터를 모두 보냈는지 관리
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) //버퍼에 저장된 데이터의 크기가 0보다 크고 오류없이 전송이 끝났을 때
                {
                    try
                    {
                        sendArgs.BufferList = null; //전송이 완료되면 버퍼리스트의 데이터를 전부 지운다.
                        _pendingList.Clear(); //전송이 완료되면 데이터를 저장했던 리스트를 초기화 한다.

                        OnSend(sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"OnSendCompleted Faild! {ex.Message}");
                    }
                }
                else
                {
                    Disconnect(); //잘못된 통신이므로 통신 종료
                }
            }
        }

        void RegisterRecv(SocketAsyncEventArgs args) //논블로킹 - 비동기적으로 작동
        {
            _receiveBuffer.Clean(); //데이터 수신할 때 마다 버퍼의 r,w위치 초기화
            ArraySegment<byte> segment = _receiveBuffer.WirteSegment; //데이터를 저장할 수 있는 버퍼 크기 반환 받아 할당.
            recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count); //수신 버퍼로 설정.

            bool pending = _socket.ReceiveAsync(args); //수신과 동시에 bool변수에 수신 상태 저장
            if (pending == false) //데이터가 모두 수신 되었을 경우
                OnRecvCompleted(null, args);
        }
        void OnRecvCompleted(object? sender, SocketAsyncEventArgs args) //byte -> string 변환 함수 (받은 데이터를 변환하여 읽음.)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    if(_receiveBuffer.OnWrite(args.BytesTransferred) == false)// 수신데이터가 알맞게 전송되지 않았다면 (검사하는 동안 false가 아니라면 w 이동)
                    {
                        Disconnect(); //연결 종료
                        return;
                    }

                    //OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred)); //Tcp과정으로 바꾸면서 주석처리됨.
                    int processLen = OnRecv(_receiveBuffer.ReadSegment); //수신된 데이터의 크기를 반환 (OnRecv함수 안에서 전송된 데이터 출력까지 진행.)
                    if(processLen < 0 || _receiveBuffer.DataSize < processLen) //수신된 데이터가 없거나, 데이터가 전부 전달되지 않았다면
                    {
                        Disconnect();
                        return;
                    }

                    if(_receiveBuffer.OnRead(processLen) == false) //읽는 데이터 크기가 수신된 데이커 크기보다 크다면 (검사하는 동안 false가 아니라면 r 이동)
                    {
                        Disconnect(); //연결 종료
                        return;
                    }

                    RegisterRecv(args); //데이터 처리가 끝나고 다시 데이터 수신 준비를 한다.
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OnRecvCompleted Faild! {ex.ToString()}");
                }

            }
            else
            {
                Disconnect(); //잘못된 통신이므로 통신 종료
            }
        }
        #endregion
    }
}
