namespace CsArena.Tests.LeetCode;

public static class Trees
{
    /**
        Given the root of a binary tree, return its maximum depth.
        A binary tree's maximum depth is the number of nodes along the longest path from the root node down to the farthest leaf node.
        Solution https://leetcode.com/problems/maximum-depth-of-binary-tree/solutions/6160126/c-solution-beats-100-by-victornovik-7ti5
        Approach
            DFS
        Complexity
            - Time complexity: O(n), where n - number of nodes
            - Space complexity: O(n), where n - number of nodes
    */
    public static int MaxDepth_DFS(TreeNode? root)
    {
        if (root == null)
            return 0;

        var leftDepth = MaxDepth_DFS(root.left);
        var rightDepth = MaxDepth_DFS(root.right);
        return int.Max(leftDepth, rightDepth) + 1;
    }

    /**
        Given the root of a binary tree, return its maximum depth.
        A binary tree's maximum depth is the number of nodes along the longest path from the root node down to the farthest leaf node.
        Approach
            BFS
        Complexity
            - Time complexity: O(n), where n - number of nodes
            - Space complexity: O(n), where n - number of nodes
    */
    public static int MaxDepth_BFS(TreeNode? root)
    {
        if (root == null)
            return 0;

        var queue = new Queue<TreeNode>([root]);
        var depth = 0;
        for (var treeLevelSize = queue.Count; treeLevelSize > 0; treeLevelSize = queue.Count)
        {
            for (var i = 0; i < treeLevelSize; i++)
            {
                var cur = queue.Dequeue();
                if (cur.left != null)
                    queue.Enqueue(cur.left);
                if (cur.right != null)
                    queue.Enqueue(cur.right);
            }
            depth++;
        }
        return depth;
    }

    /**
        Given the roots of two binary trees p and q, write a function to check if they are the same or not.
        Two binary trees are considered the same if they are structurally identical, and the nodes have the same value. 
        Solution https://leetcode.com/problems/same-tree/solutions/6164757/c-solution-beats-100-by-victornovik-8p9e
        # Approach
            DFS
        # Complexity
            - Time complexity: O(p + q)
            - Space complexity: O(p + q)
    */
    public static bool IsSameTree(TreeNode? p, TreeNode? q)
    {
        if (p == null && q == null)
            return true;
        if ((p != null) != (q != null) || p!.val != q!.val)
            return false;

        return IsSameTree(p.left, q.left) && IsSameTree(p.right, q.right);
    }

    /**
        Given the root of a complete binary tree, return the number of the nodes in the tree.
        According to https://en.wikipedia.org/wiki/Binary_tree#Types_of_binary_trees every level, except possibly the last, is completely filled
            and all nodes in the last level are as far left as possible.
        It can have between 1 and 2^h nodes inclusive at the last level h.
        Design an algorithm that runs in less than O(n) time complexity.
        Solution https://leetcode.com/problems/count-complete-tree-nodes/solutions/6165129/c-dfs-solution-beats-100-by-victornovik-izdm
        # Complexity
            - Time complexity: O(log(n) * log(n))
            - Space complexity: O(log(n))
    */
    public static int CountNodes(TreeNode? root)
    {
        if (root == null)
            return 0;

        var depth = 0;
        TreeNode? left = root, right = root;
        while (right != null)
        {
            left = left!.left;
            right = right.right;
            depth++;
        }

        // It is a perfect tree i.e. every interior node has strictly two children
        if (left == null)
            return (int)Math.Pow(2, depth) - 1;

        // Otherwise resort to recursive calls on the left and the right subtrees
        return CountNodes(root.left) + CountNodes(root.right) + 1;
    }


    /**
        Given the root of a complete binary tree, return the number of the nodes in the tree.        
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(h)
    */
    public static int CountNodes_DFS(TreeNode? root)
    {
        if (root == null)
            return 0;

        var leftNodes = CountNodes_DFS(root.left);
        var rightNodes = CountNodes_DFS(root.right);
        return leftNodes + rightNodes + 1;
    }

    /**
        Given the root of a binary tree, return the average value of the nodes on each level in an array.
        Solution https://leetcode.com/problems/average-of-levels-in-binary-tree/solutions/6165194/c-solution-beats-82-by-victornovik-26mu
        # Approach
            BFS as we have to calculate smth per tree level.
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(log(n))
    */
    public static IList<double> AverageOfLevels(TreeNode? root)
    {
        if (root == null)
            return [];

        var res = new List<double>();
        var queue = new Queue<TreeNode>([root]);
        for (var treeLevelSize = queue.Count; treeLevelSize > 0; treeLevelSize = queue.Count)
        {
            var sum = 0.0;
            for (var i = 0; i < treeLevelSize; i++)
            {
                var cur = queue.Dequeue();
                if (cur.left != null)
                    queue.Enqueue(cur.left);
                if (cur.right != null)
                    queue.Enqueue(cur.right);
                sum += cur.val;
            }
            res.Add(sum / treeLevelSize);
        }
        return res;
    }

    /**
        Given the root of a Binary Search Tree (BST), return the minimum absolute difference between the values of any two different nodes in the tree.
        Solution https://leetcode.com/problems/minimum-absolute-difference-in-bst/solutions/6167847/c-solution-beats-100-by-victornovik-qh3d
        # Approach
            Go through BST in ascending order (in-order traversal) so every current node is greater or equal to the previous node. 
            If we've found two nodes are equal (zero difference) immediately stop the algorithm.
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int GetMinimumDifference(TreeNode? root)
    {
        int? prev = null;
        var minDiff = int.MaxValue;
        if (root != null)
            inOrder(root);
        return minDiff;

        void inOrder(TreeNode cur)
        {
            if (cur.left != null)
                inOrder(cur.left);
            
            if (prev != null)
            {
                // Current node is greater or equal to the previous node as we're going through BST in ascending order
                minDiff = int.Min(minDiff, cur.val - prev.Value);
                if (minDiff == 0)
                    return; // We immediately stop as we've already found the minimal possible difference
            }
            prev = cur.val;

            if (cur.right != null)
                inOrder(cur.right);
        }
    }

    /**
        Given the root of a binary search tree, and an integer k, return the k-th smallest value (1-indexed) of all the values of the nodes in the tree.        
        Solution https://leetcode.com/problems/kth-smallest-element-in-a-bst/solutions/6168296/c-solutuion-beats-100-by-victornovik-0rv3
        # Approach
            Go through BST in ascending order and count every passed node. When we've reached the k-th node immediately break the traversal.
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static int KthSmallest(TreeNode root, int k)
    {
        var kth = -1;
        var cnt = 0;
        inOrder(root, ref cnt);
        return kth;

        void inOrder(TreeNode cur, ref int count)
        {
            if (cur.left != null && kth == -1)
                inOrder(cur.left, ref count);

            if (kth == -1 && ++count == k)
            {
                kth = cur.val;
                return; // We immediately stop as we've just found k-th smallest node
            }

            if (cur.right != null && kth == -1)
                inOrder(cur.right, ref count);
        }
    }

    /**
        Given the root of a binary tree and an integer targetSum, return true if the tree has a root-to-leaf path such that adding up all the values along the path equals targetSum.
        Solution https://leetcode.com/problems/path-sum/solutions/6170306/c-solution-beats-100-by-victornovik-pdzy/
        # Approach
            Use DFS and subtract current node value from `targetSum`. 
            If we're in the leaf and the subtraction result is `0` then a root-to-leaf path takes exactly `targetSum`.
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(1)
    */
    public static bool HasPathSum(TreeNode? root, int targetSum)
    {
        if (root == null)
            return false;

        var remaining = targetSum - root.val;
        if (root.left == null && root.right == null && remaining == 0)
            return true;

        return HasPathSum(root.left, remaining) || HasPathSum(root.right, remaining);
    }

    /**
        You are given the root of a binary tree containing digits from 0 to 9 only.
        Each root-to-leaf path in the tree represents a number.
        For example, the root-to-leaf path 1 -> 2 -> 3 represents the number 123.
        Return the total sum of all root-to-leaf numbers. Test cases are generated so that the answer will fit in a 32-bit integer.
        Solution https://leetcode.com/problems/sum-root-to-leaf-numbers/solutions/6170405/c-solution-beats-100-by-victornovik-qgut
    */
    public static int SumNumbers(TreeNode? root)
    {
        return DFS(root!, 0);

        int DFS(TreeNode node, int prevTotal)
        {
            var cur = prevTotal * 10 + node.val;
            if (node.left == null && node.right == null)
                return cur;

            var sum = 0;
            if (node.left != null)
                sum += DFS(node.left, cur);
            if (node.right != null)
                sum += DFS(node.right, cur);
            return sum;
        }
    }

    /**
        Given the root of a binary tree, return the level order traversal of its nodes' values.
        I.e. from left to right, level by level.
        Solution https://leetcode.com/problems/binary-tree-level-order-traversal/solutions/6173335/c-solution-by-victornovik-v13b
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
    */
    public static IList<IList<int>> LevelOrder(TreeNode? root)
    {
        if (root == null)
            return [];

        var queue = new Queue<TreeNode>();
        queue.Enqueue(root);

        IList<IList<int>> res = [];
        for (var treeLevelSize = queue.Count; treeLevelSize > 0; treeLevelSize = queue.Count)
        {
            var level = new List<int>();
            for (var i = 0; i < treeLevelSize; i++)
            {
                var cur = queue.Dequeue();
                level.Add(cur.val);
                if (cur.left != null)
                    queue.Enqueue(cur.left);
                if (cur.right != null)
                    queue.Enqueue(cur.right);
            }
            res.Add(level);
        }
        return res;
    }

    /**
       Given the root of a binary tree, determine if it is a valid binary search tree (BST).
       A valid BST is defined as follows:
       - The left subtree of a node contains only nodes with keys STRICTLY less than the node's key.
       - The right subtree of a node contains only nodes with keys STRICTLY greater than the node's key.
       - Both the left and right subtrees must also be binary search trees.
       Solution https://leetcode.com/problems/validate-binary-search-tree/solutions/6170471/c-solution-beats-100-by-victornovik-jnhh
   */
    public static bool IsValidBST(TreeNode? root, int? min = null, int? max = null)
    {
        if (root == null)
            return true;
        if ((min.HasValue && root.val <= min) || (max.HasValue && root.val >= max))
            return false;

        return IsValidBST(root.left, min, root.val) && IsValidBST(root.right, root.val, max);
    }

    /**
        Given an integer array nums where the elements are sorted in ascending order, convert it to a height-balanced binary search tree.
        Solution https://leetcode.com/problems/convert-sorted-array-to-binary-search-tree/solutions/6178381/c-solution-beats-100-by-victornovik-44y4
        # Complexity
            - Time complexity: O(n)
            - Space complexity: O(n)
    */
    public static TreeNode? SortedArrayToBST(int[] nums)
    {
        return CreateNode(nums, 0, nums.Length - 1);

        TreeNode? CreateNode(int[] arr, int left, int right)
        {
            if (left > right)
                return null;

            var mid = left + (right - left) / 2;
            var root = new TreeNode(arr[mid])
            {
                left = CreateNode(arr, left, mid - 1),
                right = CreateNode(arr, mid + 1, right)
            };
            return root;
        }
    }
}