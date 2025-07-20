namespace CsArena.Tests.collections;

internal static class ThrowHelper
{
    internal static void ThrowKeyNotFoundException()
    {
        throw new KeyNotFoundException();
    }

    internal static void ThrowInvalidOperationException()
    {
        throw new InvalidOperationException();
    }

    internal static void ThrowInvalidOperationException_EnumFailedVersion()
    {
        throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
    }
}