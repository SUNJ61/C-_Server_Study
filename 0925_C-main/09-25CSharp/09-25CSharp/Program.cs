namespace _09_25CSharp
{
    //Lock
    //임계 영역(Critical section)에 진입이 불가능할 때 진입이 가능할 때까지 루프를 돌면서 재시도하는 방식으로 구현된 락을 가리킨다.
    //이름은 락을 획득할 때까지 해당 스레드가 빙빙 돌고 있다는 것을 의미한다.스핀락은 바쁜 대기의 한 종류이다.
    //https://velog.io/@yarogono/C-Lock%EC%9D%84-%EC%95%8C%EC%95%84%EB%B3%B4%EC%9E%90
    //Contaxt Switch 
    //‘CPU 자원이 필요한데 얻지 못하는 스레드가 발생하는 상황을 방지하는 것
    //https://rito15.github.io/posts/04-cs-context-switching/
    // Auto ResetEvent
    /// <summary>
    /// ex) 고속도로 톨게이트: 문이 열리고 닫히고를 한동작으로 보는 개념
//    Manual Reset Event : 수동문: 수동으로 문을 잠금
//    문이 자동으로 안잠기니깐 수동으로 문을 잠궈야 함
//안그러면 여러 쓰레드들이 점유 되는 상황이 발생함
    /// </summary>
    class Lock
    {
        // 문을 연 상태로 시작 할지 닫은 상태로 시작 할지 bool <- 커널
        //AutoResetEvent _avilabe = new AutoResetEvent(true); // 문이 자동으로 닫힘 
        // 운영체제의 커널단에 다가 요청 하는 클래스 // true : 아무나 다 들어오는 형태
        // false 누구도 들어올 수 없는 문이 닫힌 상태
        // 한번에 한번씩 입장이 될때 쓰이고
        // 오토 리셋 이벤트랑 Mutex
        // 둘다 비슷한 기능을 갖고 이싿. 운영체제 커널단에서 해결된다는 점은 비슷
        // 하지만  Mutex : 몇번 들어왔는지 카운팅 된다.
        ManualResetEvent _availabe = new ManualResetEvent(true);
        // 한번에 두개 이상 들어와서 일을 해야 할때
        // 로딩을 한다든지 네트워크에서 패킷을 받는 작업 어마어마하게 오래 걸리는 작업을 기다려싸다가
        // 작업이 끝났으면 모든 스레드들을 다시 재가동하는 그런 코드를 만들때 사용한다.
       
        public void Acquire2()
        {
            _availabe.WaitOne();
            //_avilabe.WaitOne(); // 입장을 시도 AutoResetEvent
            _availabe.Set(); // bool = fasle 로 문이 닫힌다.
        }
        public void Release2()
        {
            //_avilabe.Set()// 문이 열림AutoResetEvent
        }
        // 이러한 방식으로   ManualResetEvent 보단  AutoResetEvent가 편하다.

        //bool _lock = fasle;
        volatile int _lock = 0;
            public void Acquire()
            {
                //while (_lock)
                // {
                //     // 잠김이 풀리기까지 기다린다.
                // }
                // _lock = true;
                while (true)
                {
                    #region 일반적인 범용적인 버전
                    //int original = Interlocked.Exchange(ref _lock, 1); // 1값 대입
                    //if (original == 0) //  0이 되면 풀림
                    //break;// 이렇게 출력하면 0이 나옴
                    #endregion
                    // int original = _lock;
                    // _lock = 1;
                    // 이런방법도 있지만 위에 이미 세팅이되어있어 굳이 할 필요가없다.
                    int expected = 0;
                    int desired = 1;
                    if (Interlocked.CompareExchange(ref _lock, desired, expected) == expected)
                    {
                        break;
                    }
                    Thread.Sleep(1);// 무조건 휴식
                    Thread.Sleep(0);//조건부 양보 => 나보다 우선 순위가 낮은 애들한테는 양보 불가
                                    // => 우선순위가 나보다 같거나 높은 레드가 없으면 다시 본인한테 
                    Thread.Yield(); // 관대한 양보
                                    // 무작정 기다리는거싱 아니라
                                    // 일단 다시 자기자리로 들어갔다가 잠시후 들어오는것
                }
                {
                    if (_lock == 0)
                    {
                        _lock = 1;
                    }
                }

            }
            public void Reslesse()
            {
                //_lock = false;
                _lock = 0;
            }
        }
        internal class Program
        {
            static int num = 0;
            static Lock _lock = new Lock();
            static void Thread_A()
            {
                for (int i = 0; i < 100000; i++)
                {
                //_lock.Acquire();
                //num++;
                //_lock.Reslesse();
                //_lock.Acquire2();
                //num++;
                //_lock.Release2();
                
            }
            }
            static void Thread_B()
            {
                for (int i = 0; i < 100000; i++)
                {
                //_lock.Acquire();
                //num--;
                //_lock.Reslesse();
                _lock.Acquire2();
                num--;
                _lock.Release2();
            }
            }
            static void Main(string[] args)
            {
                Task t1 = new Task(Thread_A);
                Task task2 = new Task(Thread_B);
                t1.Start();
                task2.Start();
                Task.WaitAll(t1, task2);
                Console.WriteLine(num);
            }
        }
    
}
