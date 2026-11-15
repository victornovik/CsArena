namespace CsArena.Tests.LeetCode;

using CsArena.Core.LeetCode;

using static CsArena.Core.LeetCode.Lists;

public class ListsTests
{
    // Builds a fresh list from values (null for an empty array). Every theory case creates its own
    // nodes, so cases stay independent when xUnit runs them in parallel.
    private static ListNode? Build(params int[] values)
    {
        ListNode? head = null;
        for (var i = values.Length - 1; i >= 0; i--)
        {
            head = new ListNode(values[i], head);
        }
        return head;
    }

    // A null list compares as an empty sequence; a non-null list always has at least one node.
    private static List<int> Values(ListNode? head) => head?.ToList() ?? [];

    [Theory]
    [InlineData(new[] { 3, 2, 0, -4 }, 1, true)]
    [InlineData(new[] { 1, 2 }, 0, true)]
    [InlineData(new[] { 1 }, -1, false)]
    [InlineData(new[] { 0, 1, 2 }, -1, false)]
    [InlineData(new int[] { }, -1, false)]
    public void LinkedListCycleTest(int[] values, int cyclePosition, bool expected)
    {
        var head = Build(values);
        if (cyclePosition >= 0)
        {
            // Link the tail back to the node at cyclePosition
            var target = head;
            for (var i = 0; i < cyclePosition; i++) target = target!.next;
            var tail = head;
            while (tail!.next != null) tail = tail.next;
            tail.next = target;
        }

        Assert.Equal(expected, HasCycle(head));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, new[] { 1, 2, 3, 5 })]
    [InlineData(new[] { 1, 2 }, 1, new[] { 1 })]
    [InlineData(new[] { 1, 2 }, 2, new[] { 2 })]
    public void RemoveNthFromEndTest(int[] values, int n, int[] expected)
    {
        var res = RemoveNthFromEnd(Build(values), n);

        Assert.NotNull(res);
        Assert.Equal(expected, Values(res));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, 4, new[] { 1, 4, 3, 2, 5 })] // chunk in the middle
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 3, 5, new[] { 1, 2, 5, 4, 3 })] // chunk at the end
    [InlineData(new[] { 1, 2, 3 }, 1, 2, new[] { 2, 1, 3 })] // chunk at the start
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 1, 5, new[] { 5, 4, 3, 2, 1 })] // the whole list
    [InlineData(new[] { 3, 5 }, 1, 2, new[] { 5, 3 })] // the whole two-node list
    [InlineData(new[] { 5 }, 1, 1, new[] { 5 })] // nothing to reverse
    public void ReverseBetweenTest(int[] values, int left, int right, int[] expected)
    {
        var res = ReverseBetween(Build(values), left, right);

        Assert.NotNull(res);
        Assert.Equal(expected, Values(res));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, new[] { 2, 1, 4, 3, 5 })]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 3, new[] { 3, 2, 1, 4, 5 })]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 1, new[] { 1, 2, 3, 4, 5 })]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6 }, 3, new[] { 3, 2, 1, 6, 5, 4 })]
    [InlineData(new[] { 1 }, 1, new[] { 1 })]
    public void ReverseKGroupTest(int[] values, int k, int[] expected)
    {
        var res = ReverseKGroup(Build(values), k);

        Assert.NotNull(res);
        Assert.Equal(expected, Values(res));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, new[] { 5, 4, 3, 2, 1 })]
    [InlineData(new[] { 1, 2 }, new[] { 2, 1 })]
    [InlineData(new[] { 1 }, new[] { 1 })]
    [InlineData(new int[] { }, new int[] { })]
    public void ReverseListTest(int[] values, int[] expected)
    {
        var reversed = ReverseList(Build(values));

        // An empty input yields null; any other input yields a non-null head.
        Assert.Equal(values.Length == 0, reversed is null);
        Assert.Equal(expected, Values(reversed));
    }

    [Fact]
    public void ReverseListTwiceRestoresOriginalOrderTest()
    {
        var reversed = ReverseList(Build(1, 2, 3, 4, 5));
        Assert.NotNull(reversed);
        Assert.Equal([5, 4, 3, 2, 1], Values(reversed));

        reversed = ReverseList(reversed);
        Assert.NotNull(reversed);
        Assert.Equal([1, 2, 3, 4, 5], Values(reversed));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, new[] { 4, 5, 1, 2, 3 })]
    [InlineData(new[] { 0, 1, 2 }, 4, new[] { 2, 0, 1 })]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 5, new[] { 1, 2, 3, 4, 5 })]
    public void RotateRightTest(int[] values, int k, int[] expected)
    {
        var res = RotateRight(Build(values), k);

        Assert.NotNull(res);
        Assert.Equal(expected, Values(res));
    }

    [Theory]
    [InlineData(new[] { 2, 4, 3 }, new[] { 5, 6, 4 }, new[] { 7, 0, 8 })]
    [InlineData(new[] { 0 }, new[] { 0 }, new[] { 0 })]
    [InlineData(new[] { 9, 9, 9, 9, 9, 9, 9 }, new[] { 9, 9, 9, 9 }, new[] { 8, 9, 9, 9, 0, 0, 0, 1 })]
    public void AddTwoNumbersTest(int[] first, int[] second, int[] expected)
    {
        var res = AddTwoNumbers(Build(first), Build(second));

        Assert.NotNull(res);
        Assert.Equal(expected, Values(res));
    }

    [Theory]
    [InlineData(new[] { 4, 2, 1, 3 }, new[] { 1, 2, 3, 4 })]
    [InlineData(new[] { -1, 5, 3, 4, 0 }, new[] { -1, 0, 3, 4, 5 })]
    [InlineData(new int[] { }, new int[] { })]
    public void MergeSortTest(int[] values, int[] expected)
    {
        var res = MergeSortList(Build(values));

        Assert.Equal(values.Length == 0, res is null);
        Assert.Equal(expected, Values(res));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 4 }, new[] { 1, 3, 4 }, new[] { 1, 1, 2, 3, 4, 4 })]
    [InlineData(new int[] { }, new int[] { }, new int[] { })]
    [InlineData(new int[] { }, new[] { 0 }, new[] { 0 })]
    [InlineData(new[] { -9, 3 }, new[] { 5, 7 }, new[] { -9, 3, 5, 7 })]
    public void MergeTwoListsTest(int[] first, int[] second, int[] expected)
    {
        var res = MergeSortedLists(Build(first), Build(second));

        Assert.Equal(expected.Length == 0, res is null);
        Assert.Equal(expected, Values(res));
    }

    // Elements below the threshold go first; the order inside the second group is not guaranteed.
    [Theory]
    [InlineData(new[] { 1, 2, 4, 3, 5 }, 3, new[] { 1, 2, 4, 3, 5 })]
    [InlineData(new[] { 1, 4, 3, 2, 5, 2 }, 3, new[] { 1, 2, 2, 4, 5, 3 })]
    [InlineData(new[] { 2, 1 }, 2, new[] { 1, 2 })]
    [InlineData(new[] { 1, 30, -4, 3, 5, -4, 1, 6, -8, 2, -5, 64, 1, 92 }, 3,
        new[] { 1, -4, -4, 1, -8, 2, -5, 1, 5, 30, 3, 64, 6, 92 })]
    public void PartitionListTest(int[] values, int threshold, int[] expected)
    {
        var res = PartitionList(Build(values), e => e < threshold);

        Assert.NotNull(res);
        Assert.Equal(expected, Values(res));
    }

    [Theory]
    [InlineData(new[] { 1, 4, 3, 2, 5, 2 }, 3, new[] { 1, 2, 2, 4, 3, 5 })]
    [InlineData(new[] { 2, 1 }, 2, new[] { 1, 2 })]
    public void StablePartitionTest(int[] values, int threshold, int[] expected)
    {
        var res = PartitionListStable(Build(values), e => e < threshold);

        Assert.NotNull(res);
        Assert.Equal(expected, Values(res));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 4, 3, 5 }, 3)]
    [InlineData(new[] { 1, 2, 2, 4, 5, 3 }, 3)]
    [InlineData(new[] { 1, 2 }, 2)]
    [InlineData(new[] { 1, -4, -4, 1, -8, 2, -5, 1, 5, 30, 3, 64, 6, 92 }, 3)]
    public void IsPartitionedTest(int[] values, int threshold)
    {
        Assert.True(IsPartitioned(Build(values), e => e < threshold));
    }
}