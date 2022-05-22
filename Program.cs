using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTaskTester {
    public class Program {
        static DateTime begin;
        static void Main() {
            begin = DateTime.Now;
            Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 1");

            DoSomething();

            Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 4");
            Console.ReadKey();
        }

        private static async void DoSomething() {
            await Task.Run(() => { });  // 0_0... readability??

            Thread.Sleep(1000);
            Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 2");
            Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 3");
        }
    }
}
