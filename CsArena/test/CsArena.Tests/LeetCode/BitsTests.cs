namespace CsArena.Tests.LeetCode;

using static Bits;

public class BitsTests
{
    [Fact]
    public void HammingWeightTest()
    {
        Assert.Equal(3, HammingWeight(11));
        Assert.Equal(1, HammingWeight(128));
        Assert.Equal(30, HammingWeight(2147483645));
        Assert.Equal(1, HammingWeight(8));
    }

    [Fact]
    public void SingleNumberTest() 
    {
        Assert.Equal(1, SingleNumber_HashSet([2, 2, 1]));
        Assert.Equal(4, SingleNumber_HashSet([4, 1, 2, 1, 2]));
        Assert.Equal(1, SingleNumber_HashSet([1]));

        Assert.Equal(1, SingleNumber_Xor([2, 2, 1]));
        Assert.Equal(4, SingleNumber_Xor([4, 1, 2, 1, 2]));
        Assert.Equal(1, SingleNumber_Xor([1]));
    }
}