using System.Net;

namespace CsArena.Tests;

public class HttpTests
{
    [Fact (Skip = "Integration test reaching google.com"), Trait("type", "http")]
    public async Task HttpClientTest()
    {
        for (var i = 0; i < 10 /*100_000*/; ++i)
        {
            using var client = new HttpClient();
            var res = await client.GetAsync("https://google.com");

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }
    }
}