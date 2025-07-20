namespace CsArena.Tests.LeetCode;

using static Arrays;

public class ArraysTests
{
    [Fact]
    public void RemoveElementTest()
    {
        int[] nums = [3, 2, 2, 3];
        var remainingElements = RemoveElement(nums, 3);
        Assert.Equal(2, remainingElements);
        Assert.Equal([2, 2], nums.Take(remainingElements));

        nums = [0, 1, 2, 2, 3, 0, 4, 2];
        remainingElements = RemoveElement(nums, 2);
        Assert.Equal(5, remainingElements);
        Assert.Equal([0, 1, 3, 0, 4], nums.Take(remainingElements));

        nums = [0, 1, 2, 2, 3, 0, 4, 2];
        remainingElements = RemoveElement(nums, 5);
        Assert.Equal(8, remainingElements);
        Assert.Equal([0, 1, 2, 2, 3, 0, 4, 2], nums.Take(remainingElements));

        nums = [1, 1, 1, 1];
        remainingElements = RemoveElement(nums, 1);
        Assert.Equal(0, remainingElements);
        Assert.Equal([], nums.Take(remainingElements));

        nums = [1];
        remainingElements = RemoveElement(nums, 1);
        Assert.Equal(0, remainingElements);
        Assert.Equal([], nums.Take(remainingElements));

        nums = [4, 5];
        remainingElements = RemoveElement(nums, 5);
        Assert.Equal(1, remainingElements);
        Assert.Equal([4], nums.Take(remainingElements));

        nums = [2, 2, 3];
        remainingElements = RemoveElement(nums, 2);
        Assert.Equal(1, remainingElements);
        Assert.Equal([3], nums.Take(remainingElements));
    }

    [Fact]
    public void RemoveDuplicatesTest()
    {
        int[] nums = [2, 2, 3, 3];
        var remainingElements = RemoveDuplicates(nums);
        Assert.Equal(2, remainingElements);
        Assert.Equal([2, 3], nums.Take(remainingElements));

        nums = [0, 0, 1, 2, 2, 2, 3, 4];
        remainingElements = RemoveDuplicates(nums);
        Assert.Equal(5, remainingElements);
        Assert.Equal([0, 1, 2, 3, 4], nums.Take(remainingElements));

        nums = [1, 1, 1, 1];
        remainingElements = RemoveDuplicates(nums);
        Assert.Equal(1, remainingElements);
        Assert.Equal([1], nums.Take(remainingElements));

        nums = [1];
        remainingElements = RemoveDuplicates(nums);
        Assert.Equal(1, remainingElements);
        Assert.Equal([1], nums.Take(remainingElements));

        nums = [4, 5];
        remainingElements = RemoveDuplicates(nums);
        Assert.Equal(2, remainingElements);
        Assert.Equal([4, 5], nums.Take(remainingElements));

        nums = [2, 2, 3];
        remainingElements = RemoveDuplicates(nums);
        Assert.Equal(2, remainingElements);
        Assert.Equal([2, 3], nums.Take(remainingElements));

        nums = [];
        remainingElements = RemoveDuplicates(nums);
        Assert.Equal(0, remainingElements);
        Assert.Equal([], nums.Take(remainingElements));
    }

    [Fact]
    public void RemoveMoreThanTwoDuplicatesTest()
    {
        int[] nums = [1, 1, 1, 1, 1, 1, 2];
        var remainingElements = RemoveMoreThanKDuplicates(nums);
        Assert.Equal(3, remainingElements);
        Assert.Equal([1, 1, 2], nums.Take(remainingElements));

        nums = [1, 2, 3, 4, 5];
        remainingElements = RemoveMoreThanKDuplicates(nums);
        Assert.Equal(5, remainingElements);
        Assert.Equal([1, 2, 3, 4, 5], nums.Take(remainingElements));

        nums = [1, 1, 2, 2, 3, 4];
        remainingElements = RemoveMoreThanKDuplicates(nums);
        Assert.Equal(6, remainingElements);
        Assert.Equal([1, 1, 2, 2, 3, 4], nums.Take(remainingElements));

        nums = [1, 1, 1, 2, 2, 3];
        remainingElements = RemoveMoreThanKDuplicates(nums);
        Assert.Equal(5, remainingElements);
        Assert.Equal([1, 1, 2, 2, 3], nums.Take(remainingElements));

        nums = [0, 0, 1, 1, 1, 1, 2, 2, 2];
        remainingElements = RemoveMoreThanKDuplicates(nums);
        Assert.Equal(6, remainingElements);
        Assert.Equal([0, 0, 1, 1, 2, 2], nums.Take(remainingElements));

        nums = [];
        remainingElements = RemoveMoreThanKDuplicates(nums);
        Assert.Equal(0, remainingElements);
        Assert.Equal([], nums.Take(remainingElements));

        nums = [0];
        remainingElements = RemoveMoreThanKDuplicates(nums);
        Assert.Equal(1, remainingElements);
        Assert.Equal([0], nums.Take(remainingElements));

        nums = [0, 0];
        remainingElements = RemoveMoreThanKDuplicates(nums);
        Assert.Equal(2, remainingElements);
        Assert.Equal([0, 0], nums.Take(remainingElements));

        nums = [0, 0, 0];
        remainingElements = RemoveMoreThanKDuplicates(nums);
        Assert.Equal(2, remainingElements);
        Assert.Equal([0, 0], nums.Take(remainingElements));

        nums = [0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];
        remainingElements = RemoveMoreThanKDuplicates(nums);
        Assert.Equal(4, remainingElements);
        Assert.Equal([0, 0, 1, 1], nums.Take(remainingElements));
    }

    [Fact]
    public void UniqueIntersectionOfTwoArraysTest()
    {
        Assert.Equal([2], UniqueIntersection([1, 2, 2, 1], [2, 2]));
        Assert.Equal([9, 4], UniqueIntersection([4, 9, 5], [9, 4, 9, 8, 4]));
    }

    [Fact]
    public void FullIntersectionOfTwoArraysTest()
    {
        Assert.Equal([2, 2], FullIntersection([1, 2, 2, 1], [2, 2]));
        Assert.Equal([9, 4], FullIntersection([4, 9, 5], [9, 4, 9, 8, 4]));
    }

    [Fact]
    public void IntersectionOfThreeSortedArraysTest()
    {
        Assert.Equal([2, 2], IntersectionOfThreeSortedArrays([1, 1, 2, 2], [2, 2], [2, 2]));
        Assert.Equal([4, 9], IntersectionOfThreeSortedArrays([4, 5, 9], [4, 4, 8, 8, 9], [3, 4, 9]));
        Assert.Equal([5, 20], IntersectionOfThreeSortedArrays([1, 5, 10, 20, 30], [5, 13, 15, 20], [5, 20]));
        Assert.Equal([5], IntersectionOfThreeSortedArrays([2, 5, 10, 30], [5, 20, 34], [5, 13, 19]));
    }

    [Fact]
    public void IntersectionOfThreeArraysTest()
    {
        var res = IntersectionOfThreeArrays([1, 1, 2, 2], [2, 2], [2, 2]);
        Assert.Equal([2, 2], res.Full);
        Assert.Equal([2], res.Unique);

        res = IntersectionOfThreeArrays([5, 5, 20, 5], [5, 20, 5], [5, 13, 5]);
        Assert.Equal([5, 5], res.Full);
        Assert.Equal([5], res.Unique);
    }

    [Fact]
    public void TwoSumTest()
    {
        var res = TwoSumInArray_Dictionary([2, 7, 11, 15], 9);
        Assert.Equal([1, 0], res);

        res = TwoSumInArray_Dictionary([3, 2, 4], 6);
        Assert.Equal([2, 1], res);

        res = TwoSumInArray_Dictionary([3, 3], 6);
        Assert.Equal([1, 0], res);

        res = TwoSumInArray_Dictionary([3, 4, 3, 4], 8);
        Assert.Equal([3, 1], res);

        res = TwoSumInArray_Dictionary([3, 4, 5], 3);
        Assert.Equal([], res);

        res = TwoSumInArray_Dictionary([-5, -3, 0, 2, 4, 6, 8], 5);
        Assert.Equal([6, 1], res);
    }

    [Fact]
    public void TwoSumInSortedArrayTest()
    {
        var res = TwoSumInSortedArray_BinarySearch([2, 7, 11, 15], 9);
        Assert.Equal([1, 2], res);

        res = TwoSumInSortedArray_BinarySearch([2, 3, 4], 6);
        Assert.Equal([1, 3], res);

        res = TwoSumInSortedArray_BinarySearch([-1, 0], -1);
        Assert.Equal([1, 2], res);

        res = TwoSumInSortedArray_BinarySearch([3, 3], 6);
        Assert.Equal([1, 2], res);

        res = TwoSumInSortedArray_BinarySearch([3, 4, 3, 4], 8);
        Assert.Equal([2, 4], res);

        res = TwoSumInSortedArray_BinarySearch([3, 4, 5], 3);
        Assert.Equal([], res);

        res = TwoSumInSortedArray_BinarySearch([-5, -3, 0, 2, 4, 6, 8], 5);
        Assert.Equal([2, 7], res);

        res = TwoSumInSortedArray_TwoPointers([2, 7, 11, 15], 9);
        Assert.Equal([1, 2], res);

        res = TwoSumInSortedArray_TwoPointers([2, 3, 4], 6);
        Assert.Equal([1, 3], res);

        res = TwoSumInSortedArray_TwoPointers([-1, 0], -1);
        Assert.Equal([1, 2], res);

        res = TwoSumInSortedArray_TwoPointers([3, 3], 6);
        Assert.Equal([1, 2], res);

        res = TwoSumInSortedArray_TwoPointers([3, 4, 3, 4], 8);
        Assert.Equal([2, 4], res);

        res = TwoSumInSortedArray_TwoPointers([3, 4, 5], 3);
        Assert.Equal([], res);

        res = TwoSumInSortedArray_TwoPointers([-5, -3, 0, 2, 4, 6, 8], 5);
        Assert.Equal([2, 7], res);
    }

    [Fact]
    public void SubarraySumTest()
    {
        var res = SubarraySum([1, 1, 1], 2);
        Assert.Equal(2, res);

        res = SubarraySum([1, 2, 3], 3);
        Assert.Equal(2, res);

        res = SubarraySum([1, 2, 3, 4, 5], 9);
        Assert.Equal(2, res);

        res = SubarraySum([1, 1, 1, 3], 5);
        Assert.Equal(1, res);

        res = SubarraySum([1, -1, 1, 2, 3], 5);
        Assert.Equal(2, res);
    }

    [Fact]
    public void FindFirstSubarrayWithSumTest()
    {
        var res = FindFirstSubarrayWithSum([2, 3, 3, 4], 6);
        Assert.Equal((1, 2), res);

        res = FindFirstSubarrayWithSum([2, 3, 3, 4], 5);
        Assert.Equal((0, 1), res);

        res = FindFirstSubarrayWithSum([2, 3, 3, 4], 9);
        Assert.Equal((-1, -1), res);

        res = FindFirstSubarrayWithSum([2, 3, 3, 4], 12);
        Assert.Equal((0, 3), res);

        res = FindFirstSubarrayWithSum([2, 3, 0, 4, 1, 1], 6);
        Assert.Equal((3, 5), res);

        res = FindFirstSubarrayWithSum([2, 3, 0, 4, -1, 1], 6);
        Assert.Equal((1, 4), res);

        res = FindFirstSubarrayWithSum([4, 3, 3, 3, 3, 3], 6);
        Assert.Equal((1, 2), res);

        res = FindFirstSubarrayWithSum([4, 3, 1, 8, -1, 2, 1, -4], 6);
        Assert.Equal((3, 7), res);
    }

    [Fact]
    public void FindFirstSubarrayWithSumMultipleKTest()
    {
        Assert.True(FindFirstSubarrayWithSumMultipleK([23, 2, 4, 6, 7], 6));
        Assert.True(FindFirstSubarrayWithSumMultipleK([23, 2, 3, 6, 8], 6));
        Assert.False(FindFirstSubarrayWithSumMultipleK([23, 2, 6, 4, 7], 13));
        Assert.True(FindFirstSubarrayWithSumMultipleK([23, 2, 4, 6, 6], 7));
        Assert.False(FindFirstSubarrayWithSumMultipleK([0], 1));
        Assert.True(FindFirstSubarrayWithSumMultipleK([5, 0, 0, 0], 3));
    }

    [Fact]
    public void LongestConsecutiveTest()
    {
        Assert.Equal(4, LongestConsecutive([100, 4, 200, 1, 3, 2]));
        Assert.Equal(9, LongestConsecutive([0, 3, 7, 2, 5, 8, 4, 6, 0, 1]));
        Assert.Equal(7,
            LongestConsecutive([
                -3, 2, 8, 5, 1, 7, -8, 2, -8, -4, -1, 6, -6, 9, 6, 0, -7, 4, 5, -4, 8, 2, 0, -2, -6, 9, -4, -1
            ]));
        Assert.Equal(12,
            LongestConsecutive([-4, -1, 4, -5, 1, -6, 9, -6, 0, 2, 2, 7, 0, 9, -3, 8, 9, -2, -6, 5, 0, 3, 4, -2]));
        Assert.Equal(0, LongestConsecutive([]));
    }

    [Fact]
    public void LongestConsecutiveOnesTest()
    {
        Assert.Equal(3, LongestConsecutiveOnes([1, 0, 1, 1, 1, 0, 1, 1]));
        Assert.Equal(4, LongestConsecutiveOnes([1, 0, 1, 1, 1, 1]));
        Assert.Equal(0, LongestConsecutiveOnes([0, 0, 0]));
        Assert.Equal(0, LongestConsecutiveOnes([]));
        Assert.Equal(1, LongestConsecutiveOnes([1, 0, 0]));
    }

    [Fact]
    public void MergeSortedArraysInPlaceTest()
    {
        int[] nums = [1, 2, 3, 0, 0, 0];
        MergeSortedArraysInPlace(nums, 3, [2, 5, 6], 3);
        Assert.Equal([1, 2, 2, 3, 5, 6], nums);

        nums = [1, 2, 7, 9, 0, 0, 0];
        MergeSortedArraysInPlace(nums, 4, [5, 6, 8], 3);
        Assert.Equal([1, 2, 5, 6, 7, 8, 9], nums);

        nums = [1, 2, 0, 0];
        MergeSortedArraysInPlace(nums, 2, [5, 6], 2);
        Assert.Equal([1, 2, 5, 6], nums);

        nums = [5, 6, 0, 0];
        MergeSortedArraysInPlace(nums, 2, [1, 2], 2);
        Assert.Equal([1, 2, 5, 6], nums);

        nums = [1];
        MergeSortedArraysInPlace(nums, 1, [], 0);
        Assert.Equal([1], nums);

        nums = [0];
        MergeSortedArraysInPlace(nums, 0, [1], 1);
        Assert.Equal([1], nums);
    }

    [Fact]
    public void LongestSubarrayOfOnesTest()
    {
        Assert.Equal(2, LongestSubarrayOfOnes([1, 1, 1]));
        Assert.Equal(0, LongestSubarrayOfOnes([0, 0, 0]));
        Assert.Equal(1, LongestSubarrayOfOnes([1, 0, 0, 0, 0]));
        Assert.Equal(2, LongestSubarrayOfOnes([1, 1, 0, 0, 0]));
        Assert.Equal(4, LongestSubarrayOfOnes([1, 1, 0, 1, 1]));
        Assert.Equal(1, LongestSubarrayOfOnes([0, 0, 0, 0, 1]));
        Assert.Equal(2, LongestSubarrayOfOnes([0, 0, 0, 1, 1]));
        Assert.Equal(3, LongestSubarrayOfOnes([0, 0, 1, 1, 1]));
        Assert.Equal(4, LongestSubarrayOfOnes([0, 1, 1, 1, 1]));
        Assert.Equal(4, LongestSubarrayOfOnes([1, 1, 1, 1, 1]));

        Assert.Equal(3, LongestSubarrayOfOnes([1, 1, 0, 1]));
        Assert.Equal(5, LongestSubarrayOfOnes([0, 1, 1, 1, 0, 1, 1, 0, 1]));
        Assert.Equal(11, LongestSubarrayOfOnes([1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1]));
        Assert.Equal(4, LongestSubarrayOfOnes([1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1]));
    }

    [Fact]
    public void RunningSumTest()
    {
        Assert.Equal([1, 3, 6, 10], RunningSum([1, 2, 3, 4]));
        Assert.Equal([1, 2, 3, 4, 5], RunningSum([1, 1, 1, 1, 1]));
        Assert.Equal([3, 4, 6, 16, 17], RunningSum([3, 1, 2, 10, 1]));
    }

    [Fact]
    public void StrictlyIncreasingAfterOneElementRemovalTest()
    {
        Assert.True(StrictlyIncreasingAfterOneElementRemoval([5]));
        Assert.True(StrictlyIncreasingAfterOneElementRemoval([5, 6]));
        Assert.True(StrictlyIncreasingAfterOneElementRemoval([6, 5]));
        Assert.True(StrictlyIncreasingAfterOneElementRemoval([1, 2, 10, 5, 7]));
        Assert.True(StrictlyIncreasingAfterOneElementRemoval([4, 5, 3, 6]));
        Assert.False(StrictlyIncreasingAfterOneElementRemoval([2, 3, 1, 2]));
        Assert.False(StrictlyIncreasingAfterOneElementRemoval([1, 1, 1]));
        Assert.False(StrictlyIncreasingAfterOneElementRemoval([94, 186, 565, 599, 34, 818, 882, 872]));
    }

    [Fact]
    public void MaxSubArrayTest()
    {
        Assert.Equal(5, MaxSubArray([1, -3, 5]));
        Assert.Equal(3, MaxSubArray([1, -2, 3, -2]));
        Assert.Equal(+7, MaxSubArray([5, -3, 5]));
        Assert.Equal(-2, MaxSubArray([-3, -2, -3]));
        Assert.Equal(6, MaxSubArray([-2, 1, -3, 4, -1, 2, 1, -5, 4]));
        Assert.Equal(1, MaxSubArray([1]));
        Assert.Equal(23, MaxSubArray([5, 4, -1, 7, 8]));
        Assert.Equal(5, MaxSubArray([5, -6, 0, 4]));
        Assert.Equal(-8, MaxSubArray([-8, -8, -8, -8]));
        Assert.Equal(-7, MaxSubArray([-8, -8, -8, -7]));
        Assert.Equal(-7, MaxSubArray([-7, -8, -8, -8]));
        Assert.Equal(+7, MaxSubArray([5, -3, 5]));
    }

    [Fact]
    public void MaxSubArrayReturnedTest()
    {
        Assert.Equal([5], MaxSubArrayReturned([1, -3, 5]));
        Assert.Equal([3], MaxSubArrayReturned([1, -2, 3, -2]));
        Assert.Equal([5, -3, 5], MaxSubArrayReturned([5, -3, 5]));
        Assert.Equal([-2], MaxSubArrayReturned([-3, -2, -3]));
        Assert.Equal([4, -1, 2, 1], MaxSubArrayReturned([-2, 1, -3, 4, -1, 2, 1, -5, 4]));
        Assert.Equal([1], MaxSubArrayReturned([1]));
        Assert.Equal([5, 4, -1, 7, 8], MaxSubArrayReturned([5, 4, -1, 7, 8]));
        Assert.Equal([5], MaxSubArrayReturned([5, -6, 0, 4]));
        Assert.Equal([-8], MaxSubArrayReturned([-8, -8, -8, -8]));
        Assert.Equal([-7], MaxSubArrayReturned([-8, -8, -8, -7]));
        Assert.Equal([-7], MaxSubArrayReturned([-7, -8, -8, -8]));
        Assert.Equal([5, -3, 5], MaxSubArrayReturned([5, -3, 5]));
    }


    [Fact]
    public void MaxSubarraySumCircularTest()
    {
        Assert.Equal(6, MaxSubarraySumCircular([1, -3, 5]));
        Assert.Equal(3, MaxSubarraySumCircular([1, -2, 3, -2]));
        Assert.Equal(10, MaxSubarraySumCircular([5, -3, 5]));
        Assert.Equal(-2, MaxSubarraySumCircular([-3, -2, -3]));
        Assert.Equal(6, MaxSubarraySumCircular([-2, 1, -3, 4, -1, 2, 1, -5, 4]));
        Assert.Equal(1, MaxSubarraySumCircular([1]));
        Assert.Equal(24, MaxSubarraySumCircular([5, 4, -1, 7, 8]));
        Assert.Equal(9, MaxSubarraySumCircular([5, -6, 0, 4]));
        Assert.Equal(-8, MaxSubarraySumCircular([-8, -8, -8, -8]));
        Assert.Equal(-7, MaxSubarraySumCircular([-8, -8, -8, -7]));
        Assert.Equal(-7, MaxSubarraySumCircular([-7, -8, -8, -8]));
    }

    [Fact]
    public void MajorityElementTest()
    {
        Assert.Equal(3, MajorityElement([3, 2, 3]));
        Assert.Equal(2, MajorityElement([2, 2, 1, 1, 1, 2, 2]));
        Assert.Equal(7, MajorityElement([8, 8, 7, 7, 7]));
    }

    [Fact]
    public void RotateTest()
    {
        int[] arr = [1, 2, 3, 4, 5, 6, 7];
        Rotate(arr, 3);
        Assert.Equal([5, 6, 7, 1, 2, 3, 4], arr);

        arr = [1, 2, 3, 4, 5, 6, 7];
        Rotate(arr, 1);
        Assert.Equal([7, 1, 2, 3, 4, 5, 6], arr);

        arr = [1, 2, 3, 4, 5, 6, 7];
        Rotate(arr, 2);
        Assert.Equal([6, 7, 1, 2, 3, 4, 5], arr);

        arr = [1, 2, 3, 4, 5, 6, 7];
        Rotate(arr, 2);
        Assert.Equal([6, 7, 1, 2, 3, 4, 5], arr);

        arr = [-1, -100, 3, 99];
        Rotate(arr, 1);
        Assert.Equal([99, -1, -100, 3], arr);

        arr = [-1, -100, 3, 99];
        Rotate(arr, 2);
        Assert.Equal([3, 99, -1, -100], arr);
    }
    
    [Fact]
    public void ContainsDuplicateTest()
    {
        Assert.True(ContainsDuplicate([1, 2, 3, 1]));
        Assert.False(ContainsDuplicate([1, 2, 3, 4]));
        Assert.True(ContainsDuplicate([1, 1, 1, 3, 3, 4, 3, 2, 4, 2]));

        Assert.True(ContainsDuplicate_Linq([1, 2, 3, 1]));
        Assert.False(ContainsDuplicate_Linq([1, 2, 3, 4]));
        Assert.True(ContainsDuplicate_Linq([1, 1, 1, 3, 3, 4, 3, 2, 4, 2]));
    }

    [Fact]
    public void ContainsNearbyDuplicateTest()
    {
        Assert.True(ContainsNearbyDuplicate([1, 2, 3, 1], 3));
        Assert.True(ContainsNearbyDuplicate([1, 0, 1, 1], 1));
        Assert.False(ContainsNearbyDuplicate([1, 2, 3, 1, 2, 3], 2));
    }

    [Fact]
    public void ContainsNearbyAlmostDuplicateTest()
    {
        Assert.True(ContainsNearbyAlmostDuplicate([8, 7, 15, 1, 6, 1, 9, 15], indexDiff: 1, valueDiff: 3));
        Assert.True(ContainsNearbyAlmostDuplicate([1, 3, 6, 2], indexDiff: 1, valueDiff: 2));
        Assert.True(ContainsNearbyAlmostDuplicate([1, 2, 3, 1], indexDiff: 3, valueDiff: 0));
        Assert.False(ContainsNearbyAlmostDuplicate([1, 5, 9, 1, 5, 9], indexDiff: 2, valueDiff: 3));
        Assert.True(ContainsNearbyAlmostDuplicate([1, 2, 1, 1], indexDiff: 1, valueDiff: 0));
        Assert.False(ContainsNearbyAlmostDuplicate([1, 3, 5, 7, 9, 11], indexDiff: 3, valueDiff: 1));
    }

    [Fact]
    public void MinSubArrayLenTest()
    {
        Assert.Equal(2, MinSubArrayLen_Queue(7, [2, 3, 1, 2, 4, 3]));
        Assert.Equal(1, MinSubArrayLen_Queue(4, [1, 4, 4]));
        Assert.Equal(0, MinSubArrayLen_Queue(11, [1, 1, 1, 1, 1, 1, 1, 1]));
        Assert.Equal(1, MinSubArrayLen_Queue(15, [1, 2, 3, 15, 4, 3]));
        Assert.Equal(0, MinSubArrayLen_Queue(15, []));

        Assert.Equal(2, MinSubArrayLen_TwoPointers(7, [2, 3, 1, 2, 4, 3]));
        Assert.Equal(1, MinSubArrayLen_TwoPointers(4, [1, 4, 4]));
        Assert.Equal(0, MinSubArrayLen_TwoPointers(11, [1, 1, 1, 1, 1, 1, 1, 1]));
        Assert.Equal(1, MinSubArrayLen_TwoPointers(15, [1, 2, 3, 15, 4, 3]));
        Assert.Equal(0, MinSubArrayLen_TwoPointers(15, []));
    }
}