using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTaskTester {
    public class Program {
        static void Main() {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}][{Thread.CurrentThread.ManagedThreadId}] STEP - 1");

            DoSomething();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}][{Thread.CurrentThread.ManagedThreadId}] STEP - 4");
            Console.ReadKey();
        }

        private static void DoSomething() {
            Thread.Sleep(1000);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}][{Thread.CurrentThread.ManagedThreadId}] STEP - 2");
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}][{Thread.CurrentThread.ManagedThreadId}] STEP - 3");
        }
    }
}
