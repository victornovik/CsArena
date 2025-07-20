namespace CsArena.Tests.LeetCode;

using static Heap;

public class HeapTests
{
    [Fact]
    public void FindKthLargestMinHeapTest()
    {
        Assert.Equal(5, FindKthLargest([3, 2, 1, 5, 6, 4], 2));
        Assert.Equal(4, FindKthLargest([3, 2, 3, 1, 2, 4, 5, 5, 6], 4));
        Assert.Equal(6, FindKthLargest([6], 1));
        Assert.Equal(6, FindKthLargest([5, 6], 1));
        Assert.Equal(5, FindKthLargest([5, 6], 2));
    }

    [Fact]
    public void MergeKListsTest()
    {
        var res = MergeKLists(
        [
            new ListNode(1, new ListNode(4, new ListNode(5))),
            new ListNode(1, new ListNode(3, new ListNode(4))),
            new ListNode(2, new ListNode(6))
        ]);
        Assert.NotNull(res);
        Assert.Equal([1, 1, 2, 3, 4, 4, 5, 6], res.ToList());

        res = MergeKLists([]);
        Assert.Null(res);

        res = MergeKLists([null]);
        Assert.Null(res);
    }

    [Fact]
    public void SortArrayTest()
    {
        Assert.Equal([], SortArray([]));
        Assert.Equal([1], SortArray([1]));
        Assert.Equal([1, 2, 3, 5], SortArray([5, 2, 3, 1]));
        Assert.Equal([0, 0, 1, 1, 2, 5], SortArray([5, 1, 1, 2, 0, 0]));
        Assert.Equal([0, 100, 110], SortArray([110, 100, 0]));
    }

    [Fact]
    public void LongestDiverseStringTest()
    {
        Assert.Equal("ccaccbcc", LongestDiverseString_MaxHeap(1, 1, 7));
        Assert.Equal("aabaa", LongestDiverseString_MaxHeap(7, 1, 0));
        Assert.Equal("ababcbcacab", LongestDiverseString_MaxHeap(4, 4, 3));
        Assert.Equal("ccbccbc", LongestDiverseString_MaxHeap(0, 2, 5));
        Assert.Equal("ccbccbcbcbcbcbcbcbc", LongestDiverseString_MaxHeap(0, 8, 11));
        Assert.Equal("cc", LongestDiverseString_MaxHeap(0, 0, 7));
    }
}