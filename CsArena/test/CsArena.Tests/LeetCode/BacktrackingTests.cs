namespace CsArena.Tests.LeetCode;

using static Backtracking;

public class BacktrackingTests
{
    [Fact]
    public void GenerateParenthesesTest()
    {
        Assert.Equal(["((()))", "(()())", "(())()", "()(())", "()()()"], GenerateParentheses(3));
        Assert.Equal(["(())", "()()"], GenerateParentheses(2));
        Assert.Equal(["()"], GenerateParentheses(1));
    }
    
    [Fact]
    public void SubsetsTest()
    {
        Assert.Equal([[], [1], [1, 2], [2]], Subsets([1, 2]));
        Assert.Equal([[], [1], [1, 2], [1, 2, 3], [1, 3], [2], [2, 3], [3]], Subsets([1, 2, 3]));
        Assert.Equal([[], [1]], Subsets([1]));
    }
}