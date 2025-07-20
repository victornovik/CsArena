using System.Numerics;

namespace CsArena.Tests;

public class MathTests
{
    [Fact]
    public void BitOperationsTest()
    {
        Assert.Equal(3, BitOperations.Log2(8));
    }

    [Fact]
    public void DivRemTest()
    {
        var(quotient, remainder) = Math.DivRem(10, 3);
        Assert.Equal(3, quotient);
        Assert.Equal(1, remainder);

        (quotient, remainder) = Math.DivRem(25, 5);
        Assert.Equal(5,quotient);
        Assert.Equal(0,remainder);

        (quotient, remainder) = Math.DivRem(-10, 3);
        Assert.Equal(-3, quotient);
        Assert.Equal(-1, remainder);
    }
}