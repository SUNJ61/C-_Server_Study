using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _09_23Csharp
{
    internal class SeverBase01
    {
        static int number = 0;

        #region Interlocked
        static void Thread_1()
        {
  
            for (int i = 0; i< 100000; i++) //100~10000 까지는 0이 잘 출력되지만 100000으로 하면 0으로 출력되다가 한계로 0이 아닌걸 출력할때가 있다.
            {
                number += 1; 
                //Interlocked.Increment(ref number);  
                // Increment는 원자성이 보장된 number에 +1 하는 함수
                // number에 값을 인자로 넣는게 아니고 참조형으로 넘긴다.
                // += 어셈블리는 이것을 이해 못한 +=1
            }
      
        }
       
        static void Thread_2()
        {

            for (int i = 0; i<100000; i++)
            {
                number -= 1;
                //Interlocked.Decrement(ref number);
                // Decrenment는 원자성 보장된 number에 -1하는 함수로
                
            }
        }
        #endregion
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();
            Task.WaitAll(t1,t2);
            Console.WriteLine(number);

        }
    }
}
