using CollectionArena.models;

namespace CsArena.Tests.models;

public class Player : INameable
{
    public string? Name { get; set; }
    public int? DaysSinceLastLogin { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool? IsNewbie { get; set; }

    [Obsolete("Please use GetFullTypeName()")]
    public string GetTypeName<T>()
    {
        return typeof(T).Name;
    }

    public string? GetFullTypeName<T>()
    {
        return typeof(T).FullName;
    }
}

public class PlayerList(IEnumerable<Player> players)
{
    // Indexer
    public Player this[int i] => _players[i];
    public Player this[string name] => _players.First(p => p.Name == name);

    private readonly Player[] _players = players.ToArray();
}