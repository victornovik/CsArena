namespace CsArena.Tests.ext;

public static class StringExtensions
{
    public static string Concat(this string? s1, string? s2)
    {
        return s1 + s2;
    }

    public static string ReplaceAt(this string s, int index, char ch)
    {
        var chars = s.ToCharArray();
        chars[index] = ch;
        return new string(chars);
    }
}