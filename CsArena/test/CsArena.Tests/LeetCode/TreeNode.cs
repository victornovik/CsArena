namespace CsArena.Tests.LeetCode;

public class TreeNode
{
    public int val;
    public TreeNode? left;
    public TreeNode? right;

    public TreeNode(int v = 0, TreeNode? l = null, TreeNode? r = null)
    {
        val = v;
        left = l;
        right = r;
    }
    
    public TreeNode(int?[] arr, int index = 0) => FromArray(this, arr, index);

    public void FromArray(TreeNode root, int?[] arr, int index)
    {
        if (arr[index] != null)
            val = arr[index]!.Value;

        var leftIndex = index * 2 + 1;
        if (leftIndex < arr.Length && arr[leftIndex] != null)
            left = new TreeNode(arr, leftIndex);

        var rightIndex = index * 2 + 2;
        if (rightIndex < arr.Length && arr[rightIndex] != null)
            right = new TreeNode(arr, rightIndex);
    }

    /**
        Create binary tree from its array representation.
        The children of array element with index `i` are stored at `2i + 1` and `2i + 2` indices
    */
    public static TreeNode? FromArray(int?[] arr)
    {
        return arr.Length > 0 ? new TreeNode(arr) : null;
    }

    /**
        Export binary tree to an array.
        The children of array element with index `i` are stored at `2i + 1` and `2i + 2` indices
    */
    public int?[] ToArray()
    {
        var queue = new Queue<TreeNode?>();
        queue.Enqueue(this);

        var res = new List<int?>();
        for (var treeLevelSize = queue.Count; treeLevelSize > 0; treeLevelSize = queue.Count)
        {
            for (var i = 0; i < treeLevelSize; i++)
            {
                var cur = queue.Dequeue();
                res.Add(cur?.val);

                if (cur != null)
                    queue.Enqueue(cur.left);
                if (cur != null)
                    queue.Enqueue(cur.right);
            }
        }
        // Remove all trailing nulls
        return res.Take(res.FindLastIndex(e => e != null) + 1).ToArray();
        //return ((IEnumerable<int?>)res).Reverse().SkipWhile(e => e == null).Reverse().ToArray();
    }
}