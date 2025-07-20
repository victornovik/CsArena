namespace CsArena.Tests.LeetCode;

public static class Intervals
{
    /**
        You are given a sorted unique integer array nums.
        A range [a,b] is the set of all integers from a to b (inclusive).
        Return the smallest sorted list of ranges that cover all the numbers in the array exactly.
        That is, each element of nums is covered by exactly one of the ranges,
        and there is no integer x such that x is in one of the ranges but not in nums.
        Each range [a,b] in the list should be output as: "a->b" if a != b, "a" if a == b
        Solution https://leetcode.com/problems/summary-ranges/solutions/5887969/c-solution-beats-93
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
     */
    public static IList<string> SummaryRanges(int[] nums)
    {
        List<string> res = [];
        for (var streakStart = 0; streakStart < nums.Length;)
        {
            var streakEnd = streakStart + 1;

            while (streakEnd < nums.Length && nums[streakEnd - 1] + 1 == nums[streakEnd])
                ++streakEnd;

            res.Add(streakEnd == streakStart + 1 ? $"{nums[streakStart]}" : $"{nums[streakStart]}->{nums[streakEnd - 1]}");
            streakStart = streakEnd;
        }
        return res;
    }
}