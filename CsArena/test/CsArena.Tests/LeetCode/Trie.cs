using System.Text;

namespace CsArena.Tests.LeetCode;

/**
    A trie (https://en.wikipedia.org/wiki/Trie) or prefix tree is a tree data structure used to efficiently store and retrieve keys in a dataset of strings.
    There are various applications of this data structure, such as autocomplete and spellchecker.
    Solution https://leetcode.com/problems/implement-trie-prefix-tree/solutions/6177854/c-solution-by-victornovik-qf7p/
    # Approach
        Every Trie stores array of 26 links to other Tries and a flag if this node is terminal.
    # Complexity
        - Search:       O(n)
        - StartsWith:   O(n)
        - Insert:       O(n)
        - Delete:       O(n)
        - Space complexity: O(n)
*/
public class Trie
{
    public void Insert(string word)
    {
        var cur = this;
        foreach (var ch in word)
        {
            if (cur!.children[ch - StartCode] == null)
            {
                cur.children[ch - StartCode] = new Trie();
                cur.filledLetters++;
            }
            cur = cur.children[ch - StartCode];
        }
        cur!.isTerminal = true;
    }

    /**
        Write a function to find the longest common prefix string amongst an array of strings.
        If there is no common prefix, return an empty string.
        Solution https://leetcode.com/problems/longest-common-prefix/solutions/6202365/c-solution-beats-92-by-victornovik-tr6n
        # Approach
            Create a trie storing the count of filled letters on every trie level.
            We traverse this trie while every level has only 1 filled letter (no forking) and we don't meet a terminal node (in case of ["ab", "a"]).
        # Complexity
            - Time complexity: O(L) where L is the longest common prefix length
            - Space complexity: O(M) where M is the longest string length
    */
    public string LongestCommonPrefix()
    {
        var res = new StringBuilder();
        var cur = this;
        while (cur!.filledLetters == 1)
        {
            for (var i = 0; i < cur.children.Length; i++)
            {
                if (cur.children[i] != null)
                {
                    res.Append((char)(i + StartCode));
                    if (cur.children[i]!.isTerminal)
                        return res.ToString();
                    cur = cur.children[i];
                    break;
                }
            }
        }
        return res.ToString();
    }

    public bool Search(string word)
    {
        var cur = this;
        foreach (var ch in word)
        {
            if (cur!.children[ch - StartCode] == null)
                return false;
            cur = cur.children[ch - StartCode];
        }
        return cur!.isTerminal;
    }

    public bool StartsWith(string prefix)
    {
        var cur = this;
        foreach (var ch in prefix)
        {
            if (cur!.children[ch - StartCode] == null)
                return false;
            cur = cur.children[ch - StartCode];
        }
        return true;
    }

    /**
        Design a data structure that supports adding new words and finding if a string matches any previously added string.
        Solution https://leetcode.com/problems/design-add-and-search-words-data-structure/solutions/6177333/c-solution-by-victornovik-ukrw
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)    
        Returns true if there is any string in the data structure that matches word or false otherwise.
        `word` may contain dots '.' where dots can be matched with any one letter.
    */
    public bool SearchWildcard(string word)
    {
        var cur = this;
        for (var i = 0; i < word.Length; ++i)
        {
            var ch = word[i];
            if (ch != '.')
            {
                if (cur!.children[ch - StartCode] == null)
                    return false;
                cur = cur.children[ch - StartCode];
            }
            else
            {
                foreach (var trie in cur!.children)
                {
                    if (trie != null && trie.SearchWildcard(word[(i + 1)..]))
                        return true;
                }
                return false;
            }
        }
        return cur!.isTerminal;
    }
    
    private const int StartCode = 97;

    // 26 lower-case English letters ASCII codes [97; 122]
    public readonly Trie?[] children = new Trie[26];
    public bool isTerminal;
    // E.g. for ["ab", "ac"] only 1 letter "a" is used on the first trie level and 2 letters "b" and "c are filled on the second level.
    public int filledLetters;
}