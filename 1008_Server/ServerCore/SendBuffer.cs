
namespace ServerCore
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; }); //싱글턴으로 해당 클래스 접근.
        public static int ChunkSize { get; set; } = 4096 * 100;
        public static ArraySegment<byte>Open(int reserveSize)
        {
            if(CurrentBuffer.Value == null) //해당 스레드의 버퍼의 값이 없다면
                CurrentBuffer.Value = new SendBuffer(ChunkSize); //버퍼의 크기를 4096 * 100으로 설정
            if (CurrentBuffer.Value.FreeSize < reserveSize) //해당 스레드의 버퍼의 빈공간이 reserveSize보다 크다면
                CurrentBuffer.Value = new SendBuffer(ChunkSize); //버퍼의 크기를 4096 * 100으로 설정

            return CurrentBuffer.Value.Open(reserveSize); //해당 스레드에 속한 버퍼의 속한 Open함수 호출 (SendBuffer클래스에 정의됨)
        }
        public static ArraySegment<byte>Close(int usedsize) => CurrentBuffer.Value.Close(usedsize); //해당 스레드에 속한 버퍼의 Close함수 호출 (SendBuffer클래스에 정의됨)
    }
    //ReceiveBuffer는 세션과 1대1 관계여서 내부적으로 관리가 가능하다.
    //SendBuffer는 내부에 존재하지 않고 밖으로 빠져나와 있다.
    public class SendBuffer
    {
        byte[] _buffer;
        int _usedSize = 0; //사용중인 버퍼 공간
        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }
        public int FreeSize //버퍼의 남은 공간을 반환
        {
            get { return _buffer.Length - _usedSize; }
        }
        public ArraySegment<byte> Open(int reserveSize) //버퍼에게 요구한 공간(reservesize)이 지금 남은 공간보다 크면 버퍼공간 반환 불가이므로 null반환 
        {
            if (reserveSize > FreeSize)
                return null;

            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize); //_buffer에 _usedSize인덱스부터 reserveSize길이 만큼 배열 반환
        }
        public ArraySegment<byte> Close(int usedsize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedsize); //_buffer에 _usedSize인덱스부터 usesize길이 만큼 배열 반환
            _usedSize += usedsize; //usesize만큼 버퍼공간을 더 사용한다.
            return segment;
        }
    }
}
