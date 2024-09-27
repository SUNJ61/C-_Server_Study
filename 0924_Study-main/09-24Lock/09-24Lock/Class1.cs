using System;
using System.Threading.Tasks;

namespace _09_24Lock
{
    //Context Switching 조건
    // 이 데드락은 서로 자원을 점령 하려 했지만 추가로 자원을 모으는 상황이 되어 발생하는 일이다.
    class SessionManager
    {
        static object _lock = new object();
        private static void TestSession()
        {
            lock (_lock)
            {
                // 세션 관련 작업 수행
            }
        }
        public static void Test()
        {
            lock (_lock)
            {
                TestSession();
            }
        }

        


        class UserManager
        {
            public static void Test()
            {
                //Monitor.TryEnter();
                //// 몇초동안 더 해보고 이함수가 락을 가지는데 실패 했다면 포기하는 함수
                //// 데드락이 일어났을 때 해결 방안을 찾는것보다 미 락구조를 예방 하는 쪽으로 
                // 설계를 미리 하는 방법이 매우 좋은 방법이다.
                //try
                //{

                //}
                //catch
                //{

                //} 예시 try catch 문
                lock (_lock)
                {
                    SessionManager.TestSession();
                }
            }
            public static void TestUser()
            {
                lock (_lock)
                {

                }
            }
        }
        internal class Class1
        {
            
            static void ThreadA()
            {
                for (int i = 0; i < 100000; i++)
                {
                    SessionManager.Test();
                }
            }

            static void ThreadB()
            {
                for (int i = 0; i < 100000; i++)
                {
                    UserManager.Test();
                }
            }

            static void Main(string[] args)
            {
                Task t1 = new Task(ThreadA);
                Task t2 = new Task(ThreadB);
                t1.Start();
                Thread.Sleep(100);
                t2.Start();
                Task.WaitAll(t1, t2);
                // 무책임 한 코드는 크러시가 생길때 까지 데드락을 발생 시키는 코드이다.
                // 미리 예방하는 코드 설계
            }
        }
    }
}
