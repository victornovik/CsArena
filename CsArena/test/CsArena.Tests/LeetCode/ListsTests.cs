namespace CsArena.Tests.LeetCode;

using static Lists;

public class ListsTests
{
    [Fact]
    public void LinkedListCycleTest()
    {
        var head = new ListNode(3, new ListNode(2, new ListNode(0, new ListNode(-4))));
        head.next!.next!.next!.next = head.next;
        Assert.True(HasCycle(head));

        head = new ListNode(1, new ListNode(2));
        head.next!.next = head;
        Assert.True(HasCycle(head));

        head = new ListNode(1);
        Assert.False(HasCycle(head));

        head = new ListNode(0, new ListNode(1, new ListNode(2)));
        Assert.False(HasCycle(head));

        Assert.False(HasCycle(null));
    }

    [Fact]
    public void RemoveNthFromEndTest()
    {
        var head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5)))));
        var res = RemoveNthFromEnd(head, 2);

        Assert.NotNull(res);
        while (res.next != null)
        {
            // Node with 4 is removed
            Assert.NotEqual(4, res.val);
            res = res.next;
        }

        head = new ListNode(1, new ListNode(2));
        res = RemoveNthFromEnd(head, 1);
        Assert.NotNull(res);
        Assert.Equal(1, res.val);
        Assert.Null(res.next);

        head = new ListNode(1, new ListNode(2));
        res = RemoveNthFromEnd(head, 2);
        Assert.NotNull(res);
        Assert.Equal(2, res.val);
        Assert.Null(res.next);
    }

    [Fact]
    public void ReverseBetweenTest()
    {
        // Reverse chunk in the middle
        var head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5)))));
        var res = ReverseBetween(head, 2, 4);
        Assert.NotNull(res);
        Assert.Equal([1, 4, 3, 2, 5], res.ToList());

        // Reverse chunk at the end
        head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5)))));
        res = ReverseBetween(head, 3, 5);
        Assert.NotNull(res);
        Assert.Equal([1, 2, 5, 4, 3], res.ToList());

        // Reverse chunk at the start
        head = new ListNode(1, new ListNode(2, new ListNode(3)));
        res = ReverseBetween(head, 1, 2);
        Assert.NotNull(res);
        Assert.Equal([2, 1, 3], res.ToList());

        // Reverse the whole list
        head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5)))));
        res = ReverseBetween(head, 1, 5);
        Assert.NotNull(res);
        Assert.Equal([5, 4, 3, 2, 1], res.ToList());

        // Reverse the whole list
        head = new ListNode(3, new ListNode(5));
        res = ReverseBetween(head, 1, 2);
        Assert.NotNull(res);
        Assert.Equal([5, 3], res.ToList());

        // Nothing to reverse
        head = new ListNode(5);
        res = ReverseBetween(head, 1, 1);
        Assert.NotNull(res);
        Assert.Equal([5], res.ToList());
    }

    [Fact]
    public void ReverseKGroupTest()
    {
        var head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5)))));
        var res = ReverseKGroup(head, 2);
        Assert.NotNull(res);
        Assert.Equal([2, 1, 4, 3, 5], res.ToList());

        head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5)))));
        res = ReverseKGroup(head, 3);
        Assert.NotNull(res);
        Assert.Equal([3, 2, 1, 4, 5], res.ToList());

        head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5)))));
        res = ReverseKGroup(head, 1);
        Assert.NotNull(res);
        Assert.Equal([1, 2, 3, 4, 5], res.ToList());

        head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5, new ListNode(6))))));
        res = ReverseKGroup(head, 3);
        Assert.NotNull(res);
        Assert.Equal([3, 2, 1, 6, 5, 4], res.ToList());

        head = new ListNode(1);
        res = ReverseKGroup(head, 1);
        Assert.NotNull(res);
        Assert.Equal([1], res.ToList());
    }

    [Fact]
    public void ReverseListTest()
    {
        var head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5)))));
        var reversed = ReverseList(head);
        Assert.NotNull(reversed);
        //Assert.Equal(5, count);
        Assert.Equal([5, 4, 3, 2, 1], reversed.ToList());

        reversed = ReverseList(reversed);
        Assert.NotNull(reversed);
        //Assert.Equal(5, count);
        Assert.Equal([1, 2, 3, 4, 5], reversed.ToList());

        head = new ListNode(1, new ListNode(2));
        reversed = ReverseList(head);
        Assert.NotNull(reversed);
        //Assert.Equal(2, count);
        Assert.Equal([2, 1], reversed.ToList());

        head = new ListNode(1);
        reversed = ReverseList(head);
        Assert.NotNull(reversed);
        //Assert.Equal(1, count);
        Assert.Equal([1], reversed.ToList());

        head = null;
        reversed = ReverseList(head);
        Assert.Null(reversed);
        //Assert.Equal(0, count);
    }

    [Fact]
    public void RotateRightTest()
    {
        var head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5)))));
        var res = RotateRight(head, 2);
        Assert.NotNull(res);
        Assert.Equal([4, 5, 1, 2, 3], res.ToList());

        head = new ListNode(0, new ListNode(1, new ListNode(2)));
        res = RotateRight(head, 4);
        Assert.NotNull(res);
        Assert.Equal([2, 0, 1], res.ToList());

        head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, new ListNode(5)))));
        res = RotateRight(head, 5);
        Assert.NotNull(res);
        Assert.Equal([1, 2, 3, 4, 5], res.ToList());
    }

    [Fact]
    public void AddTwoNumbersTest()
    {
        var l1 = new ListNode(2, new ListNode(4, new ListNode(3)));
        var l2 = new ListNode(5, new ListNode(6, new ListNode(4)));
        var res = AddTwoNumbers(l1, l2);
        Assert.NotNull(res);
        Assert.Equal([7, 0, 8], res.ToList());

        l1 = new ListNode();
        l2 = new ListNode();
        res = AddTwoNumbers(l1, l2);
        Assert.NotNull(res);
        Assert.Equal([0], res.ToList());

        l1 = new ListNode(9,
            new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9)))))));
        l2 = new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9))));
        res = AddTwoNumbers(l1, l2);
        Assert.NotNull(res);
        Assert.Equal([8, 9, 9, 9, 0, 0, 0, 1], res.ToList());
    }

    [Fact]
    public void MergeSortTest()
    {
        var l = new ListNode(4, new ListNode(2, new ListNode(1, new ListNode(3))));
        var res = MergeSortList(l);
        Assert.NotNull(res);
        Assert.Equal([1, 2, 3, 4], res.ToList());

        l = new ListNode(-1, new ListNode(5, new ListNode(3, new ListNode(4, new ListNode()))));
        res = MergeSortList(l);
        Assert.NotNull(res);
        Assert.Equal([-1, 0, 3, 4, 5], res.ToList());

        l = null;
        res = MergeSortList(l);
        Assert.Null(res);
    }

    [Fact]
    public void MergeTwoListsTest()
    {
        var l1 = new ListNode(1, new ListNode(2, new ListNode(4)));
        var l2 = new ListNode(1, new ListNode(3, new ListNode(4)));
        var res = MergeSortedLists(l1, l2);
        Assert.NotNull(res);
        Assert.Equal([1, 1, 2, 3, 4, 4], res.ToList());

        l1 = null;
        l2 = null;
        res = MergeSortedLists(l1, l2);
        Assert.Null(res);

        l1 = null;
        l2 = new ListNode();
        res = MergeSortedLists(l1, l2);
        Assert.NotNull(res);
        Assert.Equal([0], res.ToList());

        l1 = new ListNode(-9, new ListNode(3));
        l2 = new ListNode(5, new ListNode(7));
        res = MergeSortedLists(l1, l2);
        Assert.NotNull(res);
        Assert.Equal([-9, 3, 5, 7], res.ToList());
    }

    [Fact]
    public void PartitionListTest()
    {
        var head = new ListNode(1, new ListNode(2, new ListNode(4, new ListNode(3, new ListNode(5)))));
        var res = PartitionList(head, e => e < 3);
        Assert.NotNull(res);
        Assert.Equal([1, 2, 4, 3, 5], res.ToList());

        head = new ListNode(1, new ListNode(4, new ListNode(3, new ListNode(2, new ListNode(5, new ListNode(2))))));
        res = PartitionList(head, e => e < 3);
        Assert.NotNull(res);
        Assert.Equal([1, 2, 2, 4, 5, 3], res.ToList());

        head = new ListNode(2, new ListNode(1));
        res = PartitionList(head, e => e < 2);
        Assert.NotNull(res);
        Assert.Equal([1, 2], res.ToList());

        head = new ListNode(1, new ListNode(30, new ListNode(-4, new ListNode(3, new ListNode(5,
            new ListNode(-4, new ListNode(1, new ListNode(6, new ListNode(-8, new ListNode(2,
                new ListNode(-5, new ListNode(64, new ListNode(1, new ListNode(92))))))))))))));
        res = PartitionList(head, e => e < 3);
        Assert.NotNull(res);
        Assert.Equal([1, -4, -4, 1, -8, 2, -5, 1, 5, 30, 3, 64, 6, 92], res.ToList());
    }

    [Fact]
    public void StablePartitionTest()
    {
        var head = new ListNode(1, new ListNode(4, new ListNode(3, new ListNode(2, new ListNode(5, new ListNode(2))))));
        var res = PartitionListStable(head, e => e < 3);
        Assert.NotNull(res);
        Assert.Equal([1, 2, 2, 4, 3, 5], res.ToList());

        head = new ListNode(2, new ListNode(1));
        res = PartitionListStable(head, e => e < 2);
        Assert.NotNull(res);
        Assert.Equal([1, 2], res.ToList());
    }

    [Fact]
    public void IsPartitionedTest()
    {
        var head = new ListNode(1, new ListNode(2, new ListNode(4, new ListNode(3, new ListNode(5)))));
        Assert.True(IsPartitioned(head, e => e < 3));

        head = new ListNode(1, new ListNode(2, new ListNode(2, new ListNode(4, new ListNode(5, new ListNode(3))))));
        Assert.True(IsPartitioned(head, e => e < 3));

        head = new ListNode(1, new ListNode(2));
        Assert.True(IsPartitioned(head, e => e < 2));

        head = new ListNode(1, new ListNode(-4, new ListNode(-4, new ListNode(1, new ListNode(-8,
            new ListNode(2, new ListNode(-5, new ListNode(1, new ListNode(5, new ListNode(30,
                new ListNode(3, new ListNode(64, new ListNode(6, new ListNode(92))))))))))))));
        Assert.True(IsPartitioned(head, e => e < 3));
    }
}