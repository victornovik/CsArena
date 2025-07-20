namespace CsArena.Tests.LeetCode;

using static Strings;

public class StringsTests
{
    [Fact]
    public void RansomNoteTest()
    {
        Assert.False(CanConstructRansomNote("a", "b"));
        Assert.False(CanConstructRansomNote("aa", "ab"));
        Assert.True(CanConstructRansomNote("aa", "aab"));
        Assert.True(CanConstructRansomNote("gmaaeniz", "magazine"));
    }

    [Fact]
    public void IsAnagramTest()
    {
        Assert.True(IsAnagram_Linq("anagram", "nagaram"));
        Assert.False(IsAnagram_Linq("rat", "car"));

        Assert.True(IsAnagram_Array("anagram", "nagaram"));
        Assert.False(IsAnagram_Array("rat", "car"));

        Assert.True(IsAnagram_Dictionary("anagram", "nagaram"));
        Assert.False(IsAnagram_Dictionary("rat", "car"));
        Assert.True(IsAnagram_Dictionary("blablacar", "crabbllaa"));
    }

    [Fact]
    public void IsIsomorphicStringsTest()
    {
        Assert.False(IsIsomorphic_Dictionary("eg", "add"));
        Assert.True(IsIsomorphic_Dictionary("egg", "add"));
        Assert.False(IsIsomorphic_Dictionary("foo", "bar"));
        Assert.True(IsIsomorphic_Dictionary("paper", "title"));
        Assert.False(IsIsomorphic_Dictionary("badc", "baba"));

        Assert.False(IsIsomorphic_Array("eg", "add"));
        Assert.True(IsIsomorphic_Array("egg", "add"));
        Assert.False(IsIsomorphic_Array("foo", "bar"));
        Assert.True(IsIsomorphic_Array("paper", "title"));
        Assert.False(IsIsomorphic_Array("badc", "baba"));
        Assert.False(IsIsomorphic_Array("ab", "aa"));
        Assert.True(IsIsomorphic_Array("abc", "abc"));
    }

    [Fact]
    public void IsPalindromeTest()
    {
        Assert.True(IsPalindrome("A man, a plan, a canal: Panama"));
        Assert.False(IsPalindrome("race a car"));
        Assert.True(IsPalindrome(" "));
        Assert.True(IsPalindrome("a,,,,,,bba"));
        Assert.True(IsPalindrome("a."));
    }

    [Fact]
    public void HowManyGeneralSymbolsTest()
    {
        Assert.Equal(8, HowManyGeneralSymbols("abc", "abracadabra"));
        Assert.Equal(1, HowManyGeneralSymbols("abc", "HowMnyGenerlSymbolsTest"));
        Assert.Equal(0, HowManyGeneralSymbols("", "HowMnyGenerlSymbolsTest"));
        Assert.Equal(0, HowManyGeneralSymbols("abc", ""));
        Assert.Equal(0, HowManyGeneralSymbols("", ""));
        Assert.Equal(0, HowManyGeneralSymbols("def", "DEF"));
        Assert.Equal(2, HowManyGeneralSymbols("def", "been"));
        Assert.Equal(2, HowManyGeneralSymbols("dedfef", "been"));
    }

    [Fact]
    public void LongestDiverseStringTest()
    {
        Assert.Equal("ccbccacc", LongestDiverseString(1, 1, 7));
        Assert.Equal("aabaa", LongestDiverseString(7, 1, 0));
        Assert.Equal("aabbccbbaac", LongestDiverseString(4, 4, 3));
        Assert.Equal("ccbccbc", LongestDiverseString(0, 2, 5));
        Assert.Equal("ccbccbbccbbccbbccbc", LongestDiverseString(0, 8, 11));
        Assert.Equal("cc", LongestDiverseString(0, 0, 7));

        Assert.Equal("ccbccacc", LongestDiverseString2(1, 1, 7));
        Assert.Equal("aabaa", LongestDiverseString2(7, 1, 0));
        Assert.Equal("aabbccbbaac", LongestDiverseString2(4, 4, 3));
        Assert.Equal("ccbccbc", LongestDiverseString2(0, 2, 5));
        Assert.Equal("ccbccbccbbccbbccbbc", LongestDiverseString2(0, 8, 11));
        Assert.Equal("cc", LongestDiverseString2(0, 0, 7));
    }


    [Fact]
    public void LengthOfLastWordTest()
    {
        Assert.Equal(5, LengthOfLastWord("Hello World"));
        Assert.Equal(4, LengthOfLastWord("   fly me   to   the moon  "));
        Assert.Equal(6, LengthOfLastWord("luffy is still joyboy"));
    }

    [Fact]
    public void WordPatternTest()
    {
        Assert.True(WordPattern("abba", "dog cat cat dog"));
        Assert.False(WordPattern("abba", "dog cat cat fish"));
        Assert.False(WordPattern("aaaa", "dog cat cat dog"));
        Assert.True(WordPattern("abc", "b c a"));
    }
}