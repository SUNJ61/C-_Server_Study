using System;
using System.Threading;
using System.Threading.Tasks;
namespace _0920_ServerCore
{
    internal class ServerCore
    {
        static void MainThread(object? state)
        {
            #region Thread Pool을 사용하지 않고 직접 쓰레드를 할당한 경우.
            //while (true) //t 쓰레드가 아래의 문구를 계속 반복중.
            //{
            //    Console.WriteLine("Hello, Thread");
            //}
            #endregion
            #region 쓰레드 풀을 사용하지 않고 1000개의 쓰레드를 직접 생성한 경우(비효율적이다)
            //for (int i = 0; i < 5; i++)
            //{
            //    Console.WriteLine("Hello, Thread");
            //}
            #endregion
            #region ThreadPool 사용과 Task와 ThreadPool 함께 사용하기
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Hello, Thread");
            }
            #endregion
        }
        static void Main(string[] args)
        {
            #region ThreadPool 사용과 Task와 ThreadPool 함께 사용하기 
            ThreadPool.SetMinThreads(1, 1); //첫번째 인자(매개변수)는 사용 쓰레드 개수, 두번째 인자는 네트워크 형태 결정? -> ThreadPool이 사용할 최소 쓰레드 개수
            ThreadPool.SetMaxThreads(5, 5); //ThreadPool이 사용할 최대 쓰레드 개수

            for (int i = 0; i < 5; i++) //Task에 쓰레드를 할당하여 5개의 쓰레드 모두 무한 반복을 하도록 할당한다.
            {
                Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning);//Task는 void생성자가 없다. 인수가 필요하다. t에 무한루프를 할당하고, Task의 런타임을 길어질 것을 명시함.
                                                                                               //TaskCreationOptions.LongRunning이 없으면 먹통이 된다. 왜냐하면 TaskCreationOptions.LongRunning 옵션으로 인하여
                                                                                               //threadPool에서 할당한 쓰레드 이외의 새로운 쓰레드에 해당 작업을 할당하여 실행하기 때문이다.
                                                                                               //즉, 해당 코드에서 TaskCreationOptions.LongRunning가 있으면 5개 + ThreadPool이 사용하는 쓰레드를 사용하게 되는 것이다.
                t.Start(); //t에 할당된 함수를 실행 (TaskCreationOptions.LongRunning가 있으면 쓰레드 새로 할당, 없으면 ThreadPool에서 할당한 쓰레드 사용)
            }

            //for (int i = 0; i < 4; i++) //n개의 쓰레드에게 무한 반복문을 할당하여 n개의 쓰레드는 계속 일을 하고 있다. (n은 조건문에서 결정됨)
            //    ThreadPool.QueueUserWorkItem((obj) => { while (true) { } }); //ThreadPool의 이해를 돕기위한 코드

            ThreadPool.QueueUserWorkItem(MainThread); //오브젝트 풀링과 비슷하다. 사용할 때만 메모리를 할당, 사용하지않으면 메모리를 회수해준다. (큐 방식이여서 먼저 회수한 쓰레드를 할당, 인력사무소와 비슷함.)
                                                      //위에서 할당한 5개중 1개의 쓰레드가 남아있어 해당 코드를 수행 가능하다. for문에 조건을 i<5 로 바꾸면 남는 쓰레드가 없어 실행이 되지 않는다.
                                                      //위의 설명은 ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });을 할당한 코드의 설명
                                                      //위의 단점을 극복할 수 있는 방법은 Task를 이용하는 방법이 있다. (단점. 남는 쓰레드가 없으면 다음 실행 기능이 멈춰버림.)
                                                      //ThreadPool과 Task는 같이 사용해야한다. ThreadPool은 짧은 작업을 처리하는데 효율적이지만 의도와 다르게 긴작업을 처리하면 문제가 생길 수 있다.
                                                      //즉, 의도와 다른 ThreadPool 사용으로 작업이 멈추는 것을 방지한다.
            #endregion
            #region Thread Pool을 사용하지 않고 직접 쓰레드를 할당한 경우.
            //Thread t = new Thread(MainThread); //Thread는 void 생성자가 존재하지 않는다. t 쓰레드는 MainTread를 할당 받았다. (쓰레드 생성)
            //t.Name = "Test Thread";
            //t.IsBackground = true; //Main이 활성화 되있는 동안만 쓰레드 활성화 인듯? -> 해당 코드로 인하여 Main이 출력된후 반복을 멈춰버린다.
            //t.Start(); //t.Start를 먼저 호출 했지만 MainThread()함수가 나중에 출력됨

            //Console.WriteLine("Waiting for Thread");

            //t.Join(); //실행이 끝날 때 까지 대기. -> IsBackground를 활성화 했지만 계속 t쓰레드가 반복됨. -> 이유는 해당 t 쓰레드의 실행이 모두 종료 될 때까지 기다리기 때문이다.
            //          //메인도 종료되지 않는다. (메인 함수는 메인쓰레드에서 관리하는데 Join이 반환되지 않기 때문에 메인쓰레드도 종료되지 않고 아래의 Console.WriteLine("Hello Main!");도 호출되지 않는다.)
            //          //위 설명은 While(true)를 사용했을 때 해당된다.

            //Console.WriteLine("Hello Main!"); //t.Start를 먼저 호출 했지만 이 문구가 먼저 출력 (t.IsBackground = true; 없이 t.Start();했을 경우)
            #endregion
            #region 쓰레드 풀을 사용하지 않고 1000개의 쓰레드를 직접 생성한 경우(비효율적이다)
            //for (int i = 0; i < 1000; i++)
            //{
            //    Thread t = new Thread(MainThread); //메인에서 계속 쓰레드를 할당하여 호출 중이다. 1000번의 쓰레드 할당, 1000개의 쓰레드를 생성하는 방식이다.
            //    t.IsBackground = true;
            //    t.Start();
            //}
            #endregion

            //while (true) //임시로 메인을 종료시키지 않기 위해 사용한 무한 반복 함수, 메인이 종료되지 않아야 ThreadPool.QueueUserWorkItem(MainThread)가 MainThread를 호출할 수 있다.
            //{
            //}
            int parse = int.Parse(Console.ReadLine()); //위에 반복문을 사용하지않고 계속 메인에서 대기할 수 있도록 하는 방법
        }
    }
}
