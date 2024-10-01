using System;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    internal class Session
    {
        Socket _socket;
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        Queue<byte[]> _sendQueue = new Queue<byte[]>(); //보낼 데이터를 관리하는 방법으로 큐를 선택
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>(); //데이터를 한번에 보낼 리스트 선언

        object _lock = new object();

        int _disConnected;

        bool _pending = false;
        public void Start(Socket socket) //데이터를 받았을 때 변환하는 함수
        {
            _disConnected = 0; //socket을 재사용할 때 계속 초기화 (없으면 2번째 사용부터 소켓 종료가 되지 않음)
            _socket = socket;
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted); //버퍼에 송신 데이터가 모두 들어왔다면 발생
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted); //버퍼에 데이터 수신이 완료되면 발생하는 이벤트
            recvArgs.SetBuffer(new byte[1024], 0, 1024); //recvArgs에 소켓을 관리하는 버퍼할당, 1024바이트 공간 할당, offset = 0, 최대 1024크기의 데이터를 받을 수 있다.
            RegisterRecv(recvArgs);
        }
        public void Send(byte[] sendBuff) //데이터를 보낼 때 변환하는 함수 (보내는 시점이 정해져 있지 않음)
        {
            lock (_lock) //send에서 쓰레드는 데이터를 서로 먼저 보내려고 하므로 lock을 걸어 보낸다.
            {
                _sendQueue.Enqueue(sendBuff); //큐에 sendBuff 데이터를 넣는다.
                #region 적은 데이터를 보내는 방식
                //if (_pending == false)
                //    RegisterSend();
                #endregion
                if (_pendingList.Count == 0) //모든 데이터가 보내져서 List가 비었을 때
                    RegisterSend();
            }
        }
        public void Disconnect() //강제로 통신을 끊어야 할때 호출하는 함수
        {
            if (Interlocked.Exchange(ref _disConnected, 1) == 1) return; //_disConnected의 값이 1이라면 접속종료 실행 x (if문에 들어오면 1로 바뀐다. == 한번만 실행됨)
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        #region 네트워크 통신
        void RegisterSend() //논블로킹 - 비동기적으로 작동
        {
            #region 데이터를 조금씩 보내는 방법
            //_pending = true; //한번에 하나의 데이터만 처리하여 보내기위해 _pending을 true로 바꾼다.

            //byte[] buff =  _sendQueue.Dequeue(); //큐에 가장 먼저 담겼던 데이터 하나를 buff에 저장한다.
            //sendArgs.SetBuffer(buff, 0, buff.Length); //받은 데이터를 sendArgs의 버퍼에 저장한다. 하나씩 데이터를 보내는 방법이다.
            #endregion

            while (_sendQueue.Count > 0) //버퍼리스트는 버퍼보다 한번에 많은 데이터를 보낼수 있는 방식으로 활용 가능하다.
            {
                byte[] buff =  _sendQueue.Dequeue();
                //힙영역이 아닌 스텍 영역에 복사되는 형태이다. 
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length)); //버퍼의 0인덱스부터 버퍼 크기의 인덱스까지의 데이터를 리스트에 저장
            }

            sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(sendArgs); //Async는 쓰레드를 어느정도 조절해준다. 과부화 걱정을 좀 덜어도 됨. 전송과 동시에 전송 상태를 bool에 저장
            if (pending == false) //데이터가 모두 송신 되었을 경우
                OnSendCompleted(null, sendArgs);
        }
        void OnSendCompleted(object? sender, SocketAsyncEventArgs args) //데이터를 모두 보냈는지 관리
        {
            lock (_lock) //위에서 lock으로 진입했는데 왜 또 사용하는가?
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) //버퍼에 저장된 데이터의 크기가 0보다 크고 오류없이 전송이 끝났을 때
                {
                    try
                    {
                        Console.WriteLine($"Transferred byte : {sendArgs.BytesTransferred}"); //전송된 데이터의 크기를 출력한다.
                        sendArgs.BufferList = null; //전송이 완료되면 버퍼리스트의 데이터를 전부 지운다.
                        _pendingList.Clear(); //전송이 완료되면 데이터를 저장했던 리스트를 초기화 한다.

                        #region 적은 데이터를 보내는 방법
                        //if (_sendQueue.Count > 0) //큐에 들어온 데이터가 1개보다 많을 경우
                        //{
                        //    RegisterSend(); //다시 전송을 진행. 
                        //}
                        //else
                        //_pending = false; //전송이 완료 됬으므로 다시 전송가능 상태로 만든다.
                        #endregion
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
            bool pending = _socket.ReceiveAsync(args); //수신과 동시에 bool변수에 수신 상태 저장
            if (pending == false) //데이터가 모두 수신 되었을 경우
                OnRecvCompleted(null, args);
        }
        void OnRecvCompleted(object? sender, SocketAsyncEventArgs args) //byte -> string 변환 함수 (받은 데이터를 변환하여 읽음.)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success) //수신된 데이터가 0byte보다 크고, 데이터를 받을 때 오류가 없었다면
            {
                try
                {
                    string recvData = //recvArgs의 버퍼에서 recvArgs의 Offset(시작위치 = 0)에서 recvArgs의 수신한 데이터의 크기만큼 데이터를 변환한다.
                    Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);

                    Console.WriteLine($"[From Client]\n{recvData}");
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
