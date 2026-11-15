using System.Text;

namespace CsArena.Tests.LeetCode;

public static class Strings
{
    /**
       Given two strings `ransomNote` and `magazine,` return true if `ransomNote` can be constructed by using the letters from `magazine` and false otherwise.
       Each letter in `magazine` can only be used once in `ransomNote`.
       Solution https://leetcode.com/problems/ransom-note/solutions/5867932/simple-c-solution-by-victornovik-g2w7
       # Complexity
           - Time complexity: O(n + m)
           - Space complexity: O(1)
    */
    public static bool CanConstructRansomNote(string ransomNote, string magazine)
    {
        Dictionary<char, int> freq = new(magazine.Length);
        foreach (var ch in magazine)
            freq[ch] = freq.GetValueOrDefault(ch, 0) + 1;

        foreach (var ch in ransomNote)
        {
            if (!freq.TryGetValue(ch, out var count) || count < 1)
                return false;
            --freq[ch];
        }
        return true;
    }

    #region LINQ
    /**
        Solution https://leetcode.com/problems/valid-anagram/solutions/6147197/one-liner-c-linq-solution-by-victornovik-fgy2
        # Complexity
           - Time complexity: O(s*log(s) + t*log(t))
           - Space complexity: O(1)
    */
    public static bool IsAnagram_Linq(string s, string t)
    {
        return s.Order().SequenceEqual(t.Order());
    }
    #endregion

    /**
        Given two strings s and t, return true if t is an anagram of s, and false otherwise.
        Solution https://leetcode.com/problems/valid-anagram/solutions/5868824/simple-c-solution-by-victornovik-cs48
        # Approach
            NB: Works only for ASCII symbols [0; 255]
        # Complexity
            - Time complexity: O(s + t)
            - Space complexity: O(1)
     */
    public static bool IsAnagram_Array(string s, string t)
    {
        var freq = new int[256];
        foreach (var ch in s)
            freq[ch]++;
        foreach (var ch in t)
            freq[ch]--;
        return freq.All(count => count == 0);
    }

    /**
        Solution https://leetcode.com/problems/valid-anagram/solutions/6147190/simple-c-dictionary-solution-by-victorno-syel
        # Complexity
           - Time complexity: O(s + t)
           - Space complexity: O(s)
    */
    public static bool IsAnagram_Dictionary(string s, string t)
    {
        var freq = new Dictionary<char, int>(s.Length);
        foreach (var ch in s)
            freq[ch] = freq.GetValueOrDefault(ch, 0) + 1;

        foreach (var ch in t)
        {
            if (!freq.ContainsKey(ch))
                return false;
            freq[ch]--;
        }
        return freq.All(pair => pair.Value == 0);
    }

    /**
       Given two strings s and t, determine if they are isomorphic.
       Two strings s and t are isomorphic if the characters in s can be replaced to get t.
       All occurrences of a character must be replaced with another character while preserving the order of characters.
       No two characters may map to the same character, but a character may map to itself.
       Solution https://leetcode.com/problems/isomorphic-strings/solutions/5878338/simple-c-solution-two-dictionaries
       # Complexity
           - Time complexity: O(n)
           - Space complexity: O(n)
    */
    public static bool IsIsomorphic_Dictionary(string s, string t)
    {
        if (s.Length != t.Length)
            return false;

        Dictionary<char, char> s2t = new(), t2s = new();
        for (var i = 0; i < s.Length; ++i)
        {
            char ch1 = s[i], ch2 = t[i];
            if (s2t.TryGetValue(ch1, out var chFromT) && ch2 != chFromT)
                return false;
            if (t2s.TryGetValue(ch2, out var chFromS) && ch1 != chFromS)
                return false;

            s2t[ch1] = ch2;
            t2s[ch2] = ch1;
        }
        return true;
    }

    /**
        Solution https://leetcode.com/problems/isomorphic-strings/solutions/5878426/c-solution-charmap-arrays-beats-97/
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static bool IsIsomorphic_Array(string s, string t)
    {
        if (s.Length != t.Length)
            return false;

        int[] s2t = new int[256], t2s = new int[256];

        for (var i = 0; i < s.Length; ++i)
        {
            if (s2t[s[i]] != t2s[t[i]])
                return false;
            s2t[s[i]] = t2s[t[i]] = s[i];
        }
        return true;
    }

    /**
        A phrase is a palindrome if, after converting all uppercase letters into lowercase letters and removing all non-alphanumeric characters, it reads the same forward and backward.
        Alphanumeric characters include letters and numbers.
        Given a string s, return true if it is a palindrome, or false otherwise.
        Solution https://leetcode.com/problems/valid-palindrome/solutions/5879117/simple-c-solution-beats-92/
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static bool IsPalindrome(string s)
    {
        for (int i = 0, j = s.Length - 1; i < j;)
        {
            if (!char.IsLetterOrDigit(s[i]))
            {
                ++i;
                continue;
            }

            if (!char.IsLetterOrDigit(s[j]))
            {
                --j;
                continue;
            }

            if (char.ToLower(s[i]) != char.ToLower(s[j]))
                return false;

            ++i; --j;
        }
        return true;
    }

    /**
        How many symbols from s are met in j
        # Complexity
            - Time complexity: O(j + s)
            - Space complexity: O(j)
    */
    public static int HowManyGeneralSymbols(string j, string s)
    {
        var unique = j.ToHashSet();
        return s.Count(ch => unique.Contains(ch));
    }

    /**
        A string s is called happy if it satisfies the following conditions:
            s only contains the letters 'a', 'b', and 'c'.
            s does not contain any of "aaa", "bbb", or "ccc" as a substring.
            s contains at most a occurrences of the letter 'a'.
            s contains at most b occurrences of the letter 'b'.
            s contains at most c occurrences of the letter 'c'.
        Given 3 integers a, b, and c, return the longest possible happy string.
        If there are multiple longest happy strings, return any of them.
        If there is no such string, return the empty string "".
        A substring is a contiguous sequence of characters within a string.
        Solution https://leetcode.com/problems/longest-happy-string/solutions/5931508/c-solution-beats-100-1ms
        Solution for 2 symbols https://leetcode.com/problems/string-without-aaa-or-bbb/solutions/5933564/the-simplest-c-solution
        # Complexity
            - Time complexity: O(n * 3 * log(3))
            - Space complexity: O(n)        
     */
    public static string LongestDiverseString(int a, int b, int c)
    {
        StringBuilder[] abc = [new(), new(), new()];
        abc[0].Append('a', a);
        abc[1].Append('b', b);
        abc[2].Append('c', c);

        var res = new StringBuilder();
        do
        {
            // Sort in descending order. Maximum character count now lives in abc[0]
            Array.Sort(abc, (x, y) => y.Length - x.Length);

            if (abc[0].Length - abc[1].Length > 2)
            {
                // The most max count abc[0] greater than next abc[1] more than 2
                // Add next two characters from the most max bucket and one character from the 2-nd max bucket.
                TakeCharsFrom(index: 0, count: 2);
                TakeCharsFrom(index: 1, count: 1);
            }
            else
            {
                // The most max count abc[0] greater than next abc[1] less than 2
                // Add next two characters from the most max bucket and two characters from the 2-nd max bucket.
                TakeCharsFrom(index: 0, count: 2);
                TakeCharsFrom(index: 1, count: 2);
            }
        }
        while (abc[1].Length > 0 || abc[2].Length > 0);

        // abc[1] and [2] buckets ran out of symbols so add the remainder from the most max bucket
        if (res.Length > 0 && abc[0].Length > 0 && res[^1] != abc[0][0])
            TakeCharsFrom(0, 2);

        return res.ToString();

        void TakeCharsFrom(int index, int count)
        {
            var len = abc[index].Length;
            if (len == 0)
                return;
            if (len < count)
                count = len;
            res.Append(abc[index], 0, count);
            abc[index].Remove(0, count);
        }
    }

    /**
        Solution https://leetcode.com/problems/longest-happy-string/solutions/5932888/even-simpler-c-solution-beats-100-1ms
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(3)          
    */
    public static string LongestDiverseString2(int a, int b, int c)
    {
        (char ch, int count)[] abc = [('a', a), ('b', b), ('c', c)];

        var res = new StringBuilder();
        var prevCh = char.MinValue;
        int count;
        do
        {
            // Sort descendingly. Maximum character count lives in abc[0]
            Sort();

            var ch = abc[0].ch;
            if (ch == prevCh)
            {
                // We've already added two such characters running but this character still has the max count.
                // So add one character from the 2-nd max bucket.
                ch = abc[1].ch;
                count = int.Min(abc[1].count, 1);
                abc[1].count -= count;
            }
            else
            {
                // We've just added some different character so we can add next two characters from the greatest bucket.
                count = int.Min(abc[0].count, 2);
                abc[0].count -= count;
            }
            res.Append(ch, count);
            prevCh = ch;
        }
        while (count > 0);

        return res.ToString();

        // O(3) sort
        void Sort()
        {
            if (abc[1].count > abc[0].count)
                (abc[1], abc[0]) = (abc[0], abc[1]);
            if (abc[2].count > abc[0].count)
                (abc[2], abc[0]) = (abc[0], abc[2]);
            if (abc[2].count > abc[1].count)
                (abc[2], abc[1]) = (abc[1], abc[2]);
        }
    }

    /**
        Given a string `s` consisting of words and spaces, return the length of the last word in the string.
        A word is a maximal substring consisting of non-space characters only.
        There will be at least one word in `s`.
        Solution https://leetcode.com/problems/length-of-last-word/solutions/6202075/c-solution-beats-100-by-victornovik-l0ii
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int LengthOfLastWord(string s)
    {
        var start = s.Length - 1;
        var end = 0;

        for (; start >= 0; start--)
        {
            if (end == 0 && !char.IsWhiteSpace(s[start]))
                end = start;
            if (end != 0 && char.IsWhiteSpace(s[start]))
                break;
        }
        return end - start;
    }

    /**
        Given a pattern and a string s, find if s follows the same pattern.
        Here follow means a full match, such that there is a bijection between a letter in pattern and a non-empty word in s. 
        Specifically:
            - Each letter in pattern maps to exactly one unique word in s.
            - Each unique word in s maps to exactly one letter in pattern.
            - No two letters map to the same word, and no two words map to the same letter.
       Solution 
       # Complexity
           - Time complexity: O(n)
           - Space complexity: O(n)
    */
    public static bool WordPattern(string pattern, string s)
    {
        string[] words = s.Split();
        if (words.Length != pattern.Length)
            return false;

        Dictionary<char, string> p2s = new();
        Dictionary<string, char> s2p = new();

        for (var i = 0; i < pattern.Length; ++i)
        {
            var patt = pattern[i];
            var word = words[i];
            if (p2s.TryGetValue(patt, out var w) && w != word)
                return false;
            if (s2p.TryGetValue(word, out var c) && c != patt)
                return false;

            p2s[patt] = word;
            s2p[word] = patt;
        }
        return true;
    }
}