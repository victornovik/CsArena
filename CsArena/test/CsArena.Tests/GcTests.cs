using System.Text.Json;
using CsArena.Tests.models;

namespace CsArena.Tests;

public class GcTests
{
    internal class JsonProcessor : IDisposable
    {
        public JsonProcessor(PlayerProcessor processor)
        {
            _processor = processor;
            _processor.PlayerCreated += PlayerCreated;
        }

        private void PlayerCreated(object sender, Player p)
        {
            Assert.Equal(5, p.DaysSinceLastLogin!.Value);
        }

        public void Start(string json)
        {
            var players = JsonSerializer.Deserialize<Player[]>(json);
            if (players == null) return;

            foreach (var p in players)
            {
                _processor.Process(p);
            }
        }

        public void Dispose() => _processor.PlayerCreated -= PlayerCreated;

        private readonly PlayerProcessor _processor;
    }

    internal class PlayerProcessor
    {
        public delegate void PlayerCreatedDelegate(object sender, Player p);
        public event PlayerCreatedDelegate? PlayerCreated;

        public void Process(Player p)
        {
            PlayerCreated?.Invoke(this, p);
        }

        public int GetEventSubscribers() => PlayerCreated != null ? PlayerCreated.GetInvocationList().Length : 0;
    }

    [Fact]
    public void DisposeTest()
    {
        Player[] players =
        [
            new() { DateOfBirth = DateTime.Now, DaysSinceLastLogin = 5, IsNewbie = false, Name = "Victor" },
            new() { DateOfBirth = DateTime.Now, DaysSinceLastLogin = 5, IsNewbie = true, Name = "Ira" }
        ];

        var pp = new PlayerProcessor();
        using (var fp = new JsonProcessor(pp))
        {
            Assert.Equal(1, pp.GetEventSubscribers());

            var json = JsonSerializer.Serialize(players, options: new JsonSerializerOptions { WriteIndented = true });
            fp.Start(json);
        }

        // fp.Dispose() is called
        Assert.Equal(0, pp.GetEventSubscribers());
    }

    [Fact]
    public void GenerationTest()
    {
        var p = new Player { DateOfBirth = DateTime.Now, DaysSinceLastLogin = 5, IsNewbie = false, Name = "Victor" };

        Assert.Equal(0, GC.GetGeneration(p));

        GC.Collect();
        GC.WaitForPendingFinalizers();
        Assert.Equal(1, GC.GetGeneration(p));

        GC.Collect();
        GC.WaitForPendingFinalizers();
        Assert.Equal(GC.MaxGeneration, GC.GetGeneration(p));

        GC.Collect();
        GC.WaitForPendingFinalizers();
        Assert.Equal(GC.MaxGeneration, GC.GetGeneration(p));
    }

    public struct Disposable : IDisposable
    {
        public bool isDisposed;
        public void Dispose() => isDisposed = true;
    }

    [Fact]
    public void DisposePuzzlerTest()
    {
        var disp1 = new Disposable();
        
        Assert.False(disp1.isDisposed);
        disp1.Dispose();
        Assert.True(disp1.isDisposed);

        var disp2 = new Disposable();
        Assert.False(disp2.isDisposed);
        // disp2 is copied and boxed. Dispose() is called on another reference-type instance.
        ((IDisposable)disp2).Dispose();
        Assert.False(disp2.isDisposed);

        var disp3 = new Disposable();
        using (disp3)
        {
            Assert.False(disp3.isDisposed);
            // disp3 is copied to an invisible value-type instance by using() but it is not boxed.
            // Dispose() is called on another invisible instance.
        }
        Assert.False(disp3.isDisposed);
    }
}