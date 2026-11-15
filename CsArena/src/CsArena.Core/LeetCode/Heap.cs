using System.Text;

namespace CsArena.Tests.LeetCode;

public static class Heap
{
    /**
        Given an integer array nums and an integer k, return the k-th largest element in the array.
        Note that it is the kth largest element in the sorted order, not the k-th distinct element.
        Solution https://leetcode.com/problems/kth-largest-element-in-an-array/solutions/6150225/c-solution-minheap-by-victornovik-oktd/
        # Approach
            After passing all `nums` elements through the k-sized `minHeap` there will stay `k` largest elements in the heap.
            The smallest remaining element among the `k` heap elements is the k-th largest one.
        # Complexity
            - Time complexity: O(n * log(k))
            - Space complexity: O(k)
     */
    public static int FindKthLargest(int[] nums, int k)
    {
        var minHeap = new PriorityQueue<int, int>(k + 1);
        foreach (var n in nums)
        {
            minHeap.Enqueue(n, n);
            if (minHeap.Count > k)
                minHeap.Dequeue();
        }
        return minHeap.Peek();
    }

    /**
        You are given an array of k linked-lists lists, each linked-list is sorted in ascending order.
        Merge all the linked-lists into one sorted linked-list and return it.
        Solution https://leetcode.com/problems/merge-k-sorted-lists/solutions/6149754/c-solution-with-min-heap-by-victornovik-f7in
        # Approach
            Introduce `minHeap` as the middleware buffer and enqueue 1 node from every `k` lists.
            `minHeap` will serialize the smallest nodes from all `k` lists into one resulting linked list.
            Dequeue the smallest node from `minHeap` (the node value is used as a priority) and append it to the result list.
            Take the next node from the list, where the dequeued node came from, and enqueue it to `minHeap`.
            Do it until all `k` lists are done.
        # Complexity
            - Time complexity: O(n * log(k)) where `n` is total length of all `k` lists
            - Space complexity: O(k)
    */
    public static ListNode? MergeKLists(ListNode?[] lists)
    {
        var minHeap = new PriorityQueue<ListNode, int>(lists.Length);
        foreach (var root in lists)
        {
            if (root != null)
                minHeap.Enqueue(root, root.val);
        }

        var head = new ListNode();
        var cur = head;
        while (minHeap.Count > 0)
        {
            var min = minHeap.Dequeue();
            cur.next = min;
            cur = min;

            var next = min.next;
            if (next != null)
                minHeap.Enqueue(next, next.val);
        }
        return head.next;
    }

    /**
        Given an array of integers nums, sort the array in ascending order and return it.
        You must solve the problem without using any built-in functions in O(n * log(n)) time complexity and with the smallest space complexity possible.
        Solution https://leetcode.com/problems/sort-an-array/solutions/6167114/c-solution-minheap-by-victornovik-k5tm
        Complexity
            Time complexity: O(n ∗ log(n))
            Space complexity: O(n)
    */
    public static int[] SortArray(int[] nums)
    {
        var minHeap = new PriorityQueue<int, int>();
        foreach (var n in nums)
            minHeap.Enqueue(n, n);

        for (var i = 0; i < nums.Length; i++)
            nums[i] = minHeap.Dequeue();

        return nums;
    }

    /**
        A string s is called happy if it satisfies the following conditions:
            s only contains the letters 'a', 'b', and 'c'.
            s does not contain any of "aaa", "bbb", or "ccc" as a substring.
            s contains at most a occurrences of the letter 'a'.
            s contains at most b occurrences of the letter 'b'.
            s contains at most c occurrences of the letter 'c'.
        Given 3 integers a, b, and c, return the longest possible happy string.
        If there are multiple longest happy strings, return any of them.
        If there is no such string, return the empty string "".
        A substring is a contiguous sequence of characters within a string.
        Solution https://leetcode.com/problems/longest-happy-string
        # Complexity
            - Time complexity: O(n * log(n))
            - Space complexity: O(n)        
     */
    public static string LongestDiverseString_MaxHeap(int a, int b, int c)
    {
        var queue = new PriorityQueue<char, int>(Comparer<int>.Create((x, y) => y - x));
        queue.EnqueueRange([('a', a), ('b', b), ('c', c)]);

        var res = new StringBuilder();
        while (TryAppendNext());
        return res.ToString();

        bool TryAppendNext()
        {
            if (!queue.TryDequeue(out var ch, out var count) || count == 0)
                return false;

            if (res is [.., var x, var y] && ch == x && ch == y && !TryAppendNext())
                return false;

            queue.Enqueue(ch, --count);
            res.Append(ch);
            return true;
        }
    }
}