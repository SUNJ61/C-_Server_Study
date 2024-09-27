//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace _09_24Lock
//{
//    internal class DeadLock
//    {
//        private static readonly object lock1= new object();
//        private static readonly object lock2= new object();

//        static void Thread1Method()
//        {
//            lock (lock1)
//            {
//                Console.WriteLine("Thread 1 acqu");
//                Thread.Sleep(100); //  다른 쓰레드가 확득할 시간을 제공
//                lock (lock2)
//                {
//                    Console.WriteLine("Thread 1 acquired lock2");
//                }
//            }
//        }
//        static void Thread2Method()
//        {
//            lock (lock2)
//            {
//                Console.WriteLine("Threed 2 acqured lock2");
//                Thread.Sleep(100);
//                lock (lock1)
//                {
//                    Console.WriteLine("Threed 2 acqured lock2");
//                }
//            }
//        }

//        static void Main(string[] args)
//        {
//            Thread T1 = new Thread(new ThreadStart(Thread1Method));
//            Thread T2 = new Thread(new ThreadStart(Thread2Method));
//            T1.Start();
//            T2.Start();
//            T1.Join();
//            T2.Join();
//            Console.WriteLine("종료");
//        }
//    }
//}
