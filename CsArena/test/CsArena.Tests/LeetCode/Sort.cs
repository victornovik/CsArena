using System.Text;

namespace CsArena.Tests.LeetCode;

public static class Sort
{
    /**
        Given two arrays arr1 and arr2, the elements of arr2 are distinct, and all elements in arr2 are also in arr1.
        Sort the elements of arr1 such that the relative order of items in arr1 is the same as in arr2.
        Elements that do not appear in arr2 should be placed at the end of arr1 in ascending order.
        Solution (Array.Sort)   https://leetcode.com/problems/relative-sort-array/solutions/6009557/solution-dictionary/
        Solution (my MergeSort) https://leetcode.com/problems/relative-sort-array/solutions/6007610/solution-dictionary-and-mergesort/
        # Approach
            1. Fill dictionary `relativeOrder` where every `arr2` element maps to its index within `arr2`
            2. Sort `arr1` with custom comparer that determines that `x` less than `y` if `xIndex` in `arr2` is less than `yIndex` in `arr2`.
                - If either `x` or `y` is not found then it has to be placed after all found elements.
                - If both `x` and `y` are not found then they have to be placed after all found elements in ascending order.
        # Complexity
            - Time complexity: O(n * log(n))
            - Space complexity: O(n)
     */
    public static int[] RelativeSortArray(int[] arr1, int[] arr2)
    {
        // Map every arr2 element to its index within arr2
        var relativeOrder = Enumerable.Range(0, arr2.Length).ToDictionary(keySelector: x => arr2[x]);

        //MergeSort(arr1, 0, arr1.Length - 1, Comparer);
        Array.Sort(arr1, Comparer);
        return arr1;

        int Comparer(int x, int y)
        {
            var xFound = relativeOrder.TryGetValue(x, out var xIndex);
            var yFound = relativeOrder.TryGetValue(y, out var yIndex);

            if (xFound && yFound)
                return xIndex.CompareTo(yIndex);
            if (xFound && !yFound)
                return -1; // If y not found in arr2 it is greater than any arr1 element
            if (!xFound && yFound)
                return +1; // If x not found in arr2 it is greater than any arr1 element
            return x.CompareTo(y); // If both not found in arr2 then compare them each other
        }
    }

    /**
        You are given two strings order and s.
        All the characters of order are unique and were sorted in some custom order previously.
        Permute the characters of s so that they match the order that order was sorted.
        More specifically, if a character x occurs before a character y in order, then x should occur before y in the permuted string.
        Return any permutation of s that satisfies this property.
        Solution https://leetcode.com/problems/custom-sort-string/solutions/6010109/c-solution
        # Approach
            1. Fill dictionary `relativeOrder` where every `order` element maps to its index within `order`
            2. Sort `s` with custom comparer that determines that `x` less than `y` if `xIndex` in `relativeOrder` is less than `yIndex` in `relativeOrder`.
                - If either `x` or `y` is not found then it has to be placed after all found elements.
                - If both `x` and `y` are not found then they have to be placed after all found elements in ascending order.
        # Complexity
            - Time complexity: O(n * log(n))
            - Space complexity: O(n)
    */
    public static string CustomSortString(string order, string s)
    {
        // Map every `order` character to its index within `order`
        Dictionary<char, int> relativeOrder =
            order.Select((value, index) => (value, index)).ToDictionary(p => p.value, p => p.index);

        var chars = s.ToCharArray();
        Array.Sort(chars, Comparer);
        return new string(chars);

        int Comparer(char x, char y)
        {
            var xFound = relativeOrder.TryGetValue(x, out var xIndex);
            var yFound = relativeOrder.TryGetValue(y, out var yIndex);

            if (xFound && yFound)
                return xIndex.CompareTo(yIndex);
            if (xFound && !yFound)
                return -1; // If y not found in arr2 it is greater than any arr1 element
            if (!xFound && yFound)
                return +1; // If x not found in arr2 it is greater than any arr1 element
            return x.CompareTo(y); // If both not found in arr2 then compare them each other
        }
    }

    public static int[] RelativeSortArray_Linq1(int[] arr1, int[] arr2)
    {
        var relativeOrder = arr2
            .Select((value, index) => (value, index))
            .ToDictionary(p => p.value, p => p.index);
        return arr1
            .OrderBy(x => relativeOrder.ContainsKey(x) ? 0 : 1)
            .ThenBy(x => relativeOrder.GetValueOrDefault(x, x))
            .ToArray();
    }

    public static int[] RelativeSortArray_Linq2(int[] arr1, int[] arr2)
    {
        var other = new List<int>();
        var found = arr2.ToDictionary(k => k, _ => 0);

        foreach (var item in arr1)
        {
            if (found.ContainsKey(item))
            {
                found[item] += 1;
                continue;
            }

            other.Add(item);
        }

        other.Sort();
        return arr2.SelectMany(val => Enumerable.Repeat(val, found[val])).Concat(other).ToArray();
    }

    /**
        You are given an array of strings names, and an array heights that consists of distinct positive integers.
        Both arrays are of length n.
        For each index i, names[i] and heights[i] denote the name and height of the ith person.
        Return names sorted in descending order by the people's heights.
        # Approach
            1. Project every `names` element to name and its index
            2. Order descending by height taken from `heights`
            3. Project every tuple to name and return
        # Complexity
            - Time complexity: O(n * log(n))
            - Space complexity: O(n)
        Solution https://leetcode.com/problems/sort-the-people/solutions/6010759/c-one-line-linq-solution
    */
    public static string[] SortPeople(string[] names, int[] heights)
    {
        return names
            .Select((name, index) => (name, index))
            .OrderByDescending(el => heights[el.index])
            .Select(el => el.name)
            .ToArray();
    }

    /**
        Given a string s, sort it in decreasing order based on the frequency of the characters.
        The frequency of a character is the number of times it appears in the string.
        Return the sorted string.
        Solution https://leetcode.com/problems/sort-characters-by-frequency/solutions/6010590/c-fully-linq-solution
        # Approach
            1. Group by every character in `s` and calculate group count
            2. Order descending by `count`
            3. Append every character `count` times to the resulting string
        # Complexity
            - Time complexity: O(n * log(n))
            - Space complexity: O(n)
    */
    public static string SortByFrequency(string s)
    {
        var query = s
            .GroupBy(ch => ch, (ch, entries) => (ch, count: entries.Count()))
            .OrderByDescending(gr => gr.count)
            .Aggregate(new StringBuilder(s.Length), (sb, gr) =>
            {
                sb.Append(gr.ch, gr.count);
                return sb;
            });
        return query.ToString();
    }

    /**
        Given an array of integers nums, sort the array in ascending order and return it.
        You must solve the problem without using any built-in functions in O(n * log(n)) time complexity and with the smallest space complexity possible.
        Solution https://leetcode.com/problems/sort-an-array/solutions/6007282/c-mergesort
        Approach
            First I implemented QuickSort. But TestCase #19 contains 50000 elements sorted in descending order.
            Descending order is the worst case for QuickSort algo and its complexity gets O(n²) instead of O(n ∗ log(n)).
            So I implemented MergeSort because even in the worst case it works O(n ∗ log(n))
        Complexity
            Time complexity: O(n ∗ log(n))
            Space complexity: O(n)
    */
    public static int[] MergeSortArray(int[] arr)
    {
        if (arr.Length < 2)
            return arr;
        MergeSort(arr, 0, arr.Length - 1, (x, y) => x < y);
        return arr;
    }

    public static void MergeSort(int[] arr, int first, int last, Func<int, int, bool> pred)
    {
        if (first < last)
        {
            var middle = (first + last) / 2;
            MergeSort(arr, first, middle, pred);
            MergeSort(arr, middle + 1, last, pred);
            MergeArrays(arr, first, middle, middle + 1, last, pred);
        }
    }

    /**
        Given 2 arrays stored in the same array arr in a row.
        first1, last1 and first2, last2 indices within arr are their starts and ends.
        Merge them into a sorted sequence and write back to arr.
     */
    public static void MergeArrays(int[] arr, int first1, int last1, int first2, int last2, Func<int, int, bool> pred)
    {
        var res = new int[last2 - first1 + 1];
        var dst = first1;
        var resIndex = 0;

        while (first1 <= last1 && first2 <= last2)
        {
            if (pred(arr[first1], arr[first2]))
                res[resIndex++] = arr[first1++];
            else
                res[resIndex++] = arr[first2++];
        }

        while (first1 <= last1)
            res[resIndex++] = arr[first1++];
        while (first2 <= last2)
            res[resIndex++] = arr[first2++];

        // Write merged sorted sequence back to arr
        foreach (var t in res)
            arr[dst++] = t;
    }

    /**
        Given an array nums with n objects colored red, white, or blue, sort them in-place so that objects of the same color are adjacent, with the colors in the order red, white, and blue.
        We will use the integers 0, 1, and 2 to represent the color red, white, and blue, respectively.
        You must solve this problem without using the library's sort function.
        # Intuition
            If sort complexity is required to be O(n) then it could be radix sort.
        # Approach
            We can drastically simplify the radix sort given the fact we've got only 3 different numbers in the array by just counting them.
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
        Solution https://leetcode.com/problems/sort-colors/solutions/5981434/beats-100-c-solution
        Counting sort: https://ru.wikipedia.org/wiki/%D0%A1%D0%BE%D1%80%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%BA%D0%B0_%D0%BF%D0%BE%D0%B4%D1%81%D1%87%D1%91%D1%82%D0%BE%D0%BC
        Stable version: https://ru.wikipedia.org/wiki/%D0%A1%D0%BE%D1%80%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%BA%D0%B0_%D0%BF%D0%BE%D0%B4%D1%81%D1%87%D1%91%D1%82%D0%BE%D0%BC#%D0%A3%D1%81%D1%82%D0%BE%D0%B9%D1%87%D0%B8%D0%B2%D1%8B%D0%B9_%D0%B0%D0%BB%D0%B3%D0%BE%D1%80%D0%B8%D1%82%D0%BC
     */
    public static void SortColors(int[] nums)
    {
        int[] colors = [0, 0, 0];
        foreach (var n in nums)
        {
            ++colors[n];
        }

        Array.Fill(nums, 0, 0, colors[0]);
        Array.Fill(nums, 1, colors[0], colors[1]);
        Array.Fill(nums, 2, colors[0] + colors[1], colors[2]);
    }

    public static int[] RadixSortArray_Counting(int[] arr)
    {
        // Sort by every digit from right to left
        var max = arr.Max();
        for (var exp = 1; max / exp > 0; exp *= 10)
            arr = CountingSort(arr, exp);
        return arr;

        // Stable counting sort
        static int[] CountingSort(int[] arr, int exp)
        {
            // Count how many numbers have the least significant digit from 0 to 9
            // E.g. counts[4] = 5 means five numbers have LSD 4.
            var counts = new int[10];
            foreach (var i in arr)
                ++counts[i / exp % 10];

            // Count running total how many numbers have this digit and all previous digits.
            // E.g. counts[0] = 1, counts[1] = 3, counts[2] = 0 will become counts[0] = 1, counts[1] = 4, counts[2] = 4
            // It is needed to sort by LSD and make it stable.
            for (var i = 1; i < counts.Length; i++)
                counts[i] += counts[i - 1];

            var sorted = new int[arr.Length];
            for (var i = arr.Length - 1; i >= 0; i--)
            {
                var remainder = arr[i] / exp % 10;
                sorted[counts[remainder] - 1] = arr[i];
                --counts[remainder];
            }

            return sorted;
        }
    }

    public static int[] RadixSortArray_Buckets(int[] arr)
    {
        // Sort by every digit from right to left
        var max = arr.Max();
        for (var exp = 1; max / exp > 0; exp *= 10)
            BucketSort(arr, exp);
        return arr;

        static void BucketSort(int[] arr, int exp)
        {
            // Place input array numbers to one of the buckets from 0 to 9
            var buckets = new List<int>?[10];
            foreach (var i in arr)
            {
                ref var bucket = ref buckets[i / exp % 10];
                bucket ??= [];
                bucket.Add(i);
            }

            // Merge buckets sorted from [0] to [9] into arr
            var dst = 0;
            foreach (var list in buckets)
            {
                for (var i = 0; list != null && i < list.Count; ++i)
                    arr[dst++] = list[i];
            }
        }
    }

    /**
        Can sort negative values
     */
    public static int[] RadixSortArray_Bitwise(int[] arr)
    {
        if (arr.Length == 0)
            return arr;

        var tmp = new int[arr.Length];
        for (var shift = sizeof(int) * 8 - 1; shift > -1; --shift)
        {
            var j = 0;

            for (var i = 0; i < arr.Length; ++i)
            {
                var move = (arr[i] << shift) >= 0;

                // If the current bit matches the required position
                if (shift == 0 ? !move : move)
                    arr[i - j] = arr[i]; // Move the element to the left side of the array
                else
                    tmp[j++] = arr[i];
            }

            Array.Copy(tmp, 0, arr, arr.Length - j, j);
        }

        return arr;
    }

    /**
        Partition an array such way that all elements where `pred` is true come before elements where `pred` is false.
        Relative order of the elements is not preserved.
        # Approach
            Two pointers, `i` points to the element after the last true element, `j` goes through the whole array
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
     */
    public static void PartitionArray(int[] arr, Func<int, bool> pred)
    {
        var i = 0;
        while (pred(arr[i]) && i < arr.Length - 1)
            ++i;

        for (var j = i + 1; j < arr.Length; ++j)
        {
            if (pred(arr[j]))
            {
                (arr[i], arr[j]) = (arr[j], arr[i]);
                ++i;
            }
        }
    }

    /**
        Take the first element as a pivot and move lesser elements to the left and greater elements to the right.
        Move the pivot element to the position between the left and the right parts and return its index.
     */
    public static int PartitionArray(int[] arr, int first, int last, bool ascending = true)
    {
        var pivot = arr[first];
        var pivotIndex = first;
        for (var i = first + 1; i <= last; ++i)
        {
            if (ascending ? arr[i] < pivot : arr[i] > pivot)
            {
                ++pivotIndex;
                (arr[pivotIndex], arr[i]) = (arr[i], arr[pivotIndex]);
            }
        }
        (arr[pivotIndex], arr[first]) = (arr[first], arr[pivotIndex]);
        return pivotIndex;
    }

    /**
        Given an array of integers nums, sort the array in ascending order and return it.
        You must solve the problem without using any built-in functions in O(n * log(n)) time complexity and with the smallest space complexity possible.
        Solution
    */
    public static int[] QuickSortArray(int[] nums)
    {
        QuickSortArray(nums, 0, nums.Length - 1);
        return nums;
    }

    public static void QuickSortArray(int[] arr, int first, int last)
    {
        if (first < last)
        {
            var pivot = PartitionArray(arr, first, last);
            QuickSortArray(arr, first, pivot - 1);
            QuickSortArray(arr, pivot + 1, last);
        }
    }

    /**
        Given an integer array nums, reorder it such that nums[0] less nums[1] greater nums[2] less nums[3]...
        You may assume the input array always has a valid answer.
        Solution https://leetcode.com/problems/wiggle-sort-ii/solutions/6016341/c-linq-solution
        # Approach
            1. Sort an input array in descending order
            2. Find the middle element
            3. Merge two subarrays where the first one is [middle, end], the second one is [start, middle).
                If the array length is odd then the last element will be left intact and it's gonna be the least value of the array.
                It is needed for correct `Zip()` work that requires equal size of two zipped arrays.
            4. Write the wiggle-sorted array back to the input array.

        # Complexity
            - Time complexity: O(n * log(n))
            - Space complexity: O(n)
    */
    public static void SortWiggle_BySort(int[] nums)
    {
        if (nums.Length < 2)
            return;

        Array.Sort(nums, (x, y) => y.CompareTo(x));
        var middle = nums.Length / 2;

        nums[middle..]
            .Zip(nums[..middle], (first, second) => new[] { first, second })
            .SelectMany(arr => arr)
            .ToArray()
            .CopyTo(nums, 0);
    }

    /**
        Solution (odd and even indices) https://leetcode.com/problems/wiggle-sort-ii/solutions/6033122/naive-implementation/
    */
    public static void SortWiggle_BySort2(int[] nums)
    {
        var count = nums.Length / 2;
        foreach (var item in nums.OrderByDescending(n => n).Select((number, index) => (number, index)).ToList())
        {
            var newIndex = item.index >= count ? (item.index - count) * 2 : item.index * 2 + 1;
            nums[newIndex] = item.number;
        }
    }

    /**
        Given an integer array nums and an integer k, return the k-th largest element in the array.
        Note that it is the kth largest element in the sorted order, not the k-th distinct element.
        Solution https://leetcode.com/problems/kth-largest-element-in-an-array/solutions/6034604/c-non-recursion-solution
        # Approach
        # Complexity
            - Time complexity: O(n) (n + n/2 + n/4 + n/8 = 2n)
            - Space complexity: O(1)
     */
    public static int FindKthLargest_Partition(int[] nums, int k)
    {
        var first = 0;
        var last = nums.Length - 1;
        var kthLargestPosition = nums.Length - k;

        while (true)
        {
            var pivot = PartitionArray(nums, first, last);

            // If pivot index is equal to kthLargestPosition then we've found k-th largest element at k-th position from the end
            if (pivot == kthLargestPosition)
                return nums[pivot];

            // If pivot index is less than kthLargestPosition look up within the right subarray, otherwise within the left subarray
            if (pivot < kthLargestPosition)
                first = pivot + 1;
            else
                last = pivot - 1;
        }
    }
    
    /**
        Given an integer array nums, reorder it such that nums[0] less nums[1] greater nums[2] less nums[3]...
        You may assume the input array always has a valid answer.
        Solution https://leetcode.com/problems/wiggle-sort-ii/solutions/6036677/c-o-n-o-1-solution-partition-around-median-and-odd-even-indices
        # Approach
            1. Find the median and partition the array around the median by O(2n) time.
            2. The greater elements have to be placed as leftmost as possible. Their index `i1` is odd number. The smaller elements have to be placed as rightmost as possible. Their index `i2` is even number.
            3. Loop over the partitioned array and place greater and lesser elements according to their indices by O(n) time.
            Total time complexity is O(2n) + O(n) = O(3n) = O(n)

        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static void SortWiggle_ByPartition(int[] nums)
    {
        if (nums.Length < 2)
            return;

        // Odd index of numbers greater than median counting from the beginning 1 -> 3 -> 5 ->...
        var i1 = 1;
        // Even index of numbers less than median counting from the end ...-> 4 -> 2 -> 0
        var i2 = int.IsOddInteger(nums.Length) ? nums.Length - 1 : nums.Length - 2;

        var median = FindKthLargest_Partition(nums, nums.Length / 2);

        for (var i = 0; i < nums.Length;)
        {
            // Greater than median but is located on the wrong place. The index has to be odd instead of even.
            if (nums[i] > median && (int.IsEvenInteger(i) || i > i1))
            {
                (nums[i], nums[i1]) = (nums[i1], nums[i]);
                i1 += 2;
            }
            // Less than median but is located on the wrong place. The index has to be even instead of odd.
            else if (nums[i] < median && (int.IsOddInteger(i) || i < i2))
            {
                (nums[i], nums[i2]) = (nums[i2], nums[i]);
                i2 -= 2;
            }
            else
            {
                ++i;
            }
        }
    }
}