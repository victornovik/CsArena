using System.Collections;

namespace CollectionArena.models.Common;

internal class SinglePassSequence<T>(IEnumerable<T> other) : IEnumerable<T>
{
    private IEnumerable<T> Sequence { get; } = other;
    private bool Used { get; set; }

    public IEnumerator<T> GetEnumerator()
    {
        if (Used)
            throw new InvalidOperationException("Cannot reuse sequence");
        Used = true;
        return Sequence.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}