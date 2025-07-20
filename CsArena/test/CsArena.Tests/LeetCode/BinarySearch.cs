namespace CsArena.Tests.LeetCode;

public static class BinarySearch
{
    /**
        Given an array of integers nums sorted in non-decreasing order, find the starting and ending position of a given target value.
        If target is not found in the array, return [-1, -1].
        You must write an algorithm with O(log n) runtime complexity.
        Solution https://leetcode.com/problems/find-first-and-last-position-of-element-in-sorted-array/solutions/6234782/c-arraybinarysearch-beats-100-by-victorn-k8lx
        # Approach
            Call standard `Array.BinarySearch()` then go to the left and to the right.
        # Complexity
            - Time complexity: O(log(n) + n)
            - Space complexity: O(1)
    */
    public static int[] FindFirstAndLastPosition_ArrayBinarySearch(int[] nums, int target)
    {
        // C# BinarySearch() can return any of indices if there are several equal elements in a row
        var j = Array.BinarySearch(nums, target);
        if (j < 0)
            return [-1, -1];

        var i = j;
        while (i > 0 && nums[i] == nums[i - 1])
            i--;
        while (j < nums.Length - 1 && nums[j] == nums[j + 1])
            j++;
        return [i, j];
    }

    /**
        Given an array of integers nums sorted in non-decreasing order, find the starting and ending position of a given target value.
        If target is not found in the array, return [-1, -1].
        You must write an algorithm with O(log n) runtime complexity.
        Solution https://leetcode.com/problems/find-first-and-last-position-of-element-in-sorted-array/solutions/6235075/c-logn-optimized-solution-beats-100-by-v-tdpu
        # Approach
            1. We look for the first occurence by binary search and return its index.
            2. We look for the last occurence by binary search starting from the first occurence index.
            P.S. I have a steady feeling that both the first and the last occurences can be found during one call within one `while` loop.            
        # Complexity
            - Time complexity: O(log(n))
            - Space complexity: O(1)
    */
    public static int[] FindFirstAndLastPosition(int[] nums, int target)
    {
        var first = FindFirst(nums, target, 0, nums.Length - 1);
        if (first < 0)
            return [-1, -1];
        return [first, FindLast(nums, target, first, nums.Length - 1)];
    }

    public static int FindFirst(int[] nums, int target, int first, int last)
    {
        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (nums[mid] == target && (mid == 0 || nums[mid - 1] < target))
                return mid;

            if (target > nums[mid])
                first = mid + 1;
            else
                last = mid - 1;
        }
        return -1;
    }

    public static int FindLast(int[] nums, int target, int first, int last)
    {
        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (nums[mid] == target && (mid == nums.Length - 1 || nums[mid + 1] > target))
                return mid;

            if (target >= nums[mid])
                first = mid + 1;
            else
                last = mid - 1;
        }
        return -1;
    }

    /**
        Given a sorted array of distinct integers and a target value, return the index if the target is found.
        If not, return the index where it would be if it were inserted in order.
        You must write an algorithm with O(log(n)) runtime complexity.
        Solution https://leetcode.com/problems/search-insert-position/solutions/6239253/c-solution-beats-100-by-victornovik-jfdi
        # Complexity
            - Time complexity: O(log(n))
            - Space complexity: O(1)
    */
    public static int SearchInsertPosition(int[] nums, int target)
    {
        int first = 0, last = nums.Length - 1;
        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (nums[mid] == target)
                return mid;

            if (target > nums[mid])
                first = mid + 1;
            else
                last = mid - 1;
        }
        return first;
    }

    /**
        Solution https://leetcode.com/problems/search-insert-position/solutions/6235276/c-solution-beats-100-by-victornovik-5d77
        # Complexity
            - Time complexity: O(log(n))
            - Space complexity: O(1)
    */
    public static int SearchInsertPosition_ArrayBinarySearch(int[] nums, int target)
    {
        var index = Array.BinarySearch(nums, target);
        return index < 0 ? ~index : index;
    }

    /**
        You are a product manager and currently leading a team to develop a new product. 
        Unfortunately, the latest version of your product fails the quality check. 
        Since each version is developed based on the previous version, all the versions after a bad version are also bad.
        Suppose you have n versions [1, 2, ..., n] and you want to find out the first bad one, which causes all the following ones to be bad.
        You are given an API bool isBadVersion(version) which returns whether version is bad. 
        Implement a function to find the first bad version. 
        You should minimize the number of calls to the API.
        Solution https://leetcode.com/problems/first-bad-version/solutions/6239808/c-solution-by-victornovik-nv3s
        # Complexity
            - Time complexity: O(log(n))
            - Space complexity: O(1)
     */
    public static int FirstBadVersion(int n, Func<int, bool> IsBadVersion)
    {
        int first = 1, last = n;
        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (!IsBadVersion(mid))
                first = mid + 1;
            else
                last = mid - 1;
        }
        return first;
    }

    /**
        A conveyor belt has packages that must be shipped from one port to another within `days` days.
        The i-th package on the conveyor belt has a weight of weights[i]. 
        Each day, we load the ship with packages on the conveyor belt (in the order given by weights). 
        We may not load more weight than the maximum weight capacity of the ship.
        Return the least weight capacity of the ship that will result in all the packages on the conveyor belt being shipped within exactly `days` days.
        Solution https://leetcode.com/problems/capacity-to-ship-packages-within-d-days/solutions/6243298/c-solution-by-victornovik-wysv
        # Complexity
            - Time complexity: O(log(∑(weights)) * n)
            - Space complexity: O(1)
    */
    public static int ShipWithinDays(int[] weights, int days)
    {
        var first = weights.Max(); // Least ship capacity to carry one maximal package
        var last = weights.Sum(); // Most ship capacity to carry all packages in 1 day
        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (IsGoodCapacity(mid))
                last = mid - 1;
            else
                first = mid + 1;
        }
        return first;

        bool IsGoodCapacity(int capacity)
        {
            var totalDays = 1;
            var totalWeight = 0;
            foreach (var weight in weights)
            {
                totalWeight += weight;
                if (totalWeight > capacity)
                {
                    // Move the exceeding weight to the next day.
                    totalWeight = weight;
                    // Cannot ship for `days` days. Increase the capacity.
                    if (++totalDays > days)
                        return false;
                }
            }
            return true;
        }
    }

    /**
        Given an non-negative integer array `nums` and an integer `k`, split nums into k non-empty contiguous subarrays.
        Write an algorithm to minimize the largest sum among these `k` subarrays.
        Return the minimized largest sum of the split.
        Solution https://leetcode.com/problems/split-array-largest-sum/solutions/6243403/c-solution-by-victornovik-vmtd
        # Complexity
            - Time complexity: O(log(∑(nums)) * n)
            - Space complexity: O(1)
    */
    public static int SplitArray(int[] nums, int k)
    {
        var first = nums.Max(); // Least subarray sum
        var last = nums.Sum(); // Most subarray sum
        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (IsGoodSplit(mid))
                last = mid - 1;
            else
                first = mid + 1;
        }
        return first;

        bool IsGoodSplit(int sum)
        {
            var totalSubarrays = 1;
            var totalSum = 0;
            foreach (var n in nums)
            {
                totalSum += n;
                if (totalSum > sum)
                {
                    totalSum = n;
                    if (++totalSubarrays > k)
                        return false;
                }
            }
            return true;
        }
    }

    /**
        Koko loves to eat bananas. There are n piles of bananas, the i-th pile has piles[i] bananas. 
        The guards have gone and will come back in h hours.
        Koko can decide her bananas-per-hour eating speed of `k`. 
        Each hour, she chooses some pile of bananas and eats `k` bananas from that pile. 
        If the pile has less than `k` bananas, she eats all of them instead and will not eat any more bananas during this hour.
        Koko likes to eat slowly but still wants to finish eating all the bananas before the guards return.
        Return the minimum integer `k` such that she can eat all the bananas within h hours.
        Solution https://leetcode.com/problems/koko-eating-bananas/solutions/6244200/c-solutuion-by-victornovik-p5ns
        # Complexity
            - Time complexity: O(log(Max(piles)) * n)
            - Space complexity: O(1)
    */
    public static int MinEatingSpeed(int[] piles, int h)
    {
        var first = 1; // Least eating velocity
        var last = piles.Max(); // Most eating velocity

        if (piles.Length == h)
            return last;

        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (CanEatAllBananas(mid))
                last = mid - 1;
            else
                first = mid + 1;
        }
        return first;

        bool CanEatAllBananas(int velocity)
        {
            var hours = 0;
            foreach (var pile in piles)
            {
                hours += (pile - 1) / velocity + 1; // Faster than Math.Ceiling((double)pile/velocity) and without conversions
                if (hours > h)
                    return false;
            }
            return true;
        }
    }

    /**
        You are given an integer array `bloomDay`, an integer m and an integer k.
        You want to make `m` bouquets. 
        To make a bouquet, you need to use `k` adjacent flowers from the garden.
        The garden consists of `n` flowers, the i-th flower will bloom in the bloomDay[i] and then can be used in exactly one bouquet.
        Return the minimum number of days you need to wait to be able to make `m` bouquets from the garden. 
        If it is impossible to make `m` bouquets return -1.
        Solution https://leetcode.com/problems/minimum-number-of-days-to-make-m-bouquets/solutions/6248504/c-solution-by-victornovik-ybmn
        # Complexity   
            - Time complexity: O(log(Max(bloomDay)) * n)
            - Space complexity: O(1)
    */
    public static int MinBloomingDays(int[] bloomDay, int m, int k)
    {
        // We cannot gather more flowers than there are flowers in the garden
        if ((long)m * k > bloomDay.Length)
            return -1;

        var first = 1; // Least blooming days
        var last = bloomDay.Max(); // Most blooming days

        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (CanGatherBouquets(mid))
                last = mid - 1;
            else
                first = mid + 1;
        }
        return first;

        bool CanGatherBouquets(int days)
        {
            var bouquets = 0;
            var adjacentFlowers = 0;
            foreach (var day in bloomDay)
            {
                if (day <= days)
                {
                    // Not enough `k` flowers for one bouquet
                    if (++adjacentFlowers % k != 0)
                        continue;

                    // Early exit. We can return immediately when we've gathered `m` bouquets
                    if (++bouquets == m)
                        return true;
                }
                else
                {
                    adjacentFlowers = 0;
                }
            }
            return bouquets >= m;
        }
    }

    /**
        Nearly everyone has used the Multiplication Table https://en.wikipedia.org/wiki/Multiplication_table. 
        The multiplication table of size `rows` x `cols` is an integer matrix `mat` where mat[i][j] == i * j (1-indexed).
        Given three integers `rows`, `cols`, and `k`, return the k-th smallest element in the rows x cols multiplication table.
        Solution https://leetcode.com/problems/kth-smallest-number-in-multiplication-table/solutions/6249563/optimized-c-solution-beats-85-by-victorn-m26c
        # Approach
            The hardest part of this bisect solution is to implement fast `HasSmallerThan()` function.
            We can calculate very fast how many **row** elements are less than `val` by integer-dividing `val` by `row` index and picking the minimum among that result and the number of columns `col`.
            E.g. let's consider 3x3 multiplication table and calculate how many elements less than 5 in each row.
                **Row #1**: `Min(5/1, 3)` = 3 elements
                **Row #2**: `Min(5/2, 3)` = 2 elements
                **Row #3**: `Min(5/3, 3)` = 1 elements
                Totally we've got 3 + 2 + 1 = 6 elements less than 5 in the matrix. 
                This algo takes $$O(rows)$$ complexity though I use several checks in order to exit the per-row loop as earlier as possible.
                P.S. I believe there is a room for improvement and that complexity can be reduced till $$O(log)$$ however. Maybe by applying another inner bisect algo 😉
        # Complexity
            - Time complexity: O(log(rows * cols) * rows)
            - Space complexity: O(1)
    */
    public static int FindKthNumberOfMultiplicationTable(int rows, int cols, int k)
    {
        var first = 1; // Least multiplication table element
        var last = rows * cols; // Most multiplication table element

        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (HasSmallerThan(mid))
                last = mid - 1;
            else
                first = mid + 1;
        }
        return first;

        bool HasSmallerThan(int val)
        {
            var tableElementsLessThanVal = 0;
            for (var i = 1; i <= rows; i++)
            {
                var rowElementsLessThanVal = int.Min(val / i, cols);

                // We no longer have greater elements in the next rows
                if (rowElementsLessThanVal == 0)
                    break;

                tableElementsLessThanVal += rowElementsLessThanVal;

                // Early exit. We no longer have to analyze the next rows if we've already found `k` elements less than `val`
                if (tableElementsLessThanVal >= k)
                    return true;
            }
            return tableElementsLessThanVal >= k;
        }
    }

    /**
        The distance of a pair of integers a and b is defined as the absolute difference between a and b.
        Given an integer array `nums` and an integer `k`, return the k-th smallest distance among all the pairs 
        nums[i] and nums[j] where 0 less or equal `i` less `j` less `nums.length`.
        Solution https://leetcode.com/problems/find-k-th-smallest-pair-distance/solutions/6251321/optimized-c-solution-beats-85-by-victorn-y4fo
        # Complexity
            - Time complexity: O(n*log(n) + n*log(max(nums) - min(nums)))
            - Space complexity: O(1)
    */
    public static int SmallestDistancePair(int[] nums, int k)
    {
        Array.Sort(nums);

        var first = 0; // Least distance
        var last = nums[^1] - nums[0]; // Most distance

        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (HasSmallerThan(mid))
                last = mid - 1;
            else
                first = mid + 1;
        }
        return first;

        bool HasSmallerThan(int dist)
        {
            var pairsLessThanDist = 0;

            //TODO Generate combinations C (nums.Length by 2) and calculate how many of them have distance >= k
            for (int i = 0, j = 1; i < nums.Length; i++)
            {
                while (j < nums.Length && nums[j] - nums[i] <= dist)
                    j++;

                pairsLessThanDist += j - i - 1;

                // Early exit. We no longer have to analyze the next pairs if we've already found `k` pairs less than `dist`
                if (pairsLessThanDist >= k)
                    return true;
            }

            return pairsLessThanDist >= k;
        }
    }

    /**
        An ugly number is a positive integer that is divisible by either `a` or `b` or c.
        Given four integers n, a, b, and c, return the n-th ugly number.
        Solution https://leetcode.com/problems/ugly-number-iii/solutions/6265100/c-solution-beats-100-by-victornovik-oh93
        # Complexity
            - Time complexity: O(log(10^10))
            - Space complexity: O(1)
    */
    public static int NthUglyNumber(long n, long a, long b, long c)
    {
        var first = 1L; // Least positive integer
        var last = 10_000_000_000L; // Most positive integer 10^10
        var ab = Maths.Lcm(a, b); // Least common multiple of a and b to exclude it from total sum
        var ac = Maths.Lcm(a, c); // Least common multiple of a and c to exclude it from total sum
        var bc = Maths.Lcm(b, c); // Least common multiple of b and c to exclude it from total sum
        var abc = Maths.Lcm(a, bc);// Least common multiple of a, b and c to include it to total sum

        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (IsDivisibleByAbcMoreThan(mid))
                last = mid - 1;
            else
                first = mid + 1;
        }
        return (int)first;

        bool IsDivisibleByAbcMoreThan(long i)
        {
            var totalDivisibleNums = i / a + i / b + i / c - i / ab - i / ac - i / bc + i / abc;
            return totalDivisibleNums >= n;
        }
    }

    /**
        Given an array of integers `nums` and an integer `threshold`, we will choose a positive integer divisor, divide all the array by it, and sum the division's result. 
        Find the smallest divisor such that the result mentioned above is less than or equal to `threshold`.
        Each result of the division is rounded to the nearest integer greater than or equal to that element. 
        For example: 7/3 = 3 and 10/2 = 5.
        Solution https://leetcode.com/problems/find-the-smallest-divisor-given-a-threshold/solutions/6265235/c-solution-by-victornovik-ux8a
        # Complexity
            - Time complexity: O(log(Max(nums)) * n)
            - Space complexity: O(1)
    */
    public static int SmallestDivisor(int[] nums, int threshold)
    {
        var first = 1; // Least positive integer divisor
        var last = nums.Max(); // Most positive integer divisor

        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            if (IsSumOfDivLessThanThreshold(mid))
                last = mid - 1;
            else
                first = mid + 1;
        }
        return first;

        bool IsSumOfDivLessThanThreshold(int divisor)
        {
            // Pattern `(i - 1) / divisor + 1` is Ceiling for integers
            var sum = nums.Sum(i => (i - 1) / divisor + 1);
            return sum <= threshold;
        }
    }
}