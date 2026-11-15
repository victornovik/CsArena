namespace CsArena.Tests.LeetCode;

public static class Lists
{
    /**
        Given head, the head of a linked list, determine if the linked list has a cycle in it.
        There is a cycle in a linked list if there is some node in the list that can be reached again by continuously following the next pointer.
        Internally, pos is used to denote the index of the node that tail's next pointer is connected to.
        Note that pos is not passed as a parameter.
        Return true if there is a cycle in the linked list. Otherwise, return false.
        Solution https://leetcode.com/problems/linked-list-cycle/solutions/5851608/simple-c-solution-by-victornovik-k3v3/
        # Approach
            Run two iterators `it1` and `it2`. At a time `it1` steps by 1 node, `it2` steps by 2 nodes. 
            They gonna meet each other if there is a loop in the list.
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static bool HasCycle(ListNode? head)
    {
        ListNode? it1 = head, it2 = head;
        while (it2?.next?.next != null)
        {
            it1 = it1!.next;
            it2 = it2.next.next;
            if (it1 == it2)
                return true;
        }
        return false;
    }

    /**
    Given the head of a linked list, remove the n-th node from the end of the list and return its head.
    Solution https://leetcode.com/problems/remove-nth-node-from-end-of-list/solutions/6182538/c-solution-beats-100-by-victornovik-yzwj
    # Approach
        Run two iterators `it1` and `it2` but `it2` will start with a delay `n` after `it1`. 
        So now when `it1` points to the last node, `it2` points to the node before n-th node from the end. 
        If `it1` points to the space beyond the last node then it means n-th node from the end is the head of the list.
    # Complexity
        - Time complexity: O(n)
        - Space complexity: O(1)
     */
    public static ListNode? RemoveNthFromEnd(ListNode? head, int n)
    {
        ListNode? it1 = head, it2 = head;
        for (var i = 0; i < n; ++i)
            it1 = it1?.next;

        while (it1?.next != null)
        {
            it1 = it1.next;
            it2 = it2!.next;
        }
        // Now it2 points to the node before n-th node from the end.
        if (it1 == null)
            head = head!.next;
        else
            it2!.next = it2.next?.next;

        return head;
    }

    /**
        Given the head of a singly linked list and two integers left and right where left less or equal right, 
        reverse the nodes of the list from position left to position right, and return the reversed list.
        Solution https://leetcode.com/problems/reverse-linked-list-ii/solutions/5978781/beats-100-c-solution
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static ListNode? ReverseBetween(ListNode? head, int left, int right)
    {
        if (head == null || left >= right)
            return head;

        head = new ListNode(-1, head);
        var prev = head;

        // prev moves to the node before the range to be reversed 
        for (var i = 0; i < left - 1; ++i)
            prev = prev?.next;

        ReverseNNodes(prev, right - left + 1);
        return head.next;
    }

    /**
        Reverse `n` nodes of the list.
        `prev` points to the node before the range to be reversed.
        Return the last node of the reversed sequence.
     */
    public static ListNode? ReverseNNodes(ListNode? prev, int n)
    {
        if (prev == null)
            return prev;

        var cur = prev.next;
        for (; n > 1; n--)
        {
            var tmp = cur?.next;
            cur!.next = tmp?.next;
            tmp!.next = prev.next;
            prev.next = tmp;
        }
        return cur;
    }

    /**
        Given the head of a linked list, reverse every k nodes of the list at a time, and return the modified list.
        k is a positive integer and is less than or equal to the length of the linked list.
        If the number of nodes is not a multiple of k then left-out nodes, in the end, should remain as it is.
        You may not alter the values in the list's nodes, only nodes links may be changed.
        Solution https://leetcode.com/problems/reverse-nodes-in-k-group/solutions/5863697/simple-c-solution-by-victornovik-5usw/
        # Approach
            In `while` loop we check if there are `k` nodes ahead by calling `IsNNodesAhead`.
            If yes - reverse those `k` nodes by calling `ReverseNNodes`.
            If no - keep the rest of the list as is and return the list.
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static ListNode? ReverseKGroup(ListNode? head, int k)
    {
        if (head == null)
            return head;

        head = new ListNode(-1, head);
        var prev = head;

        while (IsNNodesAfterNode(prev?.next, k))
            prev = ReverseNNodes(prev, k);
        return head.next;

        bool IsNNodesAfterNode(ListNode? node, int n)
        {
            while (node != null)
            {
                if (--n == 0)
                    return true;
                node = node.next;
            }
            return false;
        }
    }

    /**
        Given the head of a singly linked list, reverse the list, and return the reversed list.
        Solution https://leetcode.com/problems/reverse-linked-list/solutions/5864077/simple-c-solution-by-victornovik-e70b/        
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static ListNode? ReverseList(ListNode? head)
    {
        head = new ListNode(-1, head);
        for (var cur = head.next; cur?.next != null;)
        {
            var tmp = cur.next;
            cur.next = tmp?.next;
            tmp!.next = head.next;
            head.next = tmp;
        }
        return head.next;
    }

    /**
        Given the head of a linked list, rotate the list to the right by k places.
        Solution https://leetcode.com/problems/rotate-list/solutions/5865697/c-solution-by-reversions-by-victornovik-425w
        # Approach
            1. We have to know the length of SLL by calling ListNode.Count(). 
            2. Pay attention that `k` can be greater than the length. 
                So we have to find `k % count` and use it further as a shift. 
                E.g. for 3-nodes list Rot4, Rot7 and Rot10 will give us the same result as Rot1. 
            Now let's take input [1,2,3,4,5] and k=2.
                1. Reverse left 5-2 nodes [**1,2,3**,4,5] -> [**3,2,1**,4,5]
                2. Reverse right 2 nodes [3,2,1,**4,5**] -> [3,2,1,**5,4**]
                3. Reverse the whole list [**3,2,1,5,4**] -> [**4,5,1,2,3**]            
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
     */
    public static ListNode? RotateRight(ListNode? head, int k)
    {
        if (head == null)
            return head;

        var count = head.Count();
        k %= count;
        if (k == 0)
            return head;

        head = new ListNode(-1, head);
        ReverseNNodes(ReverseNNodes(head, count - k), k);
        ReverseNNodes(head, count);
        return head.next;
    }

    /**
        Check if the list is partitioned into two parts specified by predicate.
        The first part where predicate is true, the second one where predicate is false.
     */
    public static bool IsPartitioned(ListNode? head, Func<int, bool> pred)
    {
        for (; head != null && pred(head.val); head = head.next) ;
        for (; head != null; head = head.next)
        {
            if (pred(head.val))
                return false;
        }
        return true;
    }

    /**
        Partition a singly linked list such way that all nodes where `pred` is true come before nodes where `pred` is false.
        Relative order of the elements is not preserved.
     */
    public static ListNode? PartitionList(ListNode? head, Func<int, bool> pred)
    {
        if (head == null)
            return null;

        var it1 = head;

        while (pred(it1.val) && it1.next != null)
            it1 = it1.next;

        for (var it2 = it1.next; it1 != null && it2 != null; it2 = it2.next)
        {
            if (pred(it2.val))
            {
                // Swap values
                (it1.val, it2.val) = (it2.val, it1.val);
                it1 = it1.next;
            }
        }
        return head;
    }

    /**
       Given the head of a linked list and a value x, partition it such that all nodes less than x come before nodes greater than or equal to x.
       You should preserve the original relative order of the nodes in each of the two partitions.
       Solution https://leetcode.com/problems/partition-list/solutions/5908495/simple-o-n-c-solution
       # Approach
           If element less than `x` add it to `left` list, otherwise add it to `right` list.
           Doing so we preserve the existing order.
           After that append the right list to the left one and return the merged list.
       # Complexity
           - Time complexity: O(n)
           - Space complexity: O(n)
    */
    public static ListNode? PartitionListStable(ListNode? head, Func<int, bool> pred)
    {
        if (head == null)
            return null;

        ListNode leftHead = new(-1), rightHead = new(-1);
        ListNode left = leftHead, right = rightHead;

        for (; head != null; head = head.next)
        {
            if (pred(head.val))
                left = left.next = new ListNode(head.val);
            else
                right = right.next = new ListNode(head.val);
        }
        // Merge left list (all values < x in the same order) and right list (all values >= x in the same order)
        left.next = rightHead.next;
        return leftHead.next;
    }

    /**
        You are given two non-empty linked lists representing two non-negative integers.
        The digits are stored in reverse order, and each of their nodes contains a single digit.
        Add the two numbers and return the sum as a linked list.
        You may assume the two numbers do not contain any leading zero, except the number 0 itself.
        Example
            Input: l1 = [2,4,3], l2 = [5,6,4]
            Output: [7,0,8]
            Explanation: 342 + 465 = 807.
        Solution https://leetcode.com/problems/add-two-numbers/solutions/5886286/pretty-simple-c-solution-by-victornovik-29nk/
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
     */
    public static ListNode? AddTwoNumbers(ListNode? l1, ListNode? l2)
    {
        var head = new ListNode(-1);
        var cur = head;
        var remainder = 0;

        while (l1 != null || l2 != null || remainder > 0)
        {
            cur.next = new ListNode();
            cur = cur.next;

            var sum = (l1?.val ?? 0) + (l2?.val ?? 0) + remainder;
            if (sum >= 10)
            {
                cur.val = sum - 10;
                remainder = 1;
            }
            else
            {
                cur.val = sum;
                remainder = 0;
            }
            l1 = l1?.next;
            l2 = l2?.next;
        }
        return head.next;
    }

    /**
        You are given the heads of two sorted linked lists list1 and list2.
        Merge the two lists into one sorted list. The list should be made by splicing together the nodes of the first two lists.
        Return the head of the merged linked list.
        Solution https://leetcode.com/problems/merge-two-sorted-lists/solutions/5978717/beats-100-c-solution
        # Approach
           Compare the current list1 node vs list2 node and set cur.next pointer to the smaller node. 
           When we reach the end of any list just set cur.next to the rest of the other list in order to append all its nodes by one operation.
           Pay attention we do not allocate memory for nodes. 
           We just change next pointer in already existing nodes and generate single sorted list.
        # Complexity
           - Time complexity: O(n)
           - Space complexity: O(1)
    */
    public static ListNode? MergeSortedLists(ListNode? list1, ListNode? list2)
    {
        var head = new ListNode();
        var cur = head;

        while (list1 != null && list2 != null)
        {
            if (list1.val < list2.val)
            {
                cur.next = list1;
                list1 = list1.next;
            }
            else
            {
                cur.next = list2;
                list2 = list2.next;
            }
            cur = cur.next;
        }

        if (list1 != null)
            cur.next = list1;
        else if (list2 != null)
            cur.next = list2;

        return head.next;
    }

    /**
        Split singly linked list into two equal parts.
        Move head +2 steps and middle +1 step until the head reaches the end.
        Then split the list into two parts:
            1. The first part from head to middle node. Mark the end of the first list with null.
            2. The second part from middle->next to the list end.
     */
    public static ListNode? SplitListIntoTwoHalves(ListNode? head)
    {
        var middle = head;

        while (head?.next != null)
        {
            head = head.next.next;
            if (head != null)
                middle = middle!.next;
        }

        var secondPart = middle!.next;
        middle.next = null;
        return secondPart;
    }

    /**
        Given the head of a singly linked list, return the list after sorting it in ascending order.
        Solution https://leetcode.com/problems/sort-list/solutions/5979801/c-merge-sort
        # Approach
            I'd prefer a merge sort because its space complexity is O(1).
        # Complexity
            - Time complexity: O(n * log(n))
            - Space complexity: O(1)
     */
    public static ListNode? MergeSortList(ListNode? head)
    {
        if (head?.next == null)
            return head;

        var secondPart = SplitListIntoTwoHalves(head);
        head = MergeSortList(head);
        secondPart = MergeSortList(secondPart);
        return MergeSortedLists(head, secondPart);
    }
}