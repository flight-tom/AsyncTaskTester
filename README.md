# AsyncTaskTester
 驗證 async / await 關鍵字的實際作用。

身為老程序員，過往撰寫多執行緒的程式碼，或是所謂**非同步(異步)** 呼叫，直覺就是拿出最原始的 *[Thread](https://docs.microsoft.com/zh-tw/dotnet/api/system.threading.thread?view=net-6.0)* 類別解決。這樣過了幾年也沒有太在意微軟後來推出一系列 **TAP 工具類別包**，以及相關的 **關鍵字**。

然而前陣子剛好一場面試逼得我不得不跟上時代。問到我 *[async 和 await](https://docs.microsoft.com/zh-tw/dotnet/csharp/programming-guide/concepts/async/)* 造成的執行與順序差異，我啞口無言，直呼不熟。

上網 google 了數篇文章，約略有個概念後，開啟此專案實際演練，發現有許多意外之處。

```CSharp
public class Program {
    static DateTime begin;
    static void Main() {
        begin = DateTime.Now;
        Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 1");

        DoSomething();

        Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 4");
        Console.ReadKey();
    }

    private static void DoSomething() {
        Thread.Sleep(1000);
        Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 2");
        Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 3");
    }
}
```
開始我們先以此同步執行的簡單流程執行看看。*DoSomething* 是之後會改成非同步方法的測試標的，所以裡面先埋了 *Thread.Sleep(1000)* 延遲一秒，達到較長執行時間，方便之後看出平行執行的差異，同時 *ThreadID* 方便我們追蹤不同執行緒的差異。

這個同步執行的版本，我就放在 **Sync** 這個 branch 分支，他的執行結果也很直觀，如下：
![image](https://user-images.githubusercontent.com/3304716/169710279-ad3be408-a278-4255-9c78-f0e34b2fc9d8.png)

*ThreadID* 一致，大家都在同一列火車上前進。*車廂 2* 前面卡頓了一秒鐘，二三四節車廂都同樣延遲。
