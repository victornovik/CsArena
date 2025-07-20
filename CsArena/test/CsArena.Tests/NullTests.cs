using CsArena.Tests.models;

namespace CsArena.Tests;

public class NullTests
{
    [Fact]
    public void WhenNull()
    {
        var player = new Player();

        Assert.Null(player.Name);
        Assert.True(string.IsNullOrWhiteSpace(player.Name));

        Assert.Null(player.DaysSinceLastLogin);
        Assert.False(player.DaysSinceLastLogin.HasValue);
        Assert.Equal(5, player.DaysSinceLastLogin.GetValueOrDefault(5));

        Assert.Null(player.DateOfBirth);
        Assert.False(player.DateOfBirth.HasValue);

        Assert.Null(player.IsNewbie);
        Assert.False(player.IsNewbie.GetValueOrDefault());
    }

    [Fact]
    public void WhenNotNull()
    {
        var player = new Player{ Name = " ", DaysSinceLastLogin = 3, DateOfBirth = DateTime.MinValue, IsNewbie = false };

        Assert.NotNull(player.Name);
        Assert.True(string.IsNullOrWhiteSpace(player.Name));

        Assert.NotNull(player.DaysSinceLastLogin);
        Assert.True(player.DaysSinceLastLogin.HasValue);
        Assert.Equal(3, player.DaysSinceLastLogin);

        Assert.NotNull(player.DateOfBirth);
        Assert.True(player.DateOfBirth.HasValue);
        Assert.Equal(DateTime.MinValue, player.DateOfBirth);

        Assert.NotNull(player.IsNewbie);
        Assert.True(player.IsNewbie.HasValue);
        Assert.False(player.IsNewbie);
    }

    [Fact]
    public void CompareNullable()
    {
        int? i = null, j = null;
        Assert.Equal(i, j);
    }

    [Fact]
    public void NullCoalescing()
    {
        var player = new Player();
        var days = player.DaysSinceLastLogin ?? -1;
        Assert.Equal(-1, days);

        int? x = null, y = null, z = 1;
        Assert.Equal(1, x ?? y ?? z);
    }

    [Fact]
    public void NullCoalescingAssignment()
    {
        Player? player = null;
        player ??= new Player();

        Assert.NotNull(player);
        Assert.Null(player.Name);
    }

    [Fact]
    public void NullConditional()
    {
        Player? player = null;
        var days = player?.DaysSinceLastLogin;
        Assert.Null(days);
    }

    [Fact]
    public void ArrayNullConditional()
    {
        Player?[] players = [new() { Name = "Victor" }, new(), null];

        Assert.Equal("Victor", players[0]?.Name);
        Assert.Null(players[1]?.Name);
        Assert.Null(players[2]?.Name);
    }

    [Fact]
    public void ArrayNullConditional2()
    {
        Player[]? players = null;
        Assert.Null(players?[0].Name);
    }

    [Fact]
    public void NullForgiving()
    {
        var player = new Player { Name = "Spartak" };
        Assert.Equal(7, player.Name!.Length);
    }

    [Fact]
    public void ArrayNullForgiving()
    {
        Player?[] players = [new() { Name = "Victor" }, null, null];
        Assert.NotNull(players[0]!.Name);
    }

    [Fact]
    public void NullableGenerics()
    {
        Player? nullPlayer = null;
        Player nonNullPlayer = new Player();

        Assert.True(LogNullable(nullPlayer));
        Assert.False(LogNullable(nonNullPlayer));
        Assert.True(LogNonNullable(nonNullPlayer));

        // COMPILE ERROR: Nullability of type argument 'CsArena.Tests.Player?' doesn't match 'class' constraint
        //LogNonNullable(nullPlayer);

        bool LogNullable<T>(T t) where T : class? => t is null;
        bool LogNonNullable<T>(T t) where T : class => t is not null;
    }

    [Fact]
    public void ThrowArgumentNullException()
    {
        void CheckForNull(int? obj) => ArgumentNullException.ThrowIfNull(obj);
        Assert.Throws<ArgumentNullException>(() => CheckForNull(null));
    }

    [Fact]
    public void IsNullableReferenceType()
    {
        // Nullable<T> is struct and it requires T as value type
        Assert.True(typeof(Nullable<int>).IsValueType);
        Assert.False(typeof(int?).IsClass);
        Assert.True(typeof(int?).IsValueType);

        Assert.False(typeof(DateTime?).IsClass);
        Assert.True(typeof(DateTime?).IsValueType);

        // typeof cannot be used on nullable reference types as `?` annotation in a declaration indicates that the variable might be null.
        // It doesn't indicate a different runtime type.
        string? s1 = "a";
        string s2 = "b";
        Assert.Equal(s1.GetType(), s2.GetType());
        Assert.True(s1.GetType().IsClass);
    }
}