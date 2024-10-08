
namespace ServerCore
{
    public class ReceiveBuffer
    {
        ArraySegment<byte> _buffer; // [r] [] [] [w] [] [] [] [] (readPos와 writePos를 조절하여 데이터를 읽고 쓴다. 일정시간마다 r,w위치를 초기화, r과 w가 만나면 초기화등 계속 초기화함.)
                                    //만약 데이터가 5byte가 왔을 때 w가 5까지 진행 됬을 때 r이 읽기를 진행, w가 5가되지 않는다면 r은 기다렸다가 w가 다시 정상적으로 쓰여지면 r을 진행.
        int _readPos; //버퍼안 해당 인덱스부터 쓰기 인덱스 전까지의 데이터를 읽는다.
        int _writePos; //버퍼안 해당 인덱스부터 이후로 데이터를 쓴다. (버퍼에 데이터 저장)
        public ReceiveBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize); //입력된 크기의 버퍼를 만든다.
        }

        public int DataSize //클라이언트에서 보낸 byte를 읽을수 있는 유효 크기를 반환하는 프로퍼티
        {
            get { return _writePos - _readPos; }
        }
        public int FreeSize //버퍼의 남은 byte공간 수를 반환하는 프로퍼티
        {
            get { return _buffer.Count - _writePos; }
        }
        public ArraySegment<byte> ReadSegment //버퍼에서 데이터를 읽을 수 있는 범위
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); } //버퍼에 0+readPos 인덱스부터 DataSize개의 데이터를 반환 
        }
        public ArraySegment<byte> WirteSegment //버퍼에서 데이터를 읽지 않은 부분
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); ; } //버퍼의 0+wirtePos 인덱스부터 FreeSize개의 데이터를 반환 
        }
        public void Clean() //읽고 쓰기 위치 초기화
        {
            int datasize = DataSize;
            if (DataSize == 0) //r,w가 겹친경우 (데이터 전송이 완료된 상황)
                _readPos = _writePos = 0; //남은 데이터가 없으므로 rw위치를 처음 위치로 초기화.
            else //남은 데이터가 있을 경우, 데이터와 r,w를 모두 시작위치로 복사
            {
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, datasize);
                //버퍼의 0+readPos에 인덱스까지 데이터를 복사하여 버퍼의 0위치로 이동하고 datasize의 크기를 가지게 한다.
                _writePos = datasize;
            }

        }
        public bool OnRead(int numOfByte) //제대로 읽고있는지 확인.
        {
            if (numOfByte > DataSize) //읽을 데이터크기(DataSize)가 입력된 데이터(numOfByte) 크기보다 작을 경우
                return false;

            _readPos += numOfByte; //읽기가 끝나서 r 이동
            return true;
        }

        public bool OnWrite(int numOfByte) //클라이언트에서 제대로 데이터가 전송됬는지 확인. (즉, 버퍼에 데이터가 제대로 입력되었는지를 확인.)
        {
            if (numOfByte > FreeSize) //남은 데이터크기(FreeSize)가 입력된 데이터(numOfByte) 크기보다 작을 경우
                return false; //이전에 데이터를 전송하고 남은 데이터 크기 or 버퍼보다 더 큰 데이터가 들어오면 아래에서 Array범위를 벗어나기 때문에 검사 

            _writePos += numOfByte;
            return true;
        }
    }
}
