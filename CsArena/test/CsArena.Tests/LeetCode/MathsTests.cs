namespace CsArena.Tests.LeetCode;

using static Maths;

public class MathsTests
{
    [Fact]
    public void FactorialTest()
    {
        Assert.True(2 == Fac(2));
        Assert.True(6 == Fac(3));
        Assert.True(24 == Fac(4));
        Assert.True(120 == Fac(5));
    }

    [Fact]
    public void GreatestCommonDivisorTest()
    {
        Assert.Equal(5, Gcd(55, 25));
        Assert.Equal(12, Gcd(48, 12));
        Assert.Equal(6, Gcd(48, 18));
        Assert.Equal(1, Gcd(19, 37));
    }

    [Fact]
    public void LeastCommonMultipleTest()
    {
        Assert.Equal(110, Lcm(55, 110));
        Assert.Equal(77, Lcm(7, 11));
        Assert.Equal(20, Lcm(4, 5));
        Assert.Equal(36, Lcm(12, 18));
    }

    [Fact]
    public void MySqrtTest()
    {
        Assert.Equal(1, MySqrt(1));
        Assert.Equal(2, MySqrt(4));
        Assert.Equal(2, MySqrt(8));
        Assert.Equal(46339, MySqrt(2_147_395_599));

        Assert.Equal(1, MySqrt_Newton(1));
        Assert.Equal(2, MySqrt_Newton(4));
        Assert.Equal(2, MySqrt_Newton(8));
        Assert.Equal(46339, MySqrt_Newton(2_147_395_599));
        Assert.Equal(46340, MySqrt_Newton(2_147_483_647));
    }
}