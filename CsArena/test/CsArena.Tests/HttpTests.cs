using System.Net;

namespace CsArena.Tests;

public class HttpTests
{
    [Fact (Skip = "Integration test reaching https://google.com"), Trait("type", "https")]
    public async Task HttpClientTest()
    {
        for (var i = 0; i < 10 /*100_000*/; ++i)
        {
            using var client = new HttpClient();
            var res = await client.GetAsync("https://google.com");

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }
    }
    
    private static int TotalPageSizes;
    private static readonly HttpClient HttpClientt = new() { MaxResponseContentBufferSize = 1_000_000 };
    private static readonly IEnumerable<string> Urls =
    [
        "https://learn.microsoft.com",
        "https://learn.microsoft.com/aspnet/core",
        "https://learn.microsoft.com/dotnet",
        "https://learn.microsoft.com/visualstudio"
    ];

    [Fact (Skip = "Integration test reaching https://learn.microsoft.com"), Trait("type", "http")]
    public async Task HttpClient_WhenAny()
    {
        TotalPageSizes = await SumPageSizesAsync();
        Assert.True(TotalPageSizes > 0);

        async Task<int> SumPageSizesAsync()
        {
            IEnumerable<Task<int>> downloadTasksQuery =
                from url in Urls
                select ProcessUrlAsync(url, HttpClientt);

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

    [Fact (Skip = "Integration test reaching https://learn.microsoft.com"), Trait("type", "http")]
    public async Task HttpClient_CancelAfter()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () => _ = await SumPageSizesAsync());

        async Task<int> SumPageSizesAsync()
        {
            IEnumerable<Task<int>> downloadTasksQuery =
                from url in Urls
                select ProcessUrlAsync(url, HttpClientt, cts.Token);

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

    //[Fact, Priority(20)]
    //public async Task ContinueWith()
    //{
    //    var sumPageSizes = await SumPageSizesAsync();

    //    // TotalPageSizes is already calculated during previous run of WhenAnyTest()
    //    Assert.Equal(TotalPageSizes, sumPageSizes);

    //    async Task<int> SumPageSizesAsync()
    //    {
    //        int total = 0;
    //        var continueTasks = new List<Task>();

    //        foreach (var url in Urls)
    //        {
    //            var task = Task.Run(() => ProcessUrlAsync(url, HttpClientt));

    //            var continueTask = task.ContinueWith(completed =>
    //            {
    //                switch (completed.Status)
    //                {
    //                    case TaskStatus.RanToCompletion:
    //                        total += completed.Result;
    //                        break;
    //                }
    //            });

    //            continueTasks.Add(continueTask);
    //        }

    //        await Task.WhenAll(continueTasks.ToArray());
    //        return total;
    //    }

    //    static async Task<int> ProcessUrlAsync(string url, HttpClient client)
    //    {
    //        byte[] content = await client.GetByteArrayAsync(url);
    //        return content.Length;
    //    }
    //}
}