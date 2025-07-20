using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace CsArena.Tests;

public class TypeTests
{
    [Fact]
    [SuppressMessage("ReSharper", "IntVariableOverflowInUncheckedContext")]
    public void OverflowTest()
    {
        byte b = 255;
        b++;
        Assert.Equal(0, b);

        sbyte sb = 127;
        sb++;
        Assert.Equal(-128, sb);

        var i = int.MaxValue;
        i++;
        Assert.Equal(int.MinValue, i);

        var ui = uint.MaxValue;
        ui++;
        Assert.Equal(uint.MinValue, ui);
    }

    [Fact]
    [SuppressMessage("ReSharper", "IntVariableOverflowInUncheckedContext")]
    public void UnderflowTest()
    {
        byte b = 0;
        b--;
        Assert.Equal(255, b);

        sbyte sb = -128;
        sb--;
        Assert.Equal(127, sb);

        var i = int.MinValue;
        i--;
        Assert.Equal(int.MaxValue, i);

        var ui = uint.MinValue;
        ui--;
        Assert.Equal(uint.MaxValue, ui);
    }

    [Fact]
    public void ValueTypeByRef()
    {
        void ByRef(ref int inner) => inner = 45;

        var outer = 4;
        ByRef(ref outer);

        Assert.Equal(45, outer);
    }

    [Fact]
    public void TheSameVariableTwiceByRef()
    {
        void IncrAndDoubleByRef(ref int p1, ref int p2)
        {
            // Both p1 and p2 point to the same memory location
            p1++;
            p2 *= 2;
        }

        var outer = 5;
        IncrAndDoubleByRef(ref outer, ref outer);

        Assert.Equal(12, outer);
    }

    [Fact]
    public void ValueTypeByRefReadonly()
    {
        // Value type passed by reference but the callee cannot change it
        int ByRefReadonly(ref readonly int inner) => inner;

        var outer = 4;

        Assert.Equal(4, ByRefReadonly(ref outer));
        // warning CS9193: Argument 1 should be a variable because it is passed to a 'ref readonly' parameter
        // Assert.Equal(5, ByRefReadonly( 5));
    }

    [Fact]
    public void ValueTypeByIn()
    {
        // `in` is the same as `ref readonly`
        int PassByIn(in int inner) => inner;

        var outer = 4;

        Assert.Equal(4, PassByIn(in outer));
        // No warning CS9193. We can pass rvalue to the function
        Assert.Equal(5, PassByIn(5));
    }

    [Fact]
    public void ValueTypeByInPuzzler()
    {
        static void ByIn(in int p, Action action)
        {
            Assert.Equal(10, p);
            action();
            Assert.Equal(11, p);
        }

        static void ByVal(int p, Action action)
        {
            Assert.Equal(11, p);
            action();
            Assert.Equal(11, p);
        }

        var x = 10;
        ByIn(x, () => x++);
        ByVal(x, () => x++);
    }

    [Fact]
    public void RefTypeByOut()
    {
        void ByOut(out InMemoryBook res, string newName) => res = new(newName);

        ByOut(out var b, "Alice In Wonderland");

        Assert.Equal("Alice In Wonderland", b.Name);
    }

    [Fact]
    public void RefTypeByRef()
    {
        void ByRef(ref InMemoryBook book, string newName) => book = new(newName);

        var b = new InMemoryBook("before");
        ByRef(ref b, "after");

        Assert.Equal("after", b.Name);
    }

    [Fact]
    public void RefTypeByVal()
    {
        void ByVal(InMemoryBook book, string newName) => book.Name = newName;

        var b = new InMemoryBook("before");
        ByVal(b, "after");

        Assert.Equal("after", b.Name);
    }

    [Fact]
    public void RefLocal()
    {
        var i = 0;
        ref var varRef = ref i;

        varRef = 2;
        Assert.Equal(2, i);

        i = 3;
        Assert.Equal(3, varRef);
    }

    // Valid since C# 7.3
    [Fact]
    public void RefLocalReassign()
    {
        var i = 10;
        var j = 20;

        ref var varRef = ref i;
        varRef++;
        Assert.Equal(11, i);

        // Reassign the ref local to another memory location
        varRef = ref j;
        varRef++;
        Assert.Equal(11, i);
        Assert.Equal(21, j);
    }

    [Fact]
    public void RefReturn()
    {
        ref int GetLastRef(int[] numbers) => ref numbers[^1];

        int[] arr = [1, 2, 3, 4, 5, 6, 7];

        ref var lastRef = ref GetLastRef(arr);
        Assert.Equal(7, lastRef);

        GetLastRef(arr)++;
        Assert.Equal(8, arr[^1]);
    }

    [Fact]
    public void RefConditionalOp()
    {
        static (int even, int odd) CountEvenAndOdd(IEnumerable<int> numbers)
        {
            var res = (even: 0, odd: 0);
            foreach (var n in numbers)
            {
                ref var counter = ref int.IsEvenInteger(n) ? ref res.even : ref res.odd;
                counter++;
            }
            return res;
        }

        Assert.Equal((1, 2), CountEvenAndOdd([11, 21, 4]));
        Assert.Equal((4, 0), CountEvenAndOdd([12, 22, 0, -14]));
    }

    [Fact]
    public void DifferentObjects()
    {
        var b1 = new InMemoryBook("b1");
        var b2 = new InMemoryBook("b2");

        Assert.False(ReferenceEquals(b1, b2));
    }

    [Fact]
    public void EquivalentObjects()
    {
        var b1 = new InMemoryBook("b1");
        var b2 = new InMemoryBook("b1");

        Assert.Equivalent(b1, b2, strict: true);
    }

    [Fact]
    public void TwoVarsReferenceTheSameObject()
    {
        var b1 = new InMemoryBook("b1");
        var b2 = b1;

        Assert.True(ReferenceEquals(b1, b2));
    }

    [Fact]
    public void ArraysEquality()
    {
        DateTime[] arr1 =
        [
            new(2021, 1, 1),
            new(2021, 4, 2),
            new(2021, 4, 5)
        ];

        DateTime[] arr2 =
        [
            new(2021, 1, 1),
            new(2021, 4, 2),
            new(2021, 4, 5)
        ];

        // DateTime is a value type
        Assert.True(typeof(DateTime).IsValueType);
        Assert.True(arr1[0] == arr2[0]);
        Assert.Equal(arr1[0], arr2[0]);

        // Array is a reference type
        Assert.True(typeof(DateTime[]).IsClass);
        Assert.False(arr1 == arr2);
        Assert.True(arr1.SequenceEqual(arr2));
    }

    [Fact]
    public void StringIsReferenceTypeButBehaveAsValueType()
    {
        const string s1 = "Victor";
        const string s2 = "VICTOR";

        Assert.Equal(s2, s1.ToUpper());
    }

    [Fact]
    public void StringVsStringBuilder()
    {
        string foo = "Foo";
        string bar = foo.Replace('o', 'a');

        // String is immutable so it returns a new string instance instead of changing the old one
        Assert.False(ReferenceEquals(foo, bar));

        // StringBuilder is mutable
        var sb = new StringBuilder("Foo");
        sb.Replace('o', 'a');
        Assert.True(sb.Equals("Faa"));
    }

    [Fact]
    public void StringComparison()
    {
        var s1 = "An ";
        var s2 = "example";
        var s3 = "An example";
        var s4 = s1 + s2;

        Assert.True(s3 == s4);
        Assert.Equal(s3, s4);

        // s3 and s4 are two different objects in memory
        Assert.False(ReferenceEquals(s3, s4));
    }

    [Fact]
    public void InternStringExplicitly()
    {
        var s1 = "An ";
        var s2 = "example";
        var s3 = "An example";
        var s4 = string.Intern(s1 + s2);

        Assert.True(s3 == s4);
        Assert.Equal(s3, s4);

        // s3 and s4 point to the same interned string in memory
        Assert.True(ReferenceEquals(s3, s4));
    }

    [Fact]
    public void InternStringImplicitly()
    {
        var s3 = "An example";
        var s4 = "An " + "example";

        // s3 and s4 reference to the same memory area
        Assert.True(ReferenceEquals(s3, s4));
        Assert.NotNull(string.IsInterned(s3));
        Assert.NotNull(string.IsInterned(s4));

        //NB: The complexity of concatenations of String is O(N2), while for StringBuffer it is O(N)

        // The compiler automatically interns even this string because "An example" is already in the string pool
        string s1 = new StringBuilder().Append("An ").Append("example").ToString();
        Assert.NotNull(string.IsInterned(s1));

        string sb2 = new StringBuilder().Append("An ").Append("exampl").ToString();
        // The compiler does not intern this string
        Assert.Null(string.IsInterned(sb2));
    }

    [Fact]
    public void InternedStringsTest()
    {
        var s1 = string.Format("{0}{1}", "abc", "cba");
        var s2 = "abc" + "cba";
        var s3 = "abccba";

        Assert.True(s1 == s2);
        Assert.False(ReferenceEquals(s1, s2)); // Different instances

        Assert.True(s2 == s3);
        Assert.True(ReferenceEquals(s2, s3)); // s2 and s3 are interned
    }

    [Fact]
    public void MutateImmutableString()
    {
        const string s = "I’m immutable";
        unsafe
        {
            fixed (char* p = s)
            {
                *(p + 4) = ' ';
                *(p + 5) = ' ';
            }
        }

        Assert.Equal("I’m   mutable", s);
    }

    [Fact]
    public void ParamsTest()
    {
        string AppendNumbers(params int[] numbers)
        {
            var sb = new StringBuilder();
            foreach (var x in numbers)
            {
                sb.Append(x);
            }
            return sb.ToString();
        }

        Assert.Equal("123", AppendNumbers(1, 2, 3));
    }

    public static bool IsBoxed<T>(T value)
    {
        return (typeof(T).IsInterface || typeof(T) == typeof(object)) &&
               value != null &&
               value.GetType().IsValueType;
    }

    public interface IPerson
    {
        public string Name { get; set; }
    }

    public struct Person(string name) : IPerson
    {
        public string Name { get; set; } = name;
    }

    [Fact]
    public void BoxingPuzzlerTest()
    {
        // Value type
        var a = new Person("John");

        // Boxing to reference type
        var b = (IPerson)a;
        b.Name = "Steve";

        // c is another reference to b
        var c = b;
        c.Name = "Bob";

        Assert.True(a.GetType().IsValueType);
        Assert.False(IsBoxed(a));
        Assert.Equal("John", a.Name);

        Assert.True(b.GetType().IsValueType);
        Assert.True(IsBoxed(b));
        Assert.Equal("Bob", b.Name);

        Assert.True(c.GetType().IsValueType);
        Assert.True(IsBoxed(c));
        Assert.Equal("Bob", c.Name);

        Assert.True(ReferenceEquals(b, c));
    }

    [Fact]
    public void DynamicTypeTest()
    {
        dynamic text = "dlrow olleh";

        Assert.Equal("olleh", text.Substring(6));
        Assert.Throws<RuntimeBinderException>(() => text.SubstrinG(6));
    }

    [Fact]
    public void DynamicExpandoTest()
    {
        dynamic expando = new ExpandoObject();
        expando.hello = "hi";

        Func<string, string> func = input => input + " called on dynamic";
        expando.FakeMethod = func;

        Assert.Equal("hi", expando.hello);
        Assert.Equal("this func called on dynamic", expando.FakeMethod("this func"));

        IDictionary<string, object> dic = expando;
        Assert.Equal("hi", dic["hello"]);

        dic["OtherData"] = "other";
        Assert.Equal("other", expando.OtherData);
    }
}