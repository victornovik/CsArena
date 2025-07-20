namespace CsArena.Tests.LeetCode;

using static Sort;

public class SortTests
{
    [Fact]
    public void SortColorsTest()
    {
        int[] arr = [2, 0, 2, 1, 1, 0];
        SortColors(arr);
        Assert.Equal([0, 0, 1, 1, 2, 2], arr);

        arr = [2, 0, 1];
        SortColors(arr);
        Assert.Equal([0, 1, 2], arr);
    }

    [Fact]
    public void RadixSortTest()
    {
        Assert.Equal([213, 231, 322, 330], RadixSortArray_Bitwise([322, 231, 330, 213]));
        Assert.Equal([-1, 11, 111, 901], RadixSortArray_Bitwise([111, 901, 11, -1]));

        Assert.Equal([213, 231, 322, 330], RadixSortArray_Counting([322, 231, 330, 213]));
        Assert.Equal([1, 11, 111, 901], RadixSortArray_Counting([111, 901, 11, 1]));

        Assert.Equal([213, 231, 322, 330], RadixSortArray_Buckets([322, 231, 330, 213]));
        Assert.Equal([1, 11, 111, 901], RadixSortArray_Buckets([111, 901, 11, 1]));
    }

    [Fact]
    public void QuickSortArrayTest()
    {
        Assert.Equal([1, 2, 3, 5], QuickSortArray([5, 2, 3, 1]));
        Assert.Equal([0, 0, 1, 1, 2, 5], QuickSortArray([5, 1, 1, 2, 0, 0]));
        Assert.Equal([0, 100, 110], QuickSortArray([110, 100, 0]));
    }

    [Fact]
    public void PartitionArrayWithPredTest()
    {
        int[] arr = [1, 2, 4, 3, 5];
        PartitionArray(arr, e => e < 3);
        Assert.Equal([1, 2, 4, 3, 5], arr);

        arr = [1, 4, 3, 2, 5, 2];
        PartitionArray(arr, e => e < 3);
        Assert.Equal([1, 2, 2, 4, 5, 3], arr);

        arr = [2, 1];
        PartitionArray(arr, e => e < 2);
        Assert.Equal([1, 2], arr);

        arr = [1, 30, -4, 3, 5, -4, 1, 6, -8, 2, -5, 64, 1, 92];
        PartitionArray(arr, e => e < 3);
        Assert.Equal([1, -4, -4, 1, -8, 2, -5, 1, 5, 30, 3, 64, 6, 92], arr);
    }

    [Fact]
    public void PartitionArrayWithPivotTest()
    {
        int[] arr = [3, 4, 5, 2, 1];
        var pivotIndex = PartitionArray(arr, 0, arr.Length - 1);
        Assert.Equal(3, arr[pivotIndex]);
        Assert.Equal([1, 2, 3, 4, 5], arr);

        arr = [1, 4, 3, 2, 5, 2];
        pivotIndex = PartitionArray(arr, 0, arr.Length - 1);
        Assert.Equal(1, arr[pivotIndex]);
        Assert.Equal([1, 4, 3, 2, 5, 2], arr);

        arr = [2, 1];
        pivotIndex = PartitionArray(arr, 0, arr.Length - 1);
        Assert.Equal(2, arr[pivotIndex]);
        Assert.Equal([1, 2], arr);

        arr = [1, 30, -4, 3, 5, -4, 1, 6, -8, 2, -5, 64, 1, 92];
        pivotIndex = PartitionArray(arr, 0, arr.Length - 1);
        Assert.Equal(1, arr[pivotIndex]);
        Assert.Equal([-5, -4, -4, -8, 1, 30, 1, 6, 3, 2, 5, 64, 1, 92], arr);
    }

    [Fact]
    public void PartitionArrayWithPivotDescTest()
    {
        int[] arr = [3, 4, 5, 2, 1];
        var pivotIndex = PartitionArray(arr, 0, arr.Length - 1, ascending: false);
        Assert.Equal(3, arr[pivotIndex]);
        Assert.Equal([5, 4, 3, 2, 1], arr);

        arr = [1, 4, 3, 2, 5, 2];
        pivotIndex = PartitionArray(arr, 0, arr.Length - 1, ascending: false);
        Assert.Equal(1, arr[pivotIndex]);
        Assert.Equal([2, 4, 3, 2, 5, 1], arr);

        arr = [2, 1];
        pivotIndex = PartitionArray(arr, 0, arr.Length - 1, ascending: false);
        Assert.Equal(2, arr[pivotIndex]);
        Assert.Equal([2, 1], arr);

        arr = [1, 30, -4, 3, 5, -4, 1, 6, -8, 2, -5, 64, 1, 92];
        pivotIndex = PartitionArray(arr, 0, arr.Length - 1, ascending: false);
        Assert.Equal(1, arr[pivotIndex]);
        Assert.Equal([92, 30, 3, 5, 6, 2, 64, 1, -8, -4, -5, 1, 1, -4], arr);
    }

    [Fact]
    public void MergeSortArrayTest()
    {
        Assert.Equal([1, 2, 3, 5], MergeSortArray([5, 2, 3, 1]));
        Assert.Equal([0, 0, 1, 1, 2, 5], MergeSortArray([5, 1, 1, 2, 0, 0]));
        Assert.Equal([0, 100, 110], MergeSortArray([110, 100, 0]));
    }

    [Fact]
    public void RelativeSortArrayTest()
    {
        Assert.Equal([2, 2, 2, 1, 4, 3, 3, 9, 6, 7, 19], RelativeSortArray([2, 3, 1, 3, 2, 4, 6, 7, 9, 2, 19], [2, 1, 4, 3, 9, 6]));
        Assert.Equal([22, 28, 8, 6, 17, 44], RelativeSortArray([28, 6, 22, 8, 44, 17], [22, 28, 8, 6]));

        Assert.Equal([2, 2, 2, 1, 4, 3, 3, 9, 6, 7, 19], RelativeSortArray_Linq1([2, 3, 1, 3, 2, 4, 6, 7, 9, 2, 19], [2, 1, 4, 3, 9, 6]));
        Assert.Equal([22, 28, 8, 6, 17, 44], RelativeSortArray_Linq1([28, 6, 22, 8, 44, 17], [22, 28, 8, 6]));

        Assert.Equal([2, 2, 2, 1, 4, 3, 3, 9, 6, 7, 19], RelativeSortArray_Linq2([2, 3, 1, 3, 2, 4, 6, 7, 9, 2, 19], [2, 1, 4, 3, 9, 6]));
        Assert.Equal([22, 28, 8, 6, 17, 44], RelativeSortArray_Linq2([28, 6, 22, 8, 44, 17], [22, 28, 8, 6]));
    }

    [Fact]
    public void CustomSortStringTest()
    {
        Assert.Equal("cbad", CustomSortString(order: "cba", s: "abcd"));
        Assert.Equal("bcad", CustomSortString(order: "bcafg", s: "abcd"));
    }

    [Fact]
    public void SortPeopleTest()
    {
        Assert.Equal(["Mary", "Emma", "John"], SortPeople(["Mary", "John", "Emma"], heights: [180, 165, 170]));
        Assert.Equal(["Bob", "Alice", "Bob"], SortPeople(["Alice", "Bob", "Bob"], heights: [155, 185, 150]));
    }

    [Fact]
    public void SortByFrequencyTest()
    {
        Assert.Equal("eetr", SortByFrequency("tree"));
        Assert.Equal("cccaaa", SortByFrequency("cccaaa"));
        Assert.Equal("bbAa", SortByFrequency("Aabb"));
    }

    [Fact]
    public void SortWiggle_BySortTest()
    {
        int[] nums = [4, 5, 5, 6];
        SortWiggle_BySort(nums);
        Assert.Equal([5, 6, 4, 5], nums);

        nums = [1, 4, 3, 4, 1, 2, 1, 3, 1, 3, 2, 3, 3];
        SortWiggle_BySort(nums);
        Assert.Equal([3, 4, 2, 4, 2, 3, 1, 3, 1, 3, 1, 3, 1], nums);

        nums = [1, 1, 2, 1, 2, 2, 1];
        SortWiggle_BySort(nums);
        Assert.Equal([1, 2, 1, 2, 1, 2, 1], nums);

        nums = [1, 5, 1, 1, 6, 4];
        SortWiggle_BySort(nums);
        Assert.Equal([1, 6, 1, 5, 1, 4], nums);

        nums = [1, 3, 2, 2, 3, 1];
        SortWiggle_BySort(nums);
        Assert.Equal([2, 3, 1, 3, 1, 2], nums);

        nums = [2, 3, 5, 5, 5, 6];
        SortWiggle_BySort(nums);
        Assert.Equal([5, 6, 3, 5, 2, 5], nums);
    }

    [Fact]
    public void FindKthLargestTest()
    {
        Assert.Equal(5, FindKthLargest_Partition([3, 2, 1, 5, 6, 4], 2));
        Assert.Equal(4, FindKthLargest_Partition([3, 2, 3, 1, 2, 4, 5, 5, 6], 4));
    }

    [Fact]
    public void SortWiggle_ByPartitionTest()
    {
        int[] nums = [6, 5, 5, 4];
        SortWiggle_ByPartition(nums);
        Assert.Equal([5, 6, 4, 5], nums);

        nums = [1, 4, 3, 4, 1, 2, 1, 3, 1, 3, 2, 3, 3];
        SortWiggle_ByPartition(nums);
        Assert.Equal([3, 4, 1, 4, 1, 3, 2, 3, 1, 3, 2, 3, 1], nums);

        nums = [1, 1, 2, 1, 2, 2, 1];
        SortWiggle_ByPartition(nums);
        Assert.Equal([1, 2, 1, 2, 1, 2, 1], nums);

        nums = [1, 5, 1, 1, 6, 4];
        SortWiggle_ByPartition(nums);
        Assert.Equal([1, 5, 1, 6, 1, 4], nums);

        nums = [1, 3, 2, 2, 3, 1];
        SortWiggle_ByPartition(nums);
        Assert.Equal([2, 3, 1, 3, 1, 2], nums);
    }
}