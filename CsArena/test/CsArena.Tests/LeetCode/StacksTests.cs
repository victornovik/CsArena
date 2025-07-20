namespace CsArena.Tests.LeetCode;

using static Stacks;

public class StacksTests
{
    [Fact]
    public void MinStackTest()
    {
        var stk = new StackWithMin();
        stk.Push(-2);
        stk.Push(0);
        stk.Push(-3);
        Assert.Equal(-3, stk.GetMin());
        stk.Pop();
        Assert.Equal(0, stk.Top());
        Assert.Equal(-2, stk.GetMin());

        stk = new StackWithMin();
        stk.Push(0);
        stk.Push(1);
        stk.Push(0);
        Assert.Equal(0, stk.GetMin());
        stk.Pop();
        Assert.Equal(0, stk.GetMin());

        stk = new StackWithMin();
        stk.Push(5);
        stk.Push(6);
        stk.Push(7);
        stk.Push(8);
        stk.Push(9);
        stk.Push(10);
        stk.Push(4);
        stk.Push(5);
        stk.Push(6);
        stk.Push(7);
        stk.Push(8);
        stk.Push(9);
        stk.Push(10);
        Assert.Equal(4, stk.GetMin());
    }

    [Fact]
    public void IsValidParenthesesTest()
    {
        Assert.True(IsValidParentheses_FrozenDictionary("(())"));
        Assert.True(IsValidParentheses_FrozenDictionary("()()"));
        Assert.True(IsValidParentheses_FrozenDictionary("()"));
        Assert.True(IsValidParentheses_FrozenDictionary("()[]{}"));
        Assert.False(IsValidParentheses_FrozenDictionary("(]"));
        Assert.True(IsValidParentheses_FrozenDictionary("([])"));
        Assert.False(IsValidParentheses_FrozenDictionary("]"));
        Assert.False(IsValidParentheses_FrozenDictionary("((((("));

        Assert.True(IsValidParentheses_Straight("()"));
        Assert.True(IsValidParentheses_Straight("()[]{}"));
        Assert.False(IsValidParentheses_Straight("(]"));
        Assert.True(IsValidParentheses_Straight("([])"));
        Assert.False(IsValidParentheses_Straight("]"));
        Assert.False(IsValidParentheses_Straight("((((("));
    }
}