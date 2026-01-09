using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace CsArena.Tests;

public static class CSharp14Feature
{
    // The members in the first extension block are called as though they're instance members of any number
    extension<T>(T i) where T : INumber<T>
    {
        public T Square() => i * i;
        public T Cube() => i * i * i;
    }

    // The members in the second extension block are called as though they're static members of any number
    extension<T>(T) where T : INumber<T>
    {
        public static T Pow2(T i) => i * i;
        public static T Pow3(T i) => i * i * i;
    }
}

public class CSharp14Tests
{
    [Fact]
    public void NullConditionalAssignmentTest()
    {
        string Rhs()
        {
            Assert.True(false);
            return "NullConditionalAssignment";
        }

        // If book is null, the code doesn't call the assignment and RHS is not evaluated
        InMemoryBook? book = null;
        book?.Name = Rhs();
        Assert.Null(book);
    }

    [Fact]
    public void ExtensionBlockTest()
    {
        Assert.Equal(9, 3.Square());
        Assert.Equal(27, 3.Cube());
        Assert.Equal(9.0f, 3.0f.Square());
        Assert.Equal(9.0d, 3.0d.Square());
        Assert.Equal(9.0m, 3.0m.Square());
        Assert.Equal(25u, 5u.Square());
        Assert.Equal(125u, 5u.Cube());

        Assert.Equal(9, int.Pow2(3));
        Assert.Equal(27, int.Pow3(3));
    }
}