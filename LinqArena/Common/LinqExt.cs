namespace LinqArena.Common;

public static class LinqExt
{
    public static int MyCount<T>(this IEnumerable<T> src)
    {
        var count = 0;
        foreach (var _ in src)
        {
            ++count;
        }
        return count;
    }

    public static IEnumerable<T> Filter<T>(this IEnumerable<T> src, Func<T, bool> pred)
    {
        foreach (var item in src)
        {
            if (pred(item))
                yield return item;
        }
    }

    public static IEnumerable<(T item, int index)> SelectWithIndex<T>(this IEnumerable<T> src)
    {
        var count = 0;
        foreach (var item in src)
        {
            yield return (item, count);
            ++count;
        }
    }

    public static IEnumerable<int> Random()
    {
        var rnd = new Random();
        while (true)
            yield return rnd.Next();
        // ReSharper disable once IteratorNeverReturns
    }

    public const string CsvPath = @"dataset\fuel.csv";
    public const string CsvMfgPath = @"dataset\manufacturers.csv";
    public const string XmlPath = @"dataset\fuel.xml";
}