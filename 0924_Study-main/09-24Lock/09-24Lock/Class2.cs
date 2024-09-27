//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace _2024_09_24_locked
//{
//    class SessionManager
//    {
//        static object _lock = new object();
//        public static void TestSession()
//        {
//            lock (_lock)
//            {

//            }

//        }
//        public static void Test()
//        {
//            lock (_lock)
//            {
//                UserManager.TestUser();
//            }
//        }
//    }
//    class UserManager
//    {
//        static object _lock = new object() { };
//        public static void Test()
//        {
//            //Monitor.TryEnter();
//            //제한된 시간안에  더 해보고 내가 락을 획득하는 데 실패 했으면 포기 하겠다.
//            //데드락이 일어났을 때 해결 방안을 찾는 것 보다 미리 락구조를 예방 하는 쪽으로
//            // 설계를 미리 하는 방법이 매우 좋은 방법이다.

//            lock (_lock)
//            {
//                SessionManager.TestSession();
//            }
//        }
//        public static void TestUser()
//        {
//            lock (_lock)
//            {

//            }
//        }
//    }

//    internal class MainApp2
//    {
//        static void ThreaA()
//        {
//            for (int i = 0; i < 100000; i++)
//            {
//                SessionManager.Test();
//            }
//        }
//        static void ThreaB()
//        {
//            for (int i = 0; i < 100000; i++)
//            {
//                UserManager.Test();
//            }
//        }
//        static void Main(string[] args)
//        {
//            Task t1 = new Task(ThreaA);
//            Task t2 = new Task(ThreaB);
//            t1.Start();
//            Thread.Sleep(100);
//            t2.Start();
//            Task.WaitAll(t1, t2);
//            //무책임 한 코드는 크러시가 생길때 까지 데드락을 발생 시키는 코드이다.
//            // 미리 예방 하는 코드 설계해서 하는 것이 매우 좋다
//        }
//    }
//}
