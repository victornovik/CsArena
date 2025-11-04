namespace CsArena.Tests.config;

public class Books
{
    public double Price { get; init; }
    public string? Category { get; init; }
    public string? Author { get; init; }
}

public class Login
{
    public string? UserName { get; init; }
    public string? Password { get; init; }
}

public class Credentials
{
    public Login? Dev { get; init; }
    public Login? Stg { get; init; }
    public Login? QA { get; init; }
    public Login? Prod { get; init; }
}

public class Options
{
    public Books? Books { get; init; }
    public Credentials? Credentials { get; init; }
}