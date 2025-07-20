namespace CsArena.Tests.LeetCode;

using static Trees;

public class TreesTests
{
    [Fact]
    public void TreeMaxDepthDFSTest()
    {
        var root = TreeNode.FromArray([3, 9, 20, null, null, 15, 7]);
        Assert.Equal(3, MaxDepth_DFS(root));

        root = TreeNode.FromArray([1, null, 2]);
        Assert.Equal(2, MaxDepth_DFS(root));

        root = TreeNode.FromArray([]);
        Assert.Equal(0, MaxDepth_DFS(root));

        root = TreeNode.FromArray([0, 0, 0, 0, 0, null, null, null, null, null, 0]);
        Assert.Equal(4, MaxDepth_DFS(root));
    }

    [Fact]
    public void TreeMaxDepthBFSTest()
    {
        var root = TreeNode.FromArray([3, 9, 20, null, null, 15, 7]);
        Assert.Equal(3, MaxDepth_BFS(root));

        root = TreeNode.FromArray([1, null, 2]);
        Assert.Equal(2, MaxDepth_BFS(root));

        root = TreeNode.FromArray([]);
        Assert.Equal(0, MaxDepth_BFS(root));

        root = TreeNode.FromArray([0, 0, 0, 0, 0, null, null, null, null, null, 0]);
        Assert.Equal(4, MaxDepth_BFS(root));
    }

    [Fact]
    public void IsSameTreeTest()
    {
        var p = TreeNode.FromArray([1, 2, 3]);
        var q = TreeNode.FromArray([1, 2, 3]);
        Assert.True(IsSameTree(p, q));

        p = TreeNode.FromArray([1, 2]);
        q = TreeNode.FromArray([1, null, 2]);
        Assert.False(IsSameTree(p, q));

        p = TreeNode.FromArray([1, 2, 1]);
        q = TreeNode.FromArray([1, 1, 2]);
        Assert.False(IsSameTree(p, q));

        p = TreeNode.FromArray([1]);
        q = TreeNode.FromArray([1, null, 2]);
        Assert.False(IsSameTree(p, q));
    }

    [Fact]
    public void CountNodesInCompleteTreeTest()
    {
        var root = TreeNode.FromArray([1, 2, 3, 4, 5, 6]);
        Assert.Equal(6, CountNodes_DFS(root));

        root = TreeNode.FromArray([]);
        Assert.Equal(0, CountNodes_DFS(root));

        root = TreeNode.FromArray([1]);
        Assert.Equal(1, CountNodes_DFS(root));

        root = TreeNode.FromArray([1, 2, 3, 4, 5, 6]);
        Assert.Equal(6, CountNodes(root));

        root = TreeNode.FromArray([]);
        Assert.Equal(0, CountNodes(root));

        root = TreeNode.FromArray([1]);
        Assert.Equal(1, CountNodes(root));
    }

    [Fact]
    public void AverageOfLevelsTest()
    {
        var root = TreeNode.FromArray([3, 9, 20, null, null, 15, 7]);
        Assert.Equal([3.00000, 14.50000, 11.00000], AverageOfLevels(root));

        root = TreeNode.FromArray([3, 9, 20, 15, 7]);
        Assert.Equal([3.00000, 14.50000, 11.00000], AverageOfLevels(root));
    }

    [Fact]
    public void GetMinimumDifferenceTest()
    {
        var root = TreeNode.FromArray([4, 2, 6, 1, 3]);
        Assert.Equal(1, GetMinimumDifference(root));

        root = TreeNode.FromArray([1, 0, 48, null, null, 12, 49]);
        Assert.Equal(1, GetMinimumDifference(root));

        root = TreeNode.FromArray([236, 104, 701, null, 227, null, 911]);
        Assert.Equal(9, GetMinimumDifference(root));

        root = TreeNode.FromArray([1, 1, 1, 1, 1]);
        Assert.Equal(0, GetMinimumDifference(root));
    }

    [Fact]
    public void KthSmallestTest()
    {
        var root = TreeNode.FromArray([3, 1, 4, null, 2]);
        Assert.Equal(1, KthSmallest(root!, 1));

        root = TreeNode.FromArray([5, 3, 6, 2, 4, null, null, 1]);
        Assert.Equal(3, KthSmallest(root!, 3));
    }

    [Fact]
    public void HasPathSumTest()
    {
        var root = TreeNode.FromArray([5, 4, 8, 11, null, 13, 4, 7, 2, null, null, null, 1]);
        Assert.True(HasPathSum(root, 22));

        root = TreeNode.FromArray([1, 2, 3]);
        Assert.False(HasPathSum(root, 5));

        root = TreeNode.FromArray([]);
        Assert.False(HasPathSum(root, 0));
    }

    [Fact]
    public void SumNumbersTest()
    {
        var root = TreeNode.FromArray([1, 2, 3]);
        Assert.Equal(25, SumNumbers(root));

        root = TreeNode.FromArray([4, 9, 0, 5, 1]);
        Assert.Equal(1026, SumNumbers(root));

        root = TreeNode.FromArray([1, 0]);
        Assert.Equal(10, SumNumbers(root));
    }

    [Fact]
    public void IsValidBSTTest()
    {
        var root = TreeNode.FromArray([2, 1, 3]);
        Assert.True(IsValidBST(root));

        root = TreeNode.FromArray([5, 1, 4, null, null, 3, 6]);
        Assert.False(IsValidBST(root));

        root = TreeNode.FromArray([2, 2, 2]);
        Assert.False(IsValidBST(root));

        root = TreeNode.FromArray([int.MaxValue]);
        Assert.True(IsValidBST(root));

        root = TreeNode.FromArray([int.MaxValue, int.MaxValue]);
        Assert.False(IsValidBST(root));
    }

    [Fact]
    public void LevelOrderTest()
    {
        var root = TreeNode.FromArray([3, 9, 20, null, null, 15, 7]);
        Assert.Equal([[3], [9, 20], [15, 7]], LevelOrder(root));

        root = TreeNode.FromArray([1]);
        Assert.Equal([[1]], LevelOrder(root));

        root = TreeNode.FromArray([]);
        Assert.Equal([], LevelOrder(root));
    }
    
    [Fact]
    public void SortedArrayToBSTTest()
    {
        var tree = SortedArrayToBST([-10, -3, 0, 5, 9]);
        Assert.Equal([0, -10, 5, null, -3, null, 9], tree!.ToArray());

        tree = SortedArrayToBST([1, 3]);
        Assert.Equal([1, null, 3], tree!.ToArray());
    }
}