namespace CsArena.Tests.LeetCode;

public class TriesTests
{
    [Fact]
    public void TrieTest()
    {
        var trie = new Trie();

        trie.Insert("apple");
        Assert.True(trie.Search("apple"));
        Assert.False(trie.Search("app"));
        Assert.True(trie.StartsWith("app"));

        trie.Insert("app");
        Assert.True(trie.Search("app"));
    }

    [Fact]
    public void WordDictionaryTest()
    {
        var wd = new Trie();
        wd.Insert("bad");
        wd.Insert("dad");
        wd.Insert("mad");
        Assert.False(wd.SearchWildcard("pad"));
        Assert.True(wd.SearchWildcard("bad"));
        Assert.True(wd.SearchWildcard(".ad"));
        Assert.True(wd.SearchWildcard("b.."));

        wd = new Trie();
        wd.Insert("a");
        wd.Insert("a");
        Assert.True(wd.SearchWildcard("."));
        Assert.True(wd.SearchWildcard("a"));
        Assert.False(wd.SearchWildcard("aa"));
        Assert.True(wd.SearchWildcard("a"));
        Assert.False(wd.SearchWildcard(".a"));
        Assert.False(wd.SearchWildcard("a."));
    }

    [Fact]
    public void LongestCommonPrefixTest()
    {
        var lcp = new Trie();
        lcp.Insert("flower");
        lcp.Insert("flow");
        lcp.Insert("flight");
        Assert.Equal("fl", lcp.LongestCommonPrefix());

        lcp = new Trie();
        lcp.Insert("flower");
        lcp.Insert("flow");
        lcp.Insert("floor");
        Assert.Equal("flo", lcp.LongestCommonPrefix());

        lcp = new Trie();
        lcp.Insert("dog");
        lcp.Insert("racecar");
        lcp.Insert("car");
        Assert.Equal("", lcp.LongestCommonPrefix());

        //lcp = new Trie();
        //lcp.Insert("");
        //lcp.Insert("b");
        //Assert.Equal("", lcp.LongestCommonPrefix());

        lcp = new Trie();
        lcp.Insert("ab");
        lcp.Insert("a");
        Assert.Equal("a", lcp.LongestCommonPrefix());
    }
}