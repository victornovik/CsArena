namespace CsArena.Tests.LeetCode;

public static class Backtracking
{
    /**
        Given n pairs of parentheses, write a function to generate all combinations of well-formed parentheses.
        Solution https://leetcode.com/problems/generate-parentheses/solutions/6144157/beats-100-c-recursive-solution-by-victor-s0sy
        # Intuition
            Frankly speaking I was thinking "fuck knows how to solve this problem, let's just generate sequences and avoid stack overflow".
        # Complexity
            - Time complexity: O(2^n)
            - Space complexity: O(n)
     */
    public static IList<string> GenerateParentheses(int n)
    {
        var res = new List<string>();
        Generate("", 0, 0);
        return res;

        void Generate(string seq, int openCount, int closedCount)
        {
            if (seq.Length == n * 2)
            {
                res.Add(seq);
                return;
            }
            if (openCount < n)
                Generate(seq + "(", openCount + 1, closedCount);
            if (closedCount < openCount)
                Generate(seq + ")", openCount, closedCount + 1);
        }
    }

    /**
        Given an integer array `nums` of unique elements, return all possible subsets (the power set).
        A subset of an array is a tuple that can be obtained from the array by removing some (possibly all, possibly 0) elements of it.
        The solution set must not contain duplicate subsets. Return the solution in any order.
        Solution https://leetcode.com/problems/subsets/solutions/6214327/c-recursive-backtracking-solution-by-vic-c58s
        # Approach
            Number of subsets of an array of size N is 2^N
        # Complexity
            - Time complexity: O(2^n)
            - Space complexity: O(n)
     */
    public static IList<IList<int>> Subsets(int[] nums)
    {
        IList<IList<int>> res = [];
        NextSubset([], 0);
        return res;

        void NextSubset(IList<int> tmp, int start)
        {
            res.Add([..tmp]);
            for (var i = start; i < nums.Length; i++)
            {
                // Include current element in the subset
                tmp.Add(nums[i]);
                // Recursively generate subsets with current element included
                NextSubset(tmp, i + 1);
                // Backtrack (exclude current element)
                tmp.RemoveAt(tmp.Count - 1);
            }
        }
    }
}