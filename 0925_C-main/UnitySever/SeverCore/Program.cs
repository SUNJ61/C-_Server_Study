//namespace SeverCore
//{
//    internal class Program
//    {
//        //1. 근성 2.양보 3. 갑질
//       // 위의 세가지 방법을 혼합해서 쓰는 것이 일반적이다.

//        static object _lock = new object();
//        static SpinLock _lock2 = new SpinLock();
//        //static Mutex _lock3 = new Mutex();
//        static ReaderWriterLockSlim _lock3 = new ReaderWriterLockSlim();
//        class Reward { }


//        static Reward GetRwardById(int id)
//        {
//            _lock3.EnterReadLock();
//            _lock3.ExitReadLock();
//            lock (_lock)
//            {
                
//            }
//            return null;

//        }
//        static void AddReward(Reward reward)
//        {
//            _lock3.EnterWriteLock();
//            _lock3.ExitWireLock();
//        }
//        static void Main(string[] args)
//        {
//            // 클라이언트까지 싱글쓰레드로 끝나지만 
//            // 서버에 들어가면 멀티쓰레드로 넘어와 복잡해진다.
//            lock (_lock)
//            {

//            }
//            bool lockTaken = false;
//            try
//            {
//                _lock2.Enter(ref lockTaken);
//            }
//            catch
//            {
//                if(lockTaken) 
//                _lock2.Exit(lockTaken);
//            }
    
//        }
//    }
//}
