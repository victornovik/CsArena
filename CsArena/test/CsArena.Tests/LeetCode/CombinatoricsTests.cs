namespace CsArena.Tests.LeetCode;

using static Combinatorics;

public class CombinatoricsTests
{
    [Fact]
    public void PermutationsTest()
    {
        Assert.Equal(24, P(4));
        Assert.Equal(120, P(5));
        Assert.Equal(720, P(6));
        Assert.Equal(5040, P(7));
        Assert.Equal(40320, P(8));
        Assert.Equal(362_880, P(9));
        Assert.Equal(3_628_800, P(10));
    }

    [Fact]
    public void AllocationsTest()
    {
        Assert.Equal(60, A(5,3));
        Assert.Equal(12, A(4,2));
        Assert.Equal(4896, A(18,3));
    }

    [Fact]
    public void CombinationsTest()
    {
        Assert.Equal(1, C(0, 0));
        Assert.Equal(10, C(5,2));
        Assert.Equal(10, C(5,3));
        Assert.Equal(126, C(9,4));
        Assert.Equal(28, C(8,2));
        Assert.Equal(1, C(3,0));
        Assert.Equal(3, C(3,1));
        Assert.Equal(3, C(3,2));
        Assert.Equal(1, C(3,3));
        Assert.Equal(120, C(16, 2));
        Assert.Equal(19_600, C(50, 3));
    }

    [Fact]
    public void BuildArrayFromPermutationTest()
    {
        Assert.Equal([0, 1, 2, 4, 5, 3], BuildArrayFromPermutation([0, 2, 1, 5, 3, 4]));
        Assert.Equal([4, 5, 0, 1, 2, 3], BuildArrayFromPermutation([5, 0, 1, 2, 3, 4]));
    }

    [Fact]
    public void NextPermutationTest()
    {
        int[] arr = [1, 2, 3];
        NextPermutation(arr);
        Assert.Equal([1, 3, 2], arr);

        arr = [3, 2, 1];
        NextPermutation(arr);
        Assert.Equal([1, 2, 3], arr);

        arr = [1, 3, 2];
        NextPermutation(arr);
        Assert.Equal([2, 1, 3], arr);

        arr = [1, 1, 5];
        NextPermutation(arr);
        Assert.Equal([1, 5, 1], arr);

        arr = [1, 5, 1];
        NextPermutation(arr);
        Assert.Equal([5, 1, 1], arr);

        arr = [1, 1, 1];
        NextPermutation(arr);
        Assert.Equal([1, 1, 1], arr);
    }

    [Fact]
    public void FindAllPermutationsTest()
    {
        Assert.Equal([[1, 3, 2], [2, 1, 3], [2, 3, 1], [3, 1, 2], [3, 2, 1], [1, 2, 3]], FindAllPermutations([1, 2, 3]));
        Assert.Equal([[1, 0], [0, 1]], FindAllPermutations([0, 1]));
        Assert.Equal([[1]], FindAllPermutations([1]));

        Assert.Equal([[1, 2, 3], [1, 3, 2], [2, 1, 3], [2, 3, 1], [3, 2, 1], [3, 1, 2]], FindAllPermutations_Backtracking([1, 2, 3]));
        Assert.Equal([[0, 1], [1, 0]], FindAllPermutations_Backtracking([0, 1]));
        Assert.Equal([[1]], FindAllPermutations_Backtracking([1]));
    }

    [Fact]
    public void FindAllCombinationsTest()
    {
        Assert.Equal([[1, 2], [1, 3], [1, 4], [2, 3], [2, 4], [3, 4]], FindAllCombinations(4,2));
        Assert.Equal([[1]], FindAllCombinations(1,1));

        Assert.Equal([[1, 2], [1, 3], [1, 4], [2, 3], [2, 4], [3, 4]], FindAllCombinations_Backtracking(4, 2));
        Assert.Equal([[1]], FindAllCombinations_Backtracking(1, 1));
    }
}