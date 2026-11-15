namespace CsArena.Tests;

/// <summary>
/// Per-test timeouts in milliseconds, for tests that block threads, wait on OS primitives or spin up threads.
/// A test that exceeds its timeout fails and the run continues, instead of one deadlock hanging the whole run.
/// </summary>
public static class TestTimeouts
{
    /// <summary>Five seconds: the slowest test in the suite takes about a second, which leaves headroom for slow CI machines.</summary>
    public const int Default = 5_000;
}
