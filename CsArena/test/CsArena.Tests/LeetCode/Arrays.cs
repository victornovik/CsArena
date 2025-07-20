namespace CsArena.Tests.LeetCode;

public static class Arrays
{
    /**
        Given an integer array `nums` and an integer `val`, remove all occurrences of val in the array in place. 
        The order of the elements may be changed. 
        To get accepted you need to do the following things:
            - Change the array nums such that the first k elements of nums contain the elements which are not equal to val. 
            - The remaining elements of nums are not important as well as the size of nums.
            - Return k - the number of elements in nums which are not equal to val.
        Solution https://leetcode.com/problems/remove-element/solutions/5847845/simple-c-solution/
        # Approach
            We have to move all non-`val` items to the beginning of the array.
            The simplest way is to re-write each item by a valid non-`val` item (even by itself) incrementing k on each write. 
            At the end first k items will contain all non-`val` elements in the array.
        # Complexity
            - Time complexity: O(N)
            - Space complexity: O(1)
    */
    public static int RemoveElement(int[] nums, int val)
    {
        var k = 0;
        for (var i = 0; i < nums.Length; i++)
        {
            if (nums[i] != val)
                nums[k++] = nums[i];
        }
        return k;
    }

    /**
        Given an integer array `nums` sorted in non-decreasing order, remove the duplicates in-place such that each unique element appears only once. 
        The relative order of the elements should be kept the same.                 
            - Change the array `nums` such that the first `unique` elements of nums contain the unique elements in the order they were present in nums initially. 
              The remaining elements of nums are not important as well as the size of nums.
            - Return the number of unique elements in `nums`.
        Solution https://leetcode.com/problems/remove-duplicates-from-sorted-array/solutions/6147137/beats-100-c-solution-by-victornovik-ivbn
        # Approach
            We have to move all unique items to the beginning of the array. 
            The simplest way is to re-write each item by a unique item (even by itself) incrementing `unique` on each write.
            At the end first `unique` items will contain all unique elements in the array.
        # Complexity
            - Time complexity: O(N)
            - Space complexity: O(1)
    */
    public static int RemoveDuplicates(int[] nums)
    {
        var unique = nums.Length > 0 ? 1 : 0;
        for (var i = 1; i < nums.Length; i++)
        {
            if (nums[i] != nums[i - 1])
                nums[unique++] = nums[i];
        }
        return unique;

        // return RemoveMoreThanKDuplicates(nums, 1);
    }

    /** -! GENERIC SOLUTION !-
    
        Given an integer array `nums` sorted in non-decreasing order, remove some duplicates in-place such that each unique element appears at most twice. 
        The relative order of the elements should be kept the same.
        The result has to be placed in the first part of the array nums. 
        It does not matter what you leave beyond the first `unique` elements.
            - Return `unique` after placing the final result in the first `unique` elements of `nums`.
            - You must do this by modifying the input array in-place with O(1) extra memory.
        Solution https://leetcode.com/problems/remove-duplicates-from-sorted-array-ii/solutions/6149261/c-solution-by-victornovik-1t4f    
        # Complexity
            - Time complexity: O(N)
            - Space complexity: O(1)
    */
    public static int RemoveMoreThanKDuplicates(int[] nums, int k = 2)
    {
        var unique = 0;
        foreach (var n in nums)
        {
            if (unique < k || n != nums[unique - k])
                nums[unique++] = n;
        }
        return unique;
    }

    /**
        Given two integer arrays nums1 and nums2, return an array of their intersection.
        Each element in the result must be unique and you may return the result in any order.
        Solution https://leetcode.com/problems/intersection-of-two-arrays/solutions/6180812/simple-c-solution-by-victornovik-qdgf
        # Approach
            1. Convert `nums1` to `uniqueNums1` hashset containing only unique `nums1` numbers.
            2. For each number in `nums2` if it is found in `uniqueNums1` put it into intersection array and remove from `uniqueNums1`.
        # Complexity
            - Time complexity: O(N + M) where N - nums1 length, M - nums2 length.
            - Space complexity: O(N)    
     */
    public static int[] UniqueIntersection(int[] nums1, int[] nums2)
    {
        List<int> intersection = [];
        var uniqueNums1 = nums1.ToHashSet();
        foreach (var i in nums2)
        {
            if (uniqueNums1.Remove(i))
                intersection.Add(i);
        }
        return intersection.ToArray();
    }

    /**
        Given two integer arrays nums1 and nums2, return an array of their FULL intersection.
        Each element in the result must appear as many times as it shows in both arrays and you may return the result in any order.
        Solution https://leetcode.com/problems/intersection-of-two-arrays-ii/solutions/6085579/c-solution-with-dictionary
        # Approach
            1. For each number in nums1 put its count into freq dictionary.
            2. For each number in nums2 if it is found in freq and its value is greater than 0 put it into intersection array. 
               Also we have to decrement its value in freq in order to prevent its redundant duplication in the intersection.
        # Complexity
            - Time complexity: O(N + M)
            - Space complexity: O(N)
     */
    public static int[] FullIntersection(int[] nums1, int[] nums2)
    {
        List<int> intersection = [];

        var freq = new Dictionary<int, int>(); // GroupBy(x => x).ToDictionary(grp => grp.Key, grp => grp.Count())
        foreach (var i in nums1)
        {
            freq[i] = freq.GetValueOrDefault(i, 0) + 1;
        }

        foreach (var i in nums2)
        {
            if (freq.TryGetValue(i, out var val))
            {
                if (val > 0)
                {
                    intersection.Add(i);
                    --freq[i];
                }
            }
        }
        return intersection.ToArray();
    }

    public static (int[] Full, int[] Unique) IntersectionOfThreeArrays(int[] nums1, int[] nums2, int[] nums3)
    {
        Dictionary<int, (int Count, bool Once)> Count(int[] arr)
        {
            var freq = new Dictionary<int, (int Count, bool Once)>();
            foreach (var i in arr)
            {
                if (freq.TryGetValue(i, out var tuple))
                {
                    tuple.Count++;
                    freq[i] = tuple;
                }
                else
                {
                    freq[i] = (Count: 1, Once: true);
                }
            }
            return freq;
        }

        var freq1 = Count(nums1);
        var freq2 = Count(nums2);

        List<int> fullIntersection = [];
        List<int> uniqueIntersection = [];

        foreach (var i in nums3)
        {
            if (freq1.TryGetValue(i, out var val1) && freq2.TryGetValue(i, out var val2))
            {
                if (val1.Once && val2.Once)
                {
                    uniqueIntersection.Add(i);
                    freq1[i] = (val1.Count, false);
                    freq2[i] = (val2.Count, false);
                }

                if (val1.Count > 0 && val2.Count > 0)
                {
                    fullIntersection.Add(i);
                    freq1[i] = (val1.Count - 1, false);
                    freq2[i] = (val2.Count - 1, false);
                }
            }
        }
        return (fullIntersection.ToArray(), uniqueIntersection.ToArray());
    }

    /**
        Given three arrays nums1, nums2 and nums3 sorted in non-decreasing order, return an array of integers that appear in all three arrays.            
        Solution (Requires Premium)
        # Approach
            1. Move forward the pointer to the smallest element.
            2. When all three pointers are equal add this number to the `intersection` and move forward all three pointers.
        # Complexity
            - Time complexity: O(N + M + L)
            - Space complexity: O(1)
     */
    public static int[] IntersectionOfThreeSortedArrays(int[] nums1, int[] nums2, int[] nums3)
    {
        List<int> intersection = [];
        for (int i = 0, j = 0, k = 0; i < nums1.Length && j < nums2.Length && k < nums3.Length;)
        {
            if (nums1[i] == nums2[j] && nums1[i] == nums3[k])
            {
                intersection.Add(nums1[i]);
                ++i; ++j; ++k;
            }
            else if (nums1[i] < nums2[j])
                ++i;
            else if (nums2[j] < nums3[k])
                ++j;
            else
                ++k;
        }
        return intersection.ToArray();
        // int Min(params int[] arr) => arr.Min();
    }

    /**
        Given an array of integers nums and an integer target, return indices of the two numbers such that they add up to target.
        Each input has exactly one solution and you may not use the same element twice.
        You can return the answer in any order.
        Solution https://leetcode.com/problems/two-sum/solutions/5848181/simple-c-solution/
        # Approach
            We store all unique numbers in the dictionary. 
            The key is the number itself, the value is the number index in `nums`. 
            In the same loop iteration we are looking for the second term in the dictionary (term1 + term2 = target). 
            If we've found it - game over. 
            If not - store the first term in the dictionary and take the next loop iteration.    
        # Complexity
            - Time complexity: O(N)
            - Space complexity: O(N)
     */
    public static int[] TwoSumInArray_Dictionary(int[] nums, int target)
    {
        var indices = new Dictionary<int, int>();
        for (var i1 = 0; i1 < nums.Length; i1++)
        {
            if (indices.TryGetValue(target - nums[i1], out var i2))
                return [i1, i2];
            indices[nums[i1]] = i1;
        }
        return [];
    }

    #region LINQ
    public static int[] TwoSumInArray_Linq(int[] nums, int target)
    {
        var indices = nums
            .Select((num, index) => (num, index))
            .GroupBy(i => i.Item1)
            .ToDictionary(g => g.Key, g => g.Select(i => i.Item2).ToArray());

        var halfTarget = target / 2;
        if (target % 2 == 0 && indices.TryGetValue(halfTarget, out var foundIndices) && foundIndices.Length > 1)
        {
            return foundIndices;
        }
        var found = nums.First(n => n != halfTarget && indices.ContainsKey(target - n));
        return [indices[found][0], indices[target - found][0]];
    }
    #endregion

    /**
        Given a 1-indexed array of integers `nums` that is already sorted in non-decreasing order, find two numbers such that they add up to a specific target number.
        Let these two numbers be numbers[index1] and numbers[index2] where 1 less or equal `index1` less `index2` less or equal `numbers.length`.
        Return the indices of the two numbers, index1 and index2, added by one as an integer array [index1, index2] of length 2.
        Each input has exactly one solution and you may not use the same element twice.
        NB: Your solution must use only constant O(1) extra space.     
        Solution https://leetcode.com/problems/two-sum-ii-input-array-is-sorted/solutions/5881417/simple-c-solution-binary-search
        # Intuition
            As we cannot create containers (space complexity **O(1)**) and the array is sorted the first idea that occurs to me is a binary search.
        # Complexity
            - Time complexity: O(n * log(n))
            - Space complexity: O(1)
     */
    public static int[] TwoSumInSortedArray_BinarySearch(int[] nums, int target)
    {
        for (var i1 = 0; i1 < nums.Length; i1++)
        {
            var i2 = Array.BinarySearch(nums, target - nums[i1]);
            if (i2 != i1 && i2 >= 0)
                return [int.Min(i1, i2) + 1, int.Max(i1, i2) + 1];
        }
        return [];
    }

    /**
         Solution https://leetcode.com/problems/two-sum-ii-input-array-is-sorted/solutions/5881462/simple-c-solution-two-pointers
         # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static int[] TwoSumInSortedArray_TwoPointers(int[] nums, int target)
    {
        for (int i = 0, j = nums.Length - 1; i < j;)
        {
            var sum = nums[i] + nums[j];
            if (sum < target)
                ++i; // Increase left iterator as we need the greater number
            else if (sum > target)
                --j; // Decrease right iterator as we need the lesser number
            else
                return [i + 1, j + 1];
        }
        return [];
    }

    /**
        Given an array of integers `nums` and integer `target`, return the total number of subarrays whose sum equals to `target`.
        A subarray is a contiguous non-empty sequence of elements within an array.
        Solution https://leetcode.com/problems/subarray-sum-equals-k/solutions/6157346/c-solution-with-running-total-by-victorn-jlrp
        Explanation https://leetcode.com/problems/subarray-sum-equals-k/solutions/268544/intuitively-diagnosis-problem-pattern-ja-5qa3/
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
    */
    public static int SubarraySum(int[] nums, int target)
    {
        var freq = new Dictionary<int, int> { [0] = 1 };
        int runningTotal = 0, subarrays = 0;
        foreach (var n in nums)
        {
            runningTotal += n;
            subarrays += freq.GetValueOrDefault(runningTotal - target, 0);
            freq[runningTotal] = freq.GetValueOrDefault(runningTotal, 0) + 1;
        }
        return subarrays;
    }

    /**
        Given an array of integers nums and an integer target, return the first subarray whose sum equals to target.
        A subarray is a contiguous non-empty sequence of elements within an array.        
    */
    public static (int left, int right) FindFirstSubarrayWithSum(int[] nums, int target)
    {
        var indices = new Dictionary<int, int> { [0] = -1 };
        var runningTotal = 0;
        for (var i = 0; i < nums.Length; ++i)
        {
            if (nums[i] == target)
                return (i, i);

            runningTotal += nums[i];
            if (indices.TryGetValue(runningTotal - target, out var j))
                return (j + 1, i);

            indices[runningTotal] = i;
        }
        return (-1, -1);
    }

    /**
        Given an integer array nums and an integer k, return true if nums has a good subarray or false otherwise.
        A good subarray is a subarray where:
            - its length is at least two
            - the sum of the elements of the subarray is a multiple of k.
        Note that:
            - A subarray is a contiguous part of the array.
            - An integer x is a multiple of k if there exists an integer n such that x = n * k. 0 is always a multiple of k.
        Solution https://leetcode.com/problems/continuous-subarray-sum/solutions/6157506/c-solution-by-victornovik-uqaj
    */
    public static bool FindFirstSubarrayWithSumMultipleK(int[] nums, int k)
    {
        var runningTotals = new Dictionary<int, int> { [0] = -1 };
        var runningTotal = 0;
        for (var rightBoundary = 0; rightBoundary < nums.Length; ++rightBoundary)
        {
            runningTotal += nums[rightBoundary];
            runningTotal %= k;

            if (runningTotals.TryGetValue(runningTotal, out var leftBoundary))
            {
                if (rightBoundary - leftBoundary > 1)
                    return true;
            }
            else
            {
                runningTotals[runningTotal] = rightBoundary;
            }
        }
        return false;
    }

    /**
       You are given two integer arrays `nums1` and `nums2`, sorted in non-decreasing order and two integers m and n with number of elements in nums1 and nums2.
       Merge `nums1` and `nums2` into a single array sorted in non-decreasing order.
       The final sorted array should not be returned by the function, but instead be stored inside the array `nums1`.
       To accommodate this, `nums1` has a length of m + n, where the first m elements denote the elements that should be merged.
       The last n elements are set to 0 and should be ignored. 
        `nums2` has a length of n.
       Solution https://leetcode.com/problems/merge-sorted-array/solutions/5978313/beats-100-c-solution
       # Approach
            As both `nums1` and `nums2` are already sorted we can start filling `nums1` from the end.
            We pick up a maximum among `nums1` and `nums2` and put it to the end of `nums1`.
            If we've already taken all numbers from `nums2` (i2 less 0) we stop the loop. 
            The untouched part of `nums1` is guaranteed to be sorted.
       # Complexity
           - Time complexity: O(m + n)
           - Space complexity: O(1)
    */
    public static void MergeSortedArraysInPlace(int[] nums1, int m, int[] nums2, int n)
    {
        for (int i1 = m - 1, i2 = n - 1, dst = n + m - 1; i2 >= 0;)
        {
            if (i1 >= 0)
                nums1[dst--] = nums1[i1] > nums2[i2] ? nums1[i1--] : nums2[i2--];
            else
                nums1[dst--] = nums2[i2--];
        }
    }

    /**
        Given an unsorted array of integers `nums`, return the length of the longest consecutive elements sequence.
        Those elements can be located in different places of the array (not in a row).
        You must write an algorithm that runs in O(n) time.
        Solution https://leetcode.com/problems/longest-consecutive-sequence/solutions/5885818/c-solution-with-hashset/
        # Approach
            1. Copy all `nums` into HashSet `uniqueNums` containing only unique numbers.
            2. After that we're trying to find out continuous streaks around every number.
                First we go left incrementing `streakLen` until find a break from the left.
                Then we go right incrementing `streakLen` until find a break from the right.
            3. During those steps to the left and to the right we remove already used numbers from `uniqueNums` to get rid of the same checks. It reduces the complexity.
            4. Despite the nested loops the whole complexity is O(n) because we eliminate all the used numbers and go through every number **only once**
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
     */
    public static int LongestConsecutive(int[] nums)
    {
        var maxStreak = 0;
        var uniqueNums = nums.ToHashSet();
        foreach (var n in uniqueNums)
        {
            var streakLen = 1;
            for (var i = n - 1; uniqueNums.Contains(i); --i)
            {
                uniqueNums.Remove(i);
                ++streakLen;
            }
            for (var i = n + 1; uniqueNums.Contains(i); ++i)
            {
                uniqueNums.Remove(i);
                ++streakLen;
            }
            maxStreak = int.Max(maxStreak, streakLen);
        }
        return maxStreak;
    }

    /**
        Given a binary array `nums`, return the maximum number of consecutive 1's in the array.
        Solution https://leetcode.com/problems/max-consecutive-ones/solutions/6147000/beats-100-c-solution-by-victornovik-jwrr
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int LongestConsecutiveOnes(int[] nums)
    {
        int maxCount = 0, countOfOnes = 0;
        foreach (var i in nums)
        {
            if (i == 1)
            {
                ++countOfOnes;
                maxCount = int.Max(maxCount, countOfOnes);
            }
            else
            {
                countOfOnes = 0;
            }
        }
        return maxCount;
    }

    /**
        Given a binary array `nums`, you should delete one element from it.
        Return the size of the longest non-empty subarray containing only 1's in the resulting array. 
        Return 0 if there is no such subarray.
        Solution https://leetcode.com/problems/longest-subarray-of-1s-after-deleting-one-element/solutions/6184913/c-solution-beats-100-by-victornovik-7ei0 
        # Approach
            The idea behind the solution is to find two adjacent islands of `1` divided by *single* `0` and calculate if their total length exceeds the maximum length. 
            We're looking for only *single* `0` because we can delete this single `0` and get just one contiguous sequence of `1`. 
            If we meet two or more `0` in a row we have to discard the previous `1` island from consideration. 
            For this purpose we set flag `wasZero` to see if the previous element was `0`.
            I prefer to check the maximum in `(i == 1)` clause.
            If we do it in `(i == 0)` clause then we have to check the maximum after the loop once again and it can be easily missed.
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int LongestSubarrayOfOnes(int[] nums)
    {
        var maxCount = 0;
        var curCount = 0;
        var prevCount = 0;
        var wasZero = false;

        foreach (var i in nums)
        {
            if (i == 1)
            {
                ++curCount;
                maxCount = int.Max(maxCount, prevCount + curCount);
                wasZero = false;
            }
            else
            {
                prevCount = wasZero ? 0 : curCount;
                curCount = 0;
                wasZero = true;
            }
        }
        // maxCount == nums.Length when `nums` consists of all ones
        return (maxCount == nums.Length) ? maxCount - 1 : maxCount;
    }

    /**
        Given an array nums. We define a running sum of an array as runningSum[i] = sum(nums[0]…nums[i]).
        Return the running sum of nums.
        Space complexity has to be O(1).
        Solution https://leetcode.com/problems/running-sum-of-1d-array/solutions/6185984/c-solution-by-victornovik-phk3
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int[] RunningSum(int[] nums)
    {
        for (var n = 1; n < nums.Length; n++)
            nums[n] += nums[n - 1];
        return nums;
    }

    /**
        Given a 0-indexed integer array `nums`, return true if it can be made strictly increasing after removing exactly one element, or false otherwise. 
        If the array is already strictly increasing, return true.
        The array nums is strictly increasing if nums[i - 1] strictly less than nums[i] for each i.
        Solution https://leetcode.com/problems/remove-one-element-to-make-the-array-strictly-increasing/solutions/6189824/c-solution-beats-100-by-victornovik-dgad
    */
    public static bool StrictlyIncreasingAfterOneElementRemoval(int[] nums)
    {
        var wasWrongElement = false;
        for (int i = 0, j = 1; j < nums.Length; i++, j++)
        {
            if (nums[j] <= nums[i])
            {
                if (wasWrongElement)
                    return false; // One wrong element has already been found
                wasWrongElement = true;

                // Check if nums[j] is wrong and skip it on the next iteration. Otherwise nums[j - 1] is wrong.
                if (i - 1 >= 0 && nums[j] <= nums[i - 1])
                    nums[j] = nums[i];
            }
        }
        return true;
    }

    /**
        Given an array of integers `nums`, find the subarray with the largest sum, and return its sum.
        Kadane's algorithm https://en.wikipedia.org/wiki/Maximum_subarray_problem#Kadane's_algorithm
        Solution https://leetcode.com/problems/maximum-subarray/solutions/6197487/c-solution-beats-98-by-victornovik-m4rm
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int MaxSubArray(int[] nums)
    {
        var maxSum = nums[0];
        var maxCurSum = nums[0];
        foreach (var n in nums[1..])
        {
            // Find the maximum sum of subarray ending at i by using the maximum sum of subarray ending at i - 1.
            // If the previous sum is positive then extend it by adding nums[i].
            // Otherwise start a new subarray from nums[i].
            maxCurSum = int.Max(maxCurSum + n, n);
            maxSum = int.Max(maxSum, maxCurSum);
        }
        return maxSum;
    }

    public static int[] MaxSubArrayReturned(int[] nums)
    {
        var maxSum = nums[0];
        var maxCurSum = nums[0];
        int subArrayStart = 0, subArrayEnd = 0, curStart = 0;

        for (var i = 1; i < nums.Length; ++i)
        {
            // Find the maximum sum of subarray ending at i by using the maximum sum of subarray ending at i - 1.
            // If the previous sum is positive then extend it by adding nums[i].
            // Otherwise start a new subarray from nums[i].
            if (nums[i] > maxCurSum + nums[i])
            {
                curStart = i;
                maxCurSum = nums[i];
            }
            else
            {
                maxCurSum += nums[i];
            }

            if (maxCurSum > maxSum)
            {
                subArrayStart = curStart;
                subArrayEnd = i;
                maxSum = maxCurSum;
            }
        }
        return nums[subArrayStart..(subArrayEnd + 1)];
    }

    /**
        Given a circular integer array `nums` of length n, return the maximum possible sum of a non-empty subarray of nums.
        A circular array means the end of the array connects to the beginning of the array. 
        Formally, the next element of nums[i] is nums[(i + 1) % n] and the previous element of nums[i] is nums[(i - 1 + n) % n].
        A subarray may only include each element of the fixed buffer `nums` at most once.        
        Kadane's algorithm https://en.wikipedia.org/wiki/Maximum_subarray_problem#Kadane's_algorithm
        Solution https://leetcode.com/problems/maximum-sum-circular-subarray/solutions/6200977/c-solution-by-victornovik-gglz/
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int MaxSubarraySumCircular(int[] nums)
    {
        int maxSum = nums[0], maxCurSum = nums[0], minSum = nums[0], minCurSum = nums[0], totalSum = nums[0];

        foreach (var n in nums[1..])
        {
            maxCurSum = int.Max(maxCurSum + n, n);
            maxSum = int.Max(maxSum, maxCurSum);

            minCurSum = int.Min(minCurSum + n, n);
            minSum = int.Min(minSum, minCurSum);

            totalSum += n;
        }
        return maxSum > 0 ? int.Max(maxSum, totalSum - minSum) : maxSum;
    }

    /**
        Given an array `nums` of size n, return the majority element.
        The majority element is the element that appears more than ⌊n / 2⌋ times. 
        You may assume that the majority element always exists in the array.
        Boyer–Moore majority vote algorithm https://en.wikipedia.org/wiki/Boyer%E2%80%93Moore_majority_vote_algorithm
        Solution https://leetcode.com/problems/majority-element/solutions/6206454/c-solution-beats-100-by-victornovik-xjbg
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int MajorityElement(int[] nums)
    {
        var count = 1;
        var majorityElement = nums[0];

        foreach (var n in nums[1..])
        {
            if (majorityElement == n)
            {
                if (++count > nums.Length / 2)
                    return majorityElement;
            }
            else
            {
                count--;
            }

            if (count == 0)
            {
                majorityElement = n;
                count = 1;
            }
        }
        return majorityElement;
    }

    /**
        Given an integer array nums, rotate the array to the right by k steps, where k is non-negative.
        Solution https://leetcode.com/problems/rotate-array/solutions/6206510/c-solution-beats-100-by-victornovik-5o8k
        # Approach
            Pay attention that `k` can be greater than the length. 
            So we have to find `k % nums.Length` and use it further as a shift. 
            E.g. for 3-elements array Rot4, Rot7 and Rot10 will give us the same result as Rot1. 
            Now let's take nums[1,2,3,4,5] and k=2.
                1. Reverse left 5-2 elements [**1,2,3**,4,5] -> [**3,2,1**,4,5]
                2. Reverse right 2 elements [3,2,1,**4,5**] -> [3,2,1,**5,4**]
                3. Reverse the whole array [**3,2,1,5,4**] -> [**4,5,1,2,3**]            
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static void Rotate(int[] nums, int k)
    {
        k %= nums.Length;
        if (k == 0)
            return;

        Array.Reverse(nums, nums.Length - k, k);
        Array.Reverse(nums, 0, nums.Length - k);
        Array.Reverse(nums);
    }

    /**
        Given an integer array `nums`, return true if any value appears at least twice in the array, 
        and return false if every element is distinct.
        Solution https://leetcode.com/problems/contains-duplicate/solutions/6222867/c-solution-by-victornovik-gfpx
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
    */
    public static bool ContainsDuplicate(int[] nums)
    {
        var unique = new HashSet<int>(nums.Length);
        foreach (var n in nums)
        {
            if (!unique.Add(n))
                return true;
        }
        return false;
    }

    /**
        Solution https://leetcode.com/problems/contains-duplicate/solutions/6222877/c-2-lines-solution-by-victornovik-26sj
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
    */
    public static bool ContainsDuplicate_Linq(int[] nums)
    {
        var unique = new HashSet<int>(nums.Length);
        return nums.Any(n => !unique.Add(n));
    }

    /**
        Given an integer array `nums` and an integer `k`, return true if there are two distinct indices i and j in the array 
        such that nums[i] == nums[j] and abs(i - j) less or equal `k`.
        Solution https://leetcode.com/problems/contains-duplicate-ii/solutions/6223007/c-solution-by-victornovik-xtw8
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
    */
    public static bool ContainsNearbyDuplicate(int[] nums, int k)
    {
        var unique = new Dictionary<int, int>(nums.Length);
        for (var i = 0; i < nums.Length; i++)
        {
            if (unique.TryGetValue(nums[i], out var j) && Math.Abs(i - j) <= k)
                return true;
            unique[nums[i]] = i;
        }
        return false;
    }

    /**
        You are given an integer array nums and two integers indexDiff and valueDiff.
        Find a pair of indices (i, j) such that:
            i != j
            abs(i - j) less or equal `indexDiff`
            abs(nums[i] - nums[j]) less or equal `valueDiff`
        Return true if such pair exists or false otherwise.
        Solution https://leetcode.com/problems/contains-duplicate-iii/solutions/6228702/the-shortest-c-solution-on-logindexdiff-vr9ao
        # Approach
            Maintain a sliding window of length `indexDiff` to store previous `indexDiff` elements. 
            Keep the window sorted to leverage O(log(indexDiff)) lookup.
            For each array element look for the window values in [-valueDiff; +valueDiff] neighborhood. 
            If at least one element found it means
                1. It is closer than `indexDiff` to the current element
                2. It is closer than `valueDiff` to the current element
        # Complexity
            - Time complexity: O(n * log(indexDiff))
            - Space complexity: O(n)
    */
    public static bool ContainsNearbyAlmostDuplicate(int[] nums, int indexDiff, int valueDiff)
    {
        var window = new SortedSet<int>();

        for (var i = 0; i < nums.Length; i++)
        {
            if (window.GetViewBetween(nums[i] - valueDiff, nums[i] + valueDiff).Count > 0)
                return true;

            if (i >= indexDiff)
                window.Remove(nums[i - indexDiff]);
            window.Add(nums[i]);
        }
        return false;
    }

    /**
        Given an array of positive integers `nums` and a positive integer target, return the minimal length of a subarray, whose sum is greater than or equal to target. 
        If there is no such subarray, return 0 instead.
        Solution https://leetcode.com/problems/minimum-size-subarray-sum/solutions/6233573/c-solution-beats-100-by-victornovik-m2h2
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int MinSubArrayLen_TwoPointers(int target, int[] nums)
    {
        var curSum = 0;
        var minLen = nums.Length + 1;

        for (int begin = 0, end = 0; end < nums.Length; end++)
        {
            if (nums[end] >= target)
                return 1;

            curSum += nums[end];

            while (curSum >= target)
            {
                minLen = int.Min(minLen, end - begin + 1); // `end - begin + 1` is window length

                // Early exit as 1 is the minimal possible subarray length
                if (minLen == 1)
                    return 1;

                curSum -= nums[begin++];
            }
        }
        return minLen % (nums.Length + 1);
    }

    /**
        Given an array of positive integers `nums` and a positive integer target, return the minimal length of a subarray, whose sum is greater than or equal to target.
        If there is no such subarray, return 0 instead.
        Solution https://leetcode.com/problems/minimum-size-subarray-sum/solutions/6233554/c-solution-with-queue-by-victornovik-227p
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
    */
    public static int MinSubArrayLen_Queue(int target, int[] nums)
    {
        var curSum = 0;
        var minLen = nums.Length + 1;
        var window = new Queue<int>();

        foreach (var n in nums)
        {
            if (n >= target)
                return 1;

            window.Enqueue(n);
            curSum += n;

            while (curSum >= target)
            {
                minLen = int.Min(minLen, window.Count);

                // Early exit as 1 is the minimal possible subarray length
                if (minLen == 1)
                    return 1;

                curSum -= window.Dequeue();
            }
        }
        return minLen % (nums.Length + 1);
    }
}