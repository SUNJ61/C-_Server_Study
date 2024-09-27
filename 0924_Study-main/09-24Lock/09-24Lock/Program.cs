//namespace _09_24Lock
//{
//  Lock조건
//    internal class Program
//    {
//        static int number = 0;
//        static object _obj = new object();
//        static void Threed_A()
//        {
//            for (int i = 0; i< 10000; i++)
//            {
//                // 상호배제 : Mutal Exclusive
//                // C#에서는 CriticalSection c++ std::mutex
//                  
//                //Interlocked.Increment(ref number);
//                //Monitor.Enter(_obj); // 어떤 쓰레드가 와서 이 메서드를 점유 할 경우
//                //number++;   // 다른 쓰레드가 여기 오지 못하게 잠금
//                //Monitor.Exit(_obj); // 일을 끝내는 후 잠근 것을 해제
//                lock (_obj)
//                {
//                    number++;
//                }//이렇게 lock 사용하면 for문 조건에 도달하면 자동으로 해제하여  간단히 쓸 수 있음+ 데드락이 덜 걸림
//            }
//            // 즉 다른곳에서도 뭔가 쓰는 작업이 들어가기 시작하면
//            // 이렇게 갖고 오늘것도 문제가 된다.

//        }
//        static void Threed_B()
//        {
//            for (int i = 0; i < 10000; i++)
//            {
//                lock (_obj)
//                {
//                    number--;
//                }
//                ////Interlocked.Decrement(ref number);
//                //Monitor.Enter(_obj);
//                //number--;
//                // 만약 이렇게 짰을시 실행이 안된다.  아래 상황을 데드락이라고 부른다.
//                //Monitor.Enter(_obj);
//                //{
//                //    number++;

//                //    return;
//                //}
//                //Monitor.Exit(_obj);
//                // 왜 출력이 안되는가? =  return 떄문에 빠져나가서 Exit를 할 수 없어 출력이 불가능하다.
//                //Monitor.Exit(_obj);
//                //try
//                //{
//                //    Monitor.Enter(_obj);
//                //    number++;
//                //    return;
//                //}
//                //finally
//                //{
//                //    Monitor.Exit(_obj);
//                //}
//            }
//        }



//        static void Main(string[] args)
//        {
//            Task t1 = new Task(Threed_A);
//            Task t2 = new Task(Threed_B);
//            t1.Start();
//            t2.Start();
//            Task.WaitAll(t1,t2);
//            Console.WriteLine(number);
//        }
//    }
//}
