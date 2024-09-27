//using System;
//using System.Data;
//using System.Threading;
//using System.Threading.Tasks;
//namespace _09_23Csharp
//{
//    // 현재 코드를 짤때 Debug 모드로 했지만 나중에 상용화하면 Release 모드를 해야 한다.
//    // 애당초 캐시 메모리를 무시하고 최신값을 가져와라
//    // 임시방편: 전문가들이 자주 사용말고 임시방편으로 테스용으로 사용하며
//    // 나중에 Lock(락) Atomic 같은 옵션을 쓰라고 권장한다.
//    #region static 변수 쓰레드 동시 접근시 일어나는 문제
//    //static이 붙은 메서드나 변수들은 멀티 쓰레드들이 접근가능하다.
//    // 메모리는 다 각자 Stack 메모리를 할당 받아서 사용된다.
//    // 그런데 전역으로 선언된 변수들은 Thread들이 다 공통으로 같이 사용
//    // 동시에 접근이 가능해서 동시 접근시 어떤일이 일어나는 알아보자
//    #endregion
//    internal class Program
//    {

//        //static bool _stop = false; 


//        //static void TreadMain()
//        //{
//        //    // #region:휘발성 "쓰레드가 그냥 얼신도 하지말고 최적화 하지 말아라 라는 뜻이다.
//        //    //Console.WriteLine("쓰레드 시작");
//        //    //while (_stop == false)
//        //    //{
//        //    //    //누군가가 stop 신호를 해주기를 기다린다.
//        //    //}
//        //    //// 만약 코드 잘못 입력했을때 수정할 수 있다.
//        //    ////if( _stop==false)
//        //    ////{
//        //    ////    while (true)
//        //    ////    {

//        //    ////    }
//        //    ////}
//        //    //// 위에 코드처럼 최적화 해서 망치는 경우가 발생하면 
//        //    //// Relase 모드에서 최적화가 안되도록 강제로 하는 모드가 있다.
//        //    //Console.WriteLine("쓰레드 종료");
//        //}

//        static volatile int x = 0;
//        static volatile int y = 0;
//        static volatile int r1 = 0;
//        static volatile int r2 = 0;
//        //volatile 키워드는 컴파일러나 CPU가 이 변수를 최적화하지 않도록 함
//        //이는 멀티 쓰레드 환경에서 변수의 값을 항상 메인 메모리에서 읽고 쓰도록 보장
//        static void Thread_1()
//        {
//            y = 1;
//            Thread.MemoryBarrier();//하면 CPU와 컴파일러에게 특정 메모리 작업의 순서를 보장
//                                   //이 호출은 메모리 접근의 순서를 강제하므로, 쓰레드가 실행되는 동안 변수의 값을 일관되게 유지하는 데 도움
//            r1 = x;
//        }
//        static void Thread_2()
//        {
//            x = 1;
//            Thread.MemoryBarrier();
//            r2 = y;
//        }
//        // x 도 y도 1인 상태고 그 다음에 x가 r1에 들어가면 여기서 애당초 1이 되어야 된다.
//        // 하드웨어도 우리를 위해서 최적화 해주고있다.
//        static void Main(string[] args)
//        {

//            #region  컴퍼일러 최적화
//            //Task t = new Task(TreadMain);// Task 변수생성 Task안에 함수 등록
//            //t.Start();// Task 발현
//            //Thread.Sleep(1000); // 밀리초 단위라서 1초동안 대기하겠다.
//            //_stop = true; // 멈춤 
//            //Console.WriteLine("Stop 호출");
//            //Console.WriteLine("종료 대기");
//            //t.Wait(); // Task 대기 ,이런식으로 코루틴 WaitforSecound처럼 1초대기함
//            //Console.WriteLine("종료성공");
//            #endregion
//            #region Tempporal Locality,Spacilal Locality

//            //int[,] arr = new int[10000, 10000]; // 이차 배열 선언
//            //{
//            //    long now = DateTime.Now.Ticks;
//            //    for(int x = 0; x < 10000;x++) //  x선언후 10000보다 작을떄
//            //    {
//            //        for(int y = 0; y< 10000; y++) //똑같이
//            //        {
//            //            arr[y, x] = 1;
//            //        }
//            //    }
//            //    long end = DateTime.Now.Ticks;
//            //    Console.WriteLine($"(y,x) 순서 걸린 시간:{end-now}");
//            //    // 민천한 것대로 호출하면 빠랄진다. 여유 캐시메모리의 특성이다.
//            //}
//            //{
//            //    long now = DateTime.Now.Ticks;
//            //    for (int y = 0; y < 10000; y++) //똑같이
//            //    {
//            //        for(int x = 0; x<10000; x++)
//            //        {
//            //            arr[x, y] = 1;
//            //        }
//            //    }
//            //    long end = DateTime.Now.Ticks;
//            //    Console.WriteLine($"(y,x) 순서 걸린 시간:{end - now}");
//            //}// 순서 바꾸는것도 시간이 걸리므로 코드를 제작할때 메모리 생각하며 신중히 짜야한다.
//            #endregion
//            #region 메모리 베리어
//            //하는이유 : 코드 재배치
//            // 가시성
//               // ex) 1번 직원이 뭔가 주문받은거 자체를 
//               // 다른 직원들로 바로 볼 수 있고 알 수 있냐 라는 뜻
//            // FullMemory Barrier() : Store/Load 둘 다 막는다.
//            // StoreMemory Barrier: Stroe만 막는다.
//            // LoadMemory Barrier : Load만막는다.
//            int count = 0;
//            while (true)
//            {
//                count++;
//                x = y = r1 = r2 = 0; // true가 될때까지 계속 0상태 유지
//                Task t1 = new Task(Thread_1); // 함수 접근
//                Task t2 = new Task(Thread_2); 
//                t1.Start();
//                t2.Start();
//                Task.WaitAll(t1, t2);
//                if (r1 == 0 && r2 == 0)
//                    break;
//            }
//            Console.WriteLine($"{count}번만에 빠져나옴");
//            //이 코드는 멀티 쓰레드 환경에서의 데이터 일관성 문제를 보여주며, volatile 키워드가 변수에 대한 최적화를 방지하지만 여전히 데이터 레이스 조건을 피할 수 없음을 나타남.
//            //이를 통해 메모리 배리어와 동기화의 중요성을 이해
//            #endregion
//        }
//    }
//}
