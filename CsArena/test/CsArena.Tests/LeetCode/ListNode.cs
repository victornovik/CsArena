namespace CsArena.Tests.LeetCode;

public class ListNode(int v = 0, ListNode? n = null)
{
    public int val = v;
    public ListNode? next = n;

    public List<int> ToList()
    {
        var list = new List<int>();
        for (var current = this; current != null; current = current.next)
        {
            list.Add(current.val);
        }
        return list;
    }

    public int Count()
    {
        var count = 0;
        for (var cur = this; cur != null; cur = cur.next)
        {
            ++count;
        }
        return count;
    }
}