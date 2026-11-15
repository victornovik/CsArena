namespace CsArena.Tests.LeetCode;

/**
    Design a stack that supports push, pop, top, and retrieving the minimum element in constant time.
    You must implement a solution with O(1) time complexity for each function.
    Solution https://leetcode.com/problems/min-stack/solutions/5859611/simple-c-solution-by-victornovik-0izd
    # Approach
        - I remember only corner minimums of subranges in internal stack `min`. 
            When we push [**5**, 6, 7, 8, 9, 10, **4**, 5, 6, 7, 8, 9, 10] we remember just two minimums **5** and **4**. 
        - If we push a value equal or less than a current minimum then push that value to `min` stack and it will become the new current minimum. 
        - If we pop a value equal to the current minimum then pop this minimum as well and the next element in `min` stack will become the new minimum.

    # Complexity
        - Time complexity: O(1)
        - Space complexity: O(1)
*/
public class StackWithMin
{
    public void Push(int val)
    {
        if (min.Count == 0 || val <= min.Peek())
            min.Push(val);
        stk.Push(val);
    }

    public void Pop()
    {
        if (stk.Pop() == min.Peek())
            min.Pop();
    }

    public int Top() => stk.Peek();

    public int GetMin() => min.Peek();

    private readonly Stack<int> stk = new();
    private readonly Stack<int> min = new();
}
