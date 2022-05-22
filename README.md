# AsyncTaskTester
 驗證 [async / await](https://docs.microsoft.com/zh-tw/dotnet/csharp/programming-guide/concepts/async/) 關鍵字的實際作用。

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
開始我們先以此同步執行的簡單流程執行看看。*DoSomething()* 是之後*會改成非同步*方法的測試標的，所以裡面先埋了 *Thread.Sleep(1000)* 延遲一秒，達到較長執行時間，方便之後看出平行執行的差異，同時 *ThreadID* 方便我們追蹤不同執行緒的差異。

這個同步執行的版本，我就放在 **[sync](https://github.com/flight-tom/AsyncTaskTester)** 這個 branch 分支，他的執行結果也很直觀，如下：

![image](https://user-images.githubusercontent.com/3304716/169710279-ad3be408-a278-4255-9c78-f0e34b2fc9d8.png)

*ThreadID* 一致，大家都在同一列火車上前進。*車廂 2* 前面卡頓了一秒鐘，二三四節車廂都同樣延遲。

接著我們在 *DoSomething()* 前面補上 *async* 關鍵字，再跑跑看。

(**此時編譯器會出現警告，但不影響編譯和執行：**
*警告	CS1998	這個非同步方法缺少 'await' 運算子，因此將以同步方式執行。請考慮使用 'await' 運算子等候未封鎖的應用程式開發介面呼叫，或使用 'await Task.Run(...)' 在背景執行緒上執行 CPU-bound 工作。*)

```CSharp
private static async void DoSomething() {
    Thread.Sleep(1000);
    Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 2");
    Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 3");
}
```

![image](https://user-images.githubusercontent.com/3304716/169710963-454f1185-4774-4d97-a0d4-2bd2a437c616.png)

可以看見執行結果如同前一結果，驗證網路文獻提到 *async* 並不會改變執行緒，這個關鍵字的作用只是用來輔助編譯器進行檢查和提醒，類似 *readonly* 類型的輔助宣告。

好，加入真正的非同步寫法吧。

```CSharp
private static void DoSomething() {
    Task.Run(() => {
        Thread.Sleep(1000);
        Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 2");
        Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 3");
    });
}
```

我導入了 *[Task.Run()](https://docs.microsoft.com/zh-tw/dotnet/api/system.threading.tasks.task.run?view=net-6.0)* 開啟新的執行緒進行耗費時間(1秒)的工作，請注意，*DoSomething()* 這次並沒有添加 *async* 關鍵字，編譯器缺乏關鍵字提示後，並不會有任何警告，順利編譯，順利執行。

![image](https://user-images.githubusercontent.com/3304716/169711437-50c42de1-7d22-4b5f-b4a7-c375557b270b.png)

如我們預料的，上圖呈現一號火車帶著一四車廂直奔前行，二三車廂被丟包在四號列車，一秒後才發車。如此 *射後不理* 的非同步呼叫的目的已經達成，沒有使用 *async 和 await* 語法。

這個版本我放在 [task_0](https://github.com/flight-tom/AsyncTaskTester/tree/task_0) 這個分支。

等等，我們把第三節車廂搬到 Task.Run() 之外的地方跑跑看吧！

```CSharp
private static void DoSomething() {
    Task.Run(() => {
        Thread.Sleep(1000);
        Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 2");
    });
    Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 3");
}
```

![image](https://user-images.githubusercontent.com/3304716/169711941-990bd32b-70c5-4384-b6f7-6e2c315a40d6.png)

很好，結果也在預料中。一號列車帶著一三四車廂飛奔，只有二號車廂轉交給第四列車。也就是說： 只有放在 *Task.Run()* 內的程式碼會被非同步執行。這個案例我們放在 [task_1](https://github.com/flight-tom/AsyncTaskTester/tree/task_1) 分支。

## async & await 上場！

在 *DoSomething()* 前面宣告 *async*，然後 *Task.Run()* 前面加上 *await*，修改如下：

```CSharp
private static async void DoSomething() {
    await Task.Run(() => {
        Thread.Sleep(1000);
        Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 2");
    });
    Console.WriteLine($"[{(DateTime.Now - begin).TotalMilliseconds:0000}][{Thread.CurrentThread.ManagedThreadId}] STEP - 3");
}
```

執行：
![image](https://user-images.githubusercontent.com/3304716/169712674-5381c08b-395d-42d1-86e8-a481b5a7654c.png)

如上圖，非同步如預料般....，等一下！結果跟前一版本不同，原本一號列車會帶著一三四車廂一起飛的景象，竟然會因為 *await* 出現的關係，讓*Task.Run()之外*的三號車廂停下來跟著二號車廂走。

這代表了，*await* 宣告會讓整個 *async* 宣告的 function 變成**同步作業**，不管有沒有放在 *Task.Run()* 之內根本沒差。這個版本，我放在 [async_await](https://github.com/flight-tom/AsyncTaskTester/tree/async_await) 分支。

##那也就是說，我其實可以....



