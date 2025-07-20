using CsArena.Tests.ext;
using System.Collections.Concurrent;
using System.Diagnostics;
using Xunit.Priority;

namespace CsArena.Tests;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class AsyncTests
{
    [Fact]
    public async Task AwaitOnInt()
    {
        var square = await 5;
        Assert.Equal(25, square);
    }

    [Fact]
    public async Task ContinueAfterAwaitInTheSameThread()
    {
        var curThread = Thread.CurrentThread.ManagedThreadId;

        var task = Task.Delay(50);
        Thread.Sleep(100);

        // When we reach `await` the task is already completed so we keep executing in the same thread synchronously
        await task;

        Assert.Equal(curThread, Thread.CurrentThread.ManagedThreadId);
    }

    [Fact]
    public async Task ContinueAfterAwaitInDifferentThread()
    {
        var curThread = Thread.CurrentThread.ManagedThreadId;
        await Task.Delay(50);

        // Continuation will resume executing in some other thread
        Assert.NotEqual(curThread, Thread.CurrentThread.ManagedThreadId);
    }

    [Fact]
    public async Task TaskCompletionSource_Future_Promise()
    {
        // Future-Promise model
        var data = await SearchForPrices();

        Assert.NotNull(data);
        Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7, 8, 9], data);

        Task<IEnumerable<int>> SearchForPrices()
        {
            // Create Promise
            var tcs = new TaskCompletionSource<IEnumerable<int>>();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                var prices = new List<int>();
                for (var i = 0; i < 10; ++i)
                {
                    prices.Add(i);
                }

                // Move the Promise to RanToCompletion state
                tcs.SetResult(prices);
            });

            // Return Future bound to the Promise
            return tcs.Task;
        }
    }

    [Fact]
    public async Task FactoryStartNew()
    {
        var task = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(100);
                return "Victor";
            },
            CancellationToken.None,
            TaskCreationOptions.DenyChildAttach,
            TaskScheduler.Default
        );

        var res = await await task;

        Assert.Equal("Victor", res);
    }

    [Fact]
    public async Task AttachedToParentTasks()
    {
        var mutex = new object();
        string res = "";

        // Outer task will not complete until inner attached tasks complete, i.e. Task3 in this case
        await Task.Factory.StartNew(() =>
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(200);
                lock (mutex)
                    // ReSharper disable once AccessToModifiedClosure
                    res += "-Task1Done";
            });

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(300);
                lock (mutex)
                    res += "-Task2Done";
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                lock (mutex)
                    // ReSharper disable once AccessToModifiedClosure
                    res += "Task3Done";
            }, TaskCreationOptions.AttachedToParent);
        });

        lock (mutex)
            res += "-OuterTaskDone";

        await Task.Delay(500);

        Assert.Equal("Task3Done-OuterTaskDone-Task1Done-Task2Done", res);
    }

    [Fact]
    public async Task ExceptionCallstack()
    {
        var e1 = await Assert.ThrowsAsync<ArgumentException>(AsyncFacade.Start);

        Assert.Equal("From Task.Run()", e1.Message);
        Assert.NotNull(e1.StackTrace);
        Assert.Contains("Layer1()", e1.StackTrace);
        Assert.Contains("Layer2()", e1.StackTrace);
        Assert.Contains("Load()", e1.StackTrace);

        var e2 = await Assert.ThrowsAsync<ArgumentException>(SyncFacade.Start);

        Assert.Equal("From Task.Run()", e2.Message);
        Assert.NotNull(e2.StackTrace);
        Assert.DoesNotContain("Layer1()", e2.StackTrace);
        Assert.DoesNotContain("Layer2()", e2.StackTrace);
        Assert.DoesNotContain("Load()", e2.StackTrace);
    }


    public class AsyncFacade
    {
        public static async Task<string> Start()
        {
            return await new AsyncFacade().Layer1();
        }

        public async Task<string> Layer1()
        {
            var res = await Layer2();
            return res;
        }

        public async Task<string> Layer2()
        {
            var res = await Load();
            return res;
        }

        public async Task<string> Load()
        {
            return await Task.Run(() =>
            {
                throw new ArgumentException("From Task.Run()");
#pragma warning disable CS0162 // Unreachable code detected
                return "I've loaded something";
#pragma warning restore CS0162 // Unreachable code detected
            });
        }
    }

    public class SyncFacade
    {
        public static Task<string> Start()
        {
            return new SyncFacade().Layer1();
        }

        public Task<string> Layer1()
        {
            var res = Layer2();
            return res;
        }

        public Task<string> Layer2()
        {
            var res = Load();
            return res;
        }

        public Task<string> Load()
        {
            return Task.Run(() =>
            {
                throw new ArgumentException("From Task.Run()");
#pragma warning disable CS0162 // Unreachable code detected
                return "I've loaded something";
#pragma warning restore CS0162 // Unreachable code detected
            });
        }
    }

    [Fact]
    public async Task TaskRunOnDifferentThread()
    {
        // NOT RECOMMENDED WAY. Use await instead of blocking Task.Wait().
        //
        //var mainThreadId = Thread.CurrentThread.ManagedThreadId;
        //var t = Task.Run(() => 
        //{
        //    Assert.NotEqual(mainThreadId, Thread.CurrentThread.ManagedThreadId);
        //});
        //t.Wait();

        var mainThreadId = Thread.CurrentThread.ManagedThreadId;
        var task = Task.Factory.StartNew(() =>
            {
                Assert.NotEqual(mainThreadId, Thread.CurrentThread.ManagedThreadId);
            }
        );
        await task;
    }

    [Fact]
    public async Task ValueTask()
    {
        var rnd = new Random();
        int roll1 = await RollAsync();
        int roll2 = await RollAsync();
        int twoDices = roll1 + roll2;

        Assert.True(twoDices is > 1 and < 13);

        async ValueTask<int> RollAsync()
        {
            await Task.Delay(50);
            int diceRoll = rnd.Next(1, 7);
            return diceRoll;
        }
    }

    [Fact]
    public async Task WaitOnAsyncVoid()
    {
        var mutex = new object();
        string res = string.Empty;

        var tcs = new TaskCompletionSource<bool>();

        var button = new TestButton();
        button.Clicked += OnButtonClicked2Async;
        button.Clicked += OnButtonClicked1;
        button.Clicked += OnButtonClicked1;

        // Fire event
        button.Click();

        // Wait on future. Cannot await async void event handler since it does not return Task.
        await tcs.Task;

        Assert.Equal("-OnButtonClicked1-OnButtonClicked1-OnButtonClicked2Async", res);

        // Synchronous call
        void OnButtonClicked1(object? sender, EventArgs e)
        {
            Task.Delay(10).Wait();
            lock (mutex)
                res += "-OnButtonClicked1";
        }

        // Asynchronous call
        async void OnButtonClicked2Async(object? sender, EventArgs e)
        {
            await Task.Delay(50);
            lock (mutex)
                res += "-OnButtonClicked2Async";

            tcs.SetResult(true);
        }
    }

    public class TestButton
    {
        public event EventHandler? Clicked;
        public void Click()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }

    private static int _sumPageSizes;
    private static readonly HttpClient Http = new() { MaxResponseContentBufferSize = 1_000_000 };
    private static readonly IEnumerable<string> Urls =
    [
        "https://learn.microsoft.com",
        "https://learn.microsoft.com/aspnet/core",
        "https://learn.microsoft.com/dotnet",
        "https://learn.microsoft.com/visualstudio",
    ];

    [Fact, Priority(10)]
    public async Task WhenAny()
    {
        _sumPageSizes = await SumPageSizesAsync();
        Assert.True(_sumPageSizes > 0);

        async Task<int> SumPageSizesAsync()
        {
            IEnumerable<Task<int>> downloadTasksQuery =
                from url in Urls
                select ProcessUrlAsync(url, Http);

            // Call ToList() due to deferred nature of LINQ
            var downloadTasks = downloadTasksQuery.ToList();

            int total = 0;
            while (downloadTasks.Any())
            {
                Task<int> finishedTask = await Task.WhenAny(downloadTasks);
                downloadTasks.Remove(finishedTask);
                total += await finishedTask;
            }
            return total;
        }

        static async Task<int> ProcessUrlAsync(string url, HttpClient client)
        {
            byte[] content = await client.GetByteArrayAsync(url);
            return content.Length;
        }
    }

    [Fact]
    public async Task CancellationToken_CancelAfter()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () => _ = await SumPageSizesAsync());

        async Task<int> SumPageSizesAsync()
        {
            IEnumerable<Task<int>> downloadTasksQuery =
                from url in Urls
                select ProcessUrlAsync(url, Http, cts.Token);

            // Call ToList() due to deferred nature of LINQ
            var downloadTasks = downloadTasksQuery.ToList();

            int total = 0;
            while (downloadTasks.Any())
            {
                Task<int> finishedTask = await Task.WhenAny(downloadTasks);
                downloadTasks.Remove(finishedTask);
                total += await finishedTask;
            }
            return total;
        }

        static async Task<int> ProcessUrlAsync(string url, HttpClient client, CancellationToken token)
        {
            byte[] content = await client.GetByteArrayAsync(url, token);
            return content.Length;
        }
    }

    [Fact, Priority(20)]
    public async Task ContinueWith()
    {
        var sumPageSizes = await SumPageSizesAsync();

        // _sumPageSizes is already calculated during previous run of WhenAnyTest()
        Assert.Equal(_sumPageSizes, sumPageSizes);

        async Task<int> SumPageSizesAsync()
        {
            int total = 0;
            var continueTasks = new List<Task>();

            foreach (var url in Urls)
            {
                var task = Task.Run(() => ProcessUrlAsync(url, Http));

                var continueTask = task.ContinueWith(completed =>
                {
                    switch (completed.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            total += completed.Result;
                            break;
                    }
                });

                continueTasks.Add(continueTask);
            }

            await Task.WhenAll(continueTasks.ToArray());
            return total;
        }

        static async Task<int> ProcessUrlAsync(string url, HttpClient client)
        {
            byte[] content = await client.GetByteArrayAsync(url);
            return content.Length;
        }
    }

    [Theory]
    [InlineData(64)]
    [InlineData(128)]
    public void ConcurrentQueue(int size)
    {
        var queue = new ConcurrentQueue<int>();

        for (var i = 0; i < size; i++)
            queue.Enqueue(i);

        Assert.Equal(size, queue.Count);

        var actualSum = 0;
        var expectedSum = Enumerable.Range(0, size).Sum();
        var action = () =>
        {
            while (queue.TryDequeue(out var res))
                Interlocked.Add(ref actualSum, res);
        };

        Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            action, action, action, action);

        Assert.Equal(expectedSum, actualSum);
        Assert.Empty(queue);
        Assert.False(queue.TryPeek(out _));
    }

    [Theory]
    [InlineData(64)]
    [InlineData(128)]
    public void ConcurrentStack(int size)
    {
        var stack = new ConcurrentStack<int>();

        var arr = new int[size];
        for (var i = 0; i < size; i++)
            arr[i] = i;
        stack.PushRange(arr);

        Assert.Equal(size, stack.Count);

        var actualSum = 0;
        var expectedSum = Enumerable.Range(0, size).Sum();
        var action = () =>
        {
            while (stack.TryPop(out var res))
                Interlocked.Add(ref actualSum, res);
        };

        Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            action, action, action, action);

        Assert.Equal(expectedSum, actualSum);
        Assert.Empty(stack);
        Assert.False(stack.TryPeek(out _));
    }

    [Theory]
    [InlineData(64)]
    [InlineData(128)]
    public void ConcurrentDictionary(int size)
    {
        var dic = new ConcurrentDictionary<int, int>();

        Parallel.For(0, size, _ =>
        {
            // Initial call will add dic[1] = 1.
            // Ensuing calls will update dic[1] = dic[1] + 1
            dic.AddOrUpdate(1, 1, (_, oldValue) => oldValue + 1);
        });
        Assert.Equal(size, dic[1]);

        // Should return -10, as key 2 is not yet in the dictionary
        Assert.Equal(-10, dic.GetOrAdd(2, (_) => -10));
        // Should return -10, as key 2 is already set to that value
        Assert.Equal(-10, dic.GetOrAdd(2, -20));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public async Task ConcurrentBag(int size)
    {
        var bag = new ConcurrentBag<int>();

        var producers = new List<Task>();
        for (var i = 0; i < size; i++)
        {
            var j = i;
            producers.Add(Task.Run(() => bag.Add(j)));
        }
        await Task.WhenAll(producers);

        int actualElementsInBag = 0;

        var consumers = new List<Task>();
        while (!bag.IsEmpty)
        {
            consumers.Add(Task.Run(() =>
            {
                if (bag.TryTake(out _))
                    Interlocked.Increment(ref actualElementsInBag);
            }));
        }
        await Task.WhenAll(consumers);

        Assert.Equal(size, actualElementsInBag);
        Assert.Empty(bag);
        Assert.False(bag.TryPeek(out _));
    }

    [Fact]
    public void BlockingCollection()
    {
        const int size = 1000;

        using var unboundedQueue = new BlockingCollection<int>(new ConcurrentQueue<int>());

        for (int i = 0; i < size; i++)
            unboundedQueue.Add(i);

        // After a collection has been marked as complete for adding, adding to the collection is not permitted.
        // So attempts to take from the collection will not wait when the collection is empty.
        unboundedQueue.CompleteAdding();

        var actualSum = 0;
        var expectedSum = Enumerable.Range(0, size).Sum();
        var action = () =>
        {
            while (unboundedQueue.TryTake(out var res))
                Interlocked.Add(ref actualSum, res);
        };

        // Launch three parallel actions to consume the BlockingCollection
        Parallel.Invoke(action, action, action);

        Assert.Equal(expectedSum, actualSum);
        Assert.True(unboundedQueue.IsCompleted);
    }

    [Fact]
    public async Task BlockingBoundedCollection()
    {
        const int upperLimit = 10;
        const int elements = 20;

        var counter = 0;
        var boundedQueue = new BlockingCollection<int>(boundedCapacity: upperLimit);

        var consumer = Task.Run(() =>
        {
            while (!boundedQueue.IsCompleted)
            {
                boundedQueue.Take();
                counter++;
                Thread.SpinWait(100000);
            }
        });

        var producer = Task.Run(() =>
        {
            for (var i = 0; i < elements; i++)
            {
                boundedQueue.Add(i);
                Assert.False(boundedQueue.IsCompleted);
            }
            boundedQueue.CompleteAdding();
        });

        await Task.WhenAll(producer, consumer);

        Assert.Empty(boundedQueue);
        Assert.Equal(elements, counter);
    }

    [Fact]
    public void ThreadLocal()
    {
        ThreadLocal<decimal> threadLocal = new(trackAllValues: true);
        //var options = new ParallelOptions { MaxDegreeOfParallelism = 8};

        Parallel.For(0, 100,
        i =>
            {
                threadLocal.Value += Compute(i);
            });

        // Sum of the arithmetic progression of 100 numbers from 0.5 to 99.5
        Assert.Equal(5000m, threadLocal.Values.Sum());

        decimal Compute(int value)
        {
            var ms = new Random().Next(10, 50);
            var endTime = DateTime.Now + TimeSpan.FromMilliseconds(ms);
            while (DateTime.Now < endTime)
            { }
            return value + 0.5m;
        }
    }

    [Fact]
    public async Task AsyncLocal()
    {
        const int initValue = -1;
        AsyncLocal<int> asyncLocal = new() { Value = initValue };
        ThreadLocal<int> threadLocal = new(trackAllValues: true) { Value = initValue };

        Assert.Equal(initValue, asyncLocal.Value);
        Assert.Equal(initValue, threadLocal.Value);

        await SetValue();

        Assert.Equal(initValue, asyncLocal.Value);

        if (!threadLocal.IsValueCreated)
            Assert.Equal(0, threadLocal.Value);

        async Task SetValue()
        {
            asyncLocal.Value = 1;
            threadLocal.Value = 1;
            var task1 = CheckExpected(1);

            asyncLocal.Value = 2;
            threadLocal.Value = 2;
            var task2 = CheckExpected(2);

            await task1;
            await task2;
        }

        async Task CheckExpected(int expected)
        {
            Assert.Equal(expected, asyncLocal.Value);
            Assert.Equal(expected, threadLocal.Value);

            await Task.Delay(100);

            Assert.Equal(expected, asyncLocal.Value);

            if (!threadLocal.IsValueCreated)
                Assert.Equal(0, threadLocal.Value);
        }
    }

    [Fact]
    public void InterlockedTest()
    {
        long destination = 0;

        for (var i = 0; i < 100; i++)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Interlocked.Increment(ref destination);
            });
        }

        // Compares `destination` with `comparand`:0 and if they are equal replaces `destination` with `value`:0
        // Returns the original value of `destination`
        while (Interlocked.CompareExchange(ref destination, value: 0, comparand: 0) != 100)
        { }

        while (Interlocked.Read(ref destination) != 100)
        { }

        Assert.Equal(100, destination);
    }

    [Fact]
    public void Lazy()
    {
        int Fib(int n) => n == 1 ? 1 : n == 2 ? 1 : Fib(n - 1) + Fib(n - 2);

        // Function is prepared, but not executed
        var lazyFib = new Lazy<int>(() => Fib(5));

        // Only here Fib() will be actually called
        Assert.Equal(5, lazyFib.Value);
    }

    private class LargeObject(int threadId, int threadCount)
    {
        public int CreatedByThread { get; } = threadId;
        public readonly int[] WrittenByThread = new int[threadCount];
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    public void LazyMultithreaded(int threadCount)
    {
        // LargeObject is not created until you access the Value property of the lazy
        var lazy = new Lazy<LargeObject>(() =>
        {
            var obj = new LargeObject(Thread.CurrentThread.ManagedThreadId, threadCount);
            // insert any initialization code
            return obj;
        });

        var threads = new Thread[threadCount];
        for (var i = 0; i < threads.Length; i++)
        {
            var threadIndex = i;
            threads[i] = new Thread(() =>
            {
                LargeObject large = lazy.Value;
                lock (large)
                {
                    large.WrittenByThread[threadIndex] = Thread.CurrentThread.ManagedThreadId;
                }

            });
            threads[i].Start();
        }

        foreach (var t in threads)
        {
            t.Join();
        }

        // LargeObject is created by only one of the threads
        Assert.Contains(lazy.Value.CreatedByThread, threads.Select(t => t.ManagedThreadId));

        foreach (var t in threads)
        {
            Assert.Contains(t.ManagedThreadId, lazy.Value.WrittenByThread);
        }
    }

    private static string? result;

    static async Task<string> SaySomething()
    {
        await Task.Delay(500);
        result = "Hello world";
        return "Something";
    }

    [Fact]
    public void AsyncPuzzler()
    {
        var text = SaySomething();
        Assert.Null(result);
        Assert.Equal("Something", text.Result);
    }

    [Fact]
    public void ReentrantLockTest()
    {
        var i = 0;
        var syncObject = new object();

        lock (syncObject)
        {
            i++;
            Write();
        }

        Assert.Equal(2, i);
        return;

        void Write()
        {
            lock (syncObject)
            {
                i++;
            }
        }
    }

    [Fact]
    public void MonitorWaitTest()
    {
        var sync = new object();
        var thread = new Thread(() =>
        {
            try
            {
                Work();
            }
            finally
            {
                lock (sync)
                {
                    Monitor.PulseAll(sync);
                }
            }
        });
        thread.Start();

        lock (sync)
        {
            // Releases the lock on an object and blocks the current thread until it reacquires the lock
            Monitor.Wait(sync);
        }
        return;

        void Work() => Thread.Sleep(1000);
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    public void InterProcessMutex(int threadCount, int iterationCount)
    {
        var sum = 0;
        var threads = new Thread[threadCount];

        for (var i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(() =>
            {
                var mx = new Mutex(initiallyOwned: false, "UniqueMutexName");
                for (var j = 0; j < iterationCount; j++)
                {
                    mx.WaitOne();
                    sum++;
                    mx.ReleaseMutex();
                }
            });
            threads[i].Start();
        }
        foreach (var t in threads)
            t.Join();

        Assert.Equal(threadCount * iterationCount, sum);
    }

    [Fact]
    public void MonotonicTimeVsTimeOfDayTime()
    {
        // Milliseconds that have elapsed since 1970-01-01T00:00:00.000Z (Unix epoch)
        var timeOfDayInMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Assert.True(timeOfDayInMillis > 1);

        // Ticks in the accurate timer that monotonically increases since the computer is started
        var monotonicTimeInTicks = 10000L * Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond * 100L;
        Assert.True(monotonicTimeInTicks > 1);
    }

    [Fact]
    public async Task StopWatchTest()
    {
        const int DelayInMillis = 1000;

        var sw = new Stopwatch();
        sw.Start();
        await Task.Delay(DelayInMillis);
        sw.Stop();

        var ms = sw.ElapsedMilliseconds;
        Assert.True(ms > DelayInMillis);

        //var sec = (decimal)sw.ElapsedTicks / Stopwatch.Frequency;
        //Assert.Equal(decimal.Round(sec, 3, MidpointRounding.ToNegativeInfinity) * 1000, ms);
    }
}