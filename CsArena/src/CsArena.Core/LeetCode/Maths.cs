namespace CsArena.Tests.LeetCode;

public static class Maths
{
    public static int Fac(int n)
    {
        var res = 1;
        for (var i = res; i <= n; i++)
            res *= i;
        return res;
    }

    public static long Fac(long n)
    {
        var res = 1L;
        for (var i = res; i <= n; i++)
            res *= i;
        return res;
    }

    public static ulong Fac(ulong n)
    {
        var res = 1ul;
        for (var i = res; i <= n; i++)
            res *= i;
        return res;
    }

    /**
        Greatest common divisor https://en.wikipedia.org/wiki/Euclidean_algorithm
        `x` and `y` must be positive.
    */
    public static long Gcd(long x, long y)
    {
        while (x != 0 && y != 0)
        {
            if (x > y)
                x %= y;
            else
                y %= x;
        }
        return x | y;
    }

    /**
        Least common multiple
    */
    public static long Lcm(long x, long y) => x * y / Gcd(x, y);

    /**
        Given a non-negative integer x, return the square root of x rounded down to the nearest integer. 
        The returned integer should be non-negative as well.
        You must not use any built-in exponent function or operator.
        # Approach
            We have to find the minimal number n such that n^2 > x. In such case n - 1 will be the answer.
        Solution https://leetcode.com/problems/sqrtx/solutions/6240038/c-solution-beats-100-by-victornovik-fa1y
        # Complexity
            - Time complexity: O(log(n))
            - Space complexity: O(1)
     */
    public static int MySqrt(int x)
    {
        long first = 1, last = x;
        while (first <= last)
        {
            var mid = first + (last - first) / 2;
            var pow2 = mid * mid;
            if (pow2 == x)
                return (int)mid;

            if (pow2 < x)
                first = mid + 1;
            else
                last = mid - 1;
        }
        return (int)(first - 1);
    }

    /**
        Given a non-negative integer x, return the square root of x rounded down to the nearest integer.
        The returned integer should be non-negative as well.
        You must not use any built-in exponent function or operator.
        # Approach
            https://en.wikipedia.org/wiki/Integer_square_root#Algorithm_using_Newton's_method
        Solution https://leetcode.com/problems/sqrtx/solutions/6240137/c-newtons-method-beats-100-by-victornovi-szdl
        # Complexity
            - Time complexity: O(log(n))
            - Space complexity: O(1)
     */
    public static int MySqrt_Newton(int x)
    {
        long root = x;
        while (root * root > x)
            root = (root + x / root) / 2;
        return (int)root;
    }
}