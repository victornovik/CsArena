namespace CsArena.Tests.LeetCode;

using static BinarySearch;

public class BinarySearchTests
{
    [Fact]
    public void FirstAndLastPositionOfElementTest()
    {
        Assert.Equal([0, 1], FindFirstAndLastPosition([2, 2], 2));
        Assert.Equal([0, 0], FindFirstAndLastPosition([1], 1));
        Assert.Equal([3, 4], FindFirstAndLastPosition([5, 7, 7, 8, 8, 10], 8));
        Assert.Equal([-1, -1], FindFirstAndLastPosition([5, 7, 7, 8, 8, 10], 6));
        Assert.Equal([-1, -1], FindFirstAndLastPosition([], 8));

        Assert.Equal([0, 1], FindFirstAndLastPosition_ArrayBinarySearch([2, 2], 2));
        Assert.Equal([0, 0], FindFirstAndLastPosition_ArrayBinarySearch([1], 1));
        Assert.Equal([3, 4], FindFirstAndLastPosition_ArrayBinarySearch([5, 7, 7, 8, 8, 10], 8));
        Assert.Equal([-1, -1], FindFirstAndLastPosition_ArrayBinarySearch([5, 7, 7, 8, 8, 10], 6));
        Assert.Equal([-1, -1], FindFirstAndLastPosition_ArrayBinarySearch([], 8));
    }

    [Fact]
    public void SearchInsertPositionTest()
    {
        Assert.Equal(2, SearchInsertPosition([1, 3, 5, 6], 5));
        Assert.Equal(3, SearchInsertPosition([1, 3, 5, 6], 6));
        Assert.Equal(1, SearchInsertPosition([1, 3, 5, 6], 2));
        Assert.Equal(0, SearchInsertPosition([1, 3, 5, 6], 0));
        Assert.Equal(4, SearchInsertPosition([1, 3, 5, 6], 7));

        Assert.Equal(2, SearchInsertPosition_ArrayBinarySearch([1, 3, 5, 6], 5));
        Assert.Equal(3, SearchInsertPosition_ArrayBinarySearch([1, 3, 5, 6], 6));
        Assert.Equal(1, SearchInsertPosition_ArrayBinarySearch([1, 3, 5, 6], 2));
        Assert.Equal(0, SearchInsertPosition_ArrayBinarySearch([1, 3, 5, 6], 0));
        Assert.Equal(4, SearchInsertPosition_ArrayBinarySearch([1, 3, 5, 6], 7));
    }

    [Fact]
    public void FirstBadVersionTest()
    {
        Assert.Equal(4, FirstBadVersion(5, n => n >= 4));
        Assert.Equal(1, FirstBadVersion(1, n => n >= 1));
    }

    [Fact]
    public void ShipWithinDaysTest()
    {
        Assert.Equal(15, ShipWithinDays([1, 2, 3, 4, 5, 6, 7, 8, 9, 10], 5));
        Assert.Equal(6, ShipWithinDays([3, 2, 2, 4, 1, 4], 3));
        Assert.Equal(3, ShipWithinDays([1, 2, 3, 1, 1], 4));
    }

    [Fact]
    public void SplitArrayTest()
    {
        Assert.Equal(18, SplitArray([7, 2, 5, 10, 8], 2));
        Assert.Equal(9, SplitArray([1, 2, 3, 4, 5], 2));
    }

    [Fact]
    public void MinEatingSpeedTest()
    {
        Assert.Equal(14, MinEatingSpeed([332484035, 524908576, 855865114, 632922376, 222257295, 690155293, 112677673, 679580077, 337406589, 290818316, 877337160, 901728858, 679284947, 688210097, 692137887, 718203285, 629455728, 941802184], 823_855_818));
        Assert.Equal(2, MinEatingSpeed([312_884_470], 312_884_469));
        Assert.Equal(4, MinEatingSpeed([3, 6, 7, 11], 8));
        Assert.Equal(30, MinEatingSpeed([30, 11, 23, 4, 20], 5));
        Assert.Equal(23, MinEatingSpeed([30, 11, 23, 4, 20], 6));
    }

    [Fact]
    public void MinBloomingDaysTest()
    {
        Assert.Equal(3, MinBloomingDays([1, 10, 3, 10, 2], 3, 1));
        Assert.Equal(-1, MinBloomingDays([1, 10, 3, 10, 2], 3, 2));
        Assert.Equal(12, MinBloomingDays([7, 7, 7, 7, 12, 7, 7], 2, 3));
    }

    [Fact]
    public void FindKthNumberOfMultiplicationTableTest()
    {
        Assert.Equal(3, FindKthNumberOfMultiplicationTable(12, 12, 5));
        Assert.Equal(3, FindKthNumberOfMultiplicationTable(3, 3, 5));
        Assert.Equal(6, FindKthNumberOfMultiplicationTable(2, 3, 6));
    }

    [Fact]
    public void SmallestDistancePairTest()
    {
        Assert.Equal(3, SmallestDistancePair([1, 2, 3, 4, 5], 9));
        Assert.Equal(1, SmallestDistancePair([1, 5, 6, 7, 8], 2));
        Assert.Equal(0, SmallestDistancePair([1, 3, 1], 1));
        Assert.Equal(0, SmallestDistancePair([1, 1, 1], 2));
        Assert.Equal(5, SmallestDistancePair([1, 6, 1], 3));
    }

    [Fact]
    public void NthUglyNumberTest()
    {
        //The ugly numbers are 2, 3, 4, 5, 6, 8, 9, 10... The 3-rd numbers is 4
        Assert.Equal(4, NthUglyNumber(n: 3, a: 2, b: 3, c: 5));

        //The ugly numbers are 2, 3, 4, 6, 8, 9, 10, 12... The 4-th number is 6
        Assert.Equal(6, NthUglyNumber(n: 4, a: 2, b: 3, c: 4));

        //The ugly numbers are 2, 4, 6, 8, 10, 11, 12, 13... The 5-th number is 10
        Assert.Equal(10, NthUglyNumber(n: 5, a: 2, b: 11, c: 13));

        Assert.Equal(1_999_999_984, NthUglyNumber(n: 1_000_000_000, a: 2, b: 217_983_653, c: 336_916_467));
    }

    [Fact]
    public void SmallestDivisorTest()
    {
        Assert.Equal(5, SmallestDivisor([1, 2, 5, 9], 6));
        Assert.Equal(44, SmallestDivisor([44, 22, 33, 11, 1], 5));
    }
}