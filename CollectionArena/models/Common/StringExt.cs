using System.Text;

namespace CollectionArena.models.Common;

public static class StringExtensions
{
    public static T ParseEnum<T>(this string str) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), str);
    }

    public static IEnumerable<string> SplitCsv(this string line)
    {
        var builder = new StringBuilder();
        bool escaped = false, inQuotes = false;

        foreach (char ch in line)
        {
            switch (ch)
            {
                case ',' when !inQuotes:
                    yield return builder.ToString();
                    builder.Clear();
                    break;
                case '\\' when !escaped:
                    escaped = true;
                    break;
                case '"' when !escaped:
                    inQuotes = !inQuotes;
                    break;
                default:
                    escaped = false;
                    builder.Append(ch);
                    break;
            }
        }
        yield return builder.ToString();
    }
}