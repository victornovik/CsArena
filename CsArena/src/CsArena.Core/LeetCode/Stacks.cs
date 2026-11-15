using System.Collections.Frozen;

namespace CsArena.Tests.LeetCode;

public static class Stacks
{
    /**
        Given string s containing just the characters '(', ')', '{', '}', '[' and ']', determine if the input string is valid.
        An input string is valid if
            1. Open brackets must be closed by the same type of brackets.
            2. Open brackets must be closed in the correct order.
            3. Every close bracket has a corresponding open bracket of the same type.
        Solution https://leetcode.com/problems/valid-parentheses/solutions/6143955/simple-c-solution-with-stack-and-frozend-b5hh/              
        # Complexity
        - Time complexity: O(n)
        - Space complexity: O(n)
     */
    public static bool IsValidParentheses_FrozenDictionary(string s)
    {
        var allowed = new Dictionary<char, char> { { '{', '}' }, { '[', ']' }, { '(', ')' } }.ToFrozenDictionary();

        var openBrackets = new Stack<char>();
        foreach (var ch in s)
        {
            if (allowed.ContainsKey(ch))
                openBrackets.Push(ch);
            else if (openBrackets.Count == 0 || ch != allowed[openBrackets.Pop()])
                    return false;
        }
        return openBrackets.Count == 0;
    }

    /**        
        Solution https://leetcode.com/problems/valid-parentheses/solutions/6143986/straight-c-solution-beats-80-by-victorno-rthx
     */
    public static bool IsValidParentheses_Straight(string s)
    {
        var openBrackets = new Stack<char>();
        foreach (var ch in s)
        {
            if (ch == '{' || ch == '[' || ch == '(')
            {
                openBrackets.Push(ch);
            }
            else
            {
                if (openBrackets.Count == 0)
                    return false;

                var prevOpen = openBrackets.Pop();
                if ((ch == '}' && prevOpen != '{') || (ch == ']' && prevOpen != '[') || (ch == ')' && prevOpen != '('))
                    return false;
            }
        }
        return openBrackets.Count == 0;
    }
}