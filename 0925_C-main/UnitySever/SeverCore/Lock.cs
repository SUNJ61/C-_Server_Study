using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeverCore
{
    // 재귀적으로 lock을 허용 할 것인지 정해야 된다
    // 스핀락 정책 (5000번 시도 했을 경우 안될 확률이 높으므로 --> while 할 예정이다)
    internal class Lock
    {
        // lock 를 획득 했을 때 여러 쓰레드들이 동시에
        // read를 잡을 수 있다.
        // [Unused(1)] [ ] [ WriteThreadId 15비트] [ReadCount (16비트)]
        // write lock 같은 경우 한번에 쓰레드만 획득 가능
        const int EMPTY_FLAG = 0X00000;

        int _flag;



    }
}
