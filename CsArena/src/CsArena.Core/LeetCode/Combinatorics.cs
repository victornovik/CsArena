namespace CsArena.Tests.LeetCode;

public static class Combinatorics
{
    /**
        Permutations P(N) = N!
    */
    public static ulong P(ulong n)
    {
        return Maths.Fac(n);
    }

    public static int P(int n)
    {
        return Maths.Fac(n);
    }

    /**
        Allocations A(N by M) = N!/(N-M)!
    */
    public static ulong A(ulong n, ulong m)
    {
        var res = 1UL;
        for (var i = n; i > n - m; i--)
            res *= i;
        return res;
    }

    public static int A(int n, int m)
    {
        var res = 1;
        for (var i = n; i > n - m; i--)
            res *= i;
        return res;
    }

    /**
        Combinations C(N by M) = N!/(M! * (N-M)!)
    */
    public static ulong C(ulong n, ulong m)
    {
        var max = Math.Max(m, n - m);
        var min = Math.Min(m, n - m);

        var dividend = 1UL;
        for (var i = n; i > max; i--)
            dividend *= i;
        return dividend / Maths.Fac(min);
    }

    public static int C(int n, int m)
    {
        var max = Math.Max(m, n - m);
        var min = Math.Min(m, n - m);

        var dividend = 1;
        for (var i = n; i > max; i--)
            dividend *= i;
        return dividend / Maths.Fac(min);
    }

    /**
        Given a zero-based permutation `nums` change it in-place so nums[i] = nums[nums[i]] and return it.
        A zero-based permutation `nums` is an array of distinct integers from 0 to nums.length - 1 (inclusive).
        Space complexity has to be O(1).
        Solution https://leetcode.com/problems/build-array-from-permutation/solutions/6214987/c-solution-o1-by-victornovik-xjh9
        # Approach
            According to memory constraint O(1) we have to change the elements of the input array **in-place**.
            So we have to mix an old value together with a new value in the same array element in order to avoid a rewriting the old value by the new one.
            According to the problem specification any `nums[i]` less than `nums.length`
            so we can mix both old and new value in one number as follows `nums[i] = new * len + old`.
                1. We can fetch the old value by `nums[i] % len`
                2. We can fetch the new value by `nums[i] / len`
            P.S. For the sake of clarity we can use divisor 1000 since `nums.length` is always less than 1000 in the task.
                In that case last three digits of the array element contain an old value.
                Digits starting from the fourth digit contain a new value.
                E.g. **1***002* contains old value `2 = 1002 % 1000`and new value `1 = 1002 / 1000`.
                Very visual and obvious.
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static int[] BuildArrayFromPermutation(int[] nums)
    {
        var len = nums.Length;

        for (var i = 0; i < len; i++)
            nums[i] += nums[nums[i]] % len * len;

        for (var i = 0; i < len; i++)
            nums[i] /= len;

        return nums;
    }

    /**
        A permutation of an array of integers is an arrangement of its members into a sequence or linear order.
        For example, for arr [1,2,3], the following are all the permutations of arr: [1,2,3], [1,3,2], [2, 1, 3], [2, 3, 1], [3,1,2], [3,2,1].
        The next permutation of an array of integers is the next lexicographically greater permutation of its integers. 
        More formally, if all the permutations of the array are sorted in one container according to their lexicographical order, then the next permutation of that array is the permutation that follows it in the sorted container. 
        If such arrangement is not possible, the array must be rearranged as the lowest possible order (i.e., sorted in ascending order).
        For example, the next permutation of arr = [1,2,3] is [1,3,2].
        Similarly, the next permutation of arr = [2,3,1] is [3,1,2].
        While the next permutation of arr = [3,2,1] is [1,2,3] because [3,2,1] does not have a lexicographical larger rearrangement.
        Given an array of integers nums, find the next permutation of nums.
        The replacement must be in place and use only constant extra memory.
        Solution https://leetcode.com/problems/next-permutation/solutions/6209534/c-solution-beats-100-by-victornovik-ibn6
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static void NextPermutation(int[] nums)
    {
        // Find non-increasing tail from the end i.e. nums[i - 1] < nums[i]
        // i is zero if we reached final permutation sorted in descending order(anti-lexicographically). 
        var i = nums.Length - 1;
        while (i > 0 && nums[i - 1] >= nums[i])
            i--;

        // Find the first element from the end greater than i-1 and swap it with i-1
        for (var j = nums.Length - 1; i > 0 && j >= 0; j--)
        {
            if (nums[j] > nums[i - 1])
            {
                (nums[j], nums[i - 1]) = (nums[i - 1], nums[j]);
                break;
            }
        }
        // Permute the decreasing tail in increasing order
        Array.Reverse(nums, i, nums.Length - i);
    }

    /**
        Given an array nums of distinct integers, return all the possible permutations.
        Solution https://leetcode.com/problems/permutations/solutions/6209854/c-solution-by-victornovik-s3p5
        # Approach
            Call `NextPermutation()` developed in [31. Next Permutation](https://leetcode.com/problems/next-permutation/solutions/6209534/c-solution-beats-100-by-victornovik-ibn6/) 
            until it returns the initial permutation that means we've permuted all the options.
        # Complexity
            - Time complexity: O(n! * n)
            - Space complexity: O(n!)
    */
    public static IList<IList<int>> FindAllPermutations(int[] nums)
    {
        IList<IList<int>> res = [];
        var initial = nums.ToArray();
        do
        {
            NextPermutation(nums);
            res.Add(nums.ToArray());
        }
        while (!initial.SequenceEqual(nums));
        return res;
    }

    /**
        Given an array nums of distinct integers, return all the possible permutations.
        Solution https://leetcode.com/problems/permutations/solutions/6214302/c-recursive-backtracking-solution-by-vic-mm01
        # Approach
            Backtracking 
                - https://www.geeksforgeeks.org/introduction-to-backtracking-2/
                - https://www.geeksforgeeks.org/write-a-c-program-to-print-all-permutations-of-a-given-string/
        # Complexity
            - Time complexity: O(n! * n)
            - Space complexity: O(n!)
    */
    public static IList<IList<int>> FindAllPermutations_Backtracking(int[] nums)
    {
        IList<IList<int>> res = [];
        Permute(nums, 0);
        return res;

        void Permute(int[] perm, int start)
        {
            if (start == nums.Length - 1)
            {
                res.Add(perm.ToArray());
                return;
            }

            for (var i = start; i < nums.Length; i++)
            {
                // Swap `start` with `i`
                (nums[start], nums[i]) = (nums[i], nums[start]);
                // First `start` elements are fixed
                Permute(perm, start + 1);
                // Backtrack (rollback swapping `start` with `i`)
                (nums[start], nums[i]) = (nums[i], nums[start]);
            }
        }
    }

    /**
        E.g. let's consider C(7,5) = 21.
        The first combinations in lexicographical order are [1,2,3,4,5], [1,2,3,4,6], [1,2,3,4,7], [1,2,3,5,6],  [1,2,3,5,7] ... [3,4,5,6,7]
        Given an array of integers `nums`, find the next combination of `nums`.
        Return `true` if we generated the next combination, otherwise return `false`
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static bool NextCombination(int[] nums, int n)
    {
        // Find the first element from the end than can be increased to become <= n
        var i = nums.Length - 1;
        while (i >= 0 && nums[i] + nums.Length - i > n)
            i--;

        // Cannot generate the next combination as we reached the last one in lexicographical order
        if (i < 0)
            return false;
        nums[i]++;

        // Fill the right part after nums[i] as nums[i] + 1, nums[i] + 2, ...
        for (var j = i + 1; j < nums.Length; j++)
            nums[j] = nums[j - 1] + 1;

        return true;
    }

    /**
        Given two integers n and k, return all possible combinations of k numbers chosen from the range [1, n].
        You may return the answer in any order.
        Solution https://leetcode.com/problems/combinations/solutions/6270111/c-iterative-solution-beats-100-by-victor-wrlk
        # Complexity
            - Time complexity: O(k * C(n,k))
            - Space complexity: O(C(n,k))
     */
    public static IList<IList<int>> FindAllCombinations(int n, int k)
    {
        // Initial combination is 1..k
        var nums = Enumerable.Range(1, k).ToArray();

        IList<IList<int>> res = [];
        do
        {
            res.Add(nums.ToArray());
        } while (NextCombination(nums, n));
        return res;
    }

    /**
        Given two integers n and k, return all possible combinations of k numbers chosen from the range [1, n].
        You may return the answer in any order.
        Solution https://leetcode.com/problems/combinations/solutions/6270239/c-backtracking-solution-beats-100-by-vic-a8zp
        # Complexity
            - Time complexity: O(k * C(n,k))
            - Space complexity: O(C(n,k))
     */
    public static IList<IList<int>> FindAllCombinations_Backtracking(int n, int k)
    {
        IList<IList<int>> res = [];
        Combine([], 1);
        return res;

        void Combine(List<int> combination, int start)
        {
            if (combination.Count == k)
            {
                res.Add(combination.ToArray());
                return;
            }
            for (var i = start; i <= n; i++)
            {
                combination.Add(i);
                Combine(combination, i + 1);
                combination.RemoveAt(combination.Count - 1);
            }
        }
    }
}