namespace CsArena.Tests.models;

public record Country(string Name, string Code, string Region, int Population)
{
    public string? Name { get; set; } = Name;
    public string Code { get; } = Code;
    public string Region { get; } = Region;
    public int Population { get; } = Population;
}