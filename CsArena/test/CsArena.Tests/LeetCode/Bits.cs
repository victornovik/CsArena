namespace CsArena.Tests.LeetCode;

public static class Bits
{

    /**
        Number of 1 Bits.
        Write a function that takes the binary representation of a positive integer and returns the number of 1 bits it has.
        Hamming Weight https://en.wikipedia.org/wiki/Hamming_weight the number of symbols in the string that are different from the zero-symbol.
        Hamming Distance https://en.wikipedia.org/wiki/Hamming_distance between two strings or vectors of equal length is the number of positions at which the corresponding symbols are different.
        Solution https://leetcode.com/problems/number-of-1-bits/solutions/5869356/rightmost-1-bit-reset-c-solution
        # Approach
            Zero the rightmost **1** bit of `n` until the whole number `n` turns into 0 and count how many such rightmost **1** bits were resetted.
        # Complexity
            - Time complexity: O(1) as int consists of 32 bits.
            - Space complexity: O(1)
     */
    public static int HammingWeight(int n)
    {
        var count = 0;
        while (n != 0)
        {
            ++count;
            n &= n - 1;
        }
        return count;
    }

    /**
        Given a non-empty array of integers nums, every element appears twice except for one. 
        Find that single one.
        You must implement a solution with a linear runtime complexity and use only constant extra space.
        Solution https://leetcode.com/problems/single-number/solutions/6265316/c-solution-hashset-by-victornovik-7b5k
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
    */
    public static int SingleNumber_HashSet(int[] nums)
    {
        var unique = new HashSet<int>();
        foreach (var n in nums)
        {
            if (!unique.Add(n))
                unique.Remove(n);
        }
        return unique.Single();
    }

    /**
        Given a non-empty array of integers nums, every element appears twice except for one.
        Find that single one.
        You must implement a solution with a linear runtime complexity and use only constant extra space.
        Solution https://leetcode.com/problems/single-number/solutions/6265408/c-one-liner-o1-by-victornovik-w10n
        # Approach
            Let's have a look at [2, 2, 1] array in binary [10, 10, 01]. Initial value of the aggregation is 0.
            1. 00 xor 10 = 10
            2. 10 xor 10 = 00
            3. 00 xor 01 = 01             
            After the end of the loop all values, met twice, will turn into 0 and the only single value will be left.
        # Complexity 
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int SingleNumber_Xor(int[] nums) => nums.Aggregate(0, (total, next) => total ^ next);

    /**
        Given an integer n, return an array ans of length n + 1 such that for each i (0 less i less n), 
        ans[i] is the number of 1's in the binary representation of i.
        Solution
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    //public int[] CountBits(int n)
    //{

    //}
}