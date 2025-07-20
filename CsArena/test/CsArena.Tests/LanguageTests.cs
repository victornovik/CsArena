using CsArena.Tests.ext;

namespace CsArena.Tests;

public class LanguageTests
{
    [Fact]
    public void KeywordUsedAsVariableNameTest()
    {
        var @string = string.Empty;
        Assert.Empty(@string);

        var @new = 5;
        Assert.Equal(5, @new);

        // @ charcter is stripped
        Assert.Equal("new", nameof(@new));
    }

    public class TestStatic
    {
        public static int TestValue;

        public TestStatic()
        {
            if (TestValue == 0)
                TestValue = 5;
        }

        // static ctor is called before any instance of the class is created
        static TestStatic()
        {
            if (TestValue == 0)
                TestValue = 10;
        }

        public void Touch()
        {
            if (TestValue == 5)
                TestValue = 6;
        }
    }

    [Fact]
    public void StaticConstructorTest()
    {
        var t = new TestStatic();
        t.Touch();

        Assert.Equal(10, TestStatic.TestValue);
    }

    [Fact]
    public void CapturedVariableTest()
    {
        var total = 0;

        var actions = new List<Action>();
        for (var i = 0; i < 10; i++)
        {
            actions.Add(() => total += i);
        }
        foreach (var action in actions)
        {
            action();
        }

        Assert.Equal(100, total);
    }

    public class A
    {
        public virtual string Print1() => "A";
        public string Print2() => "A";
    }
    public class B : A
    {
        public override string Print1() => "B";
    }
    public class C : B
    {
        // new keyword explicitly hides Print2() that is inherited from `A` class
        public new string Print2() => "C";
    }

    [Fact]
    public void NewQualifierHidesTest()
    {
        C c = new C();
        A a = c;

        Assert.Equal("B", a.Print1());

        Assert.Equal("A", a.Print2());
        Assert.Equal("C", c.Print2());
    }

    [Fact]
    public void OperatorAsTest()
    {
        string? Cast(object obj) => (string?)obj;
        string? OperatorAs(object obj) => obj as string;

        Assert.Throws<InvalidCastException>(() => Cast(1));
        Assert.Null(OperatorAs(1));
    }

    class Cloneable(InMemoryBook book) : ICloneable
    {
        public InMemoryBook book = book;

        // MemberwiseClone() is always shallow copy
        public object Clone() => MemberwiseClone();

        public Cloneable ShallowCopy() => (Cloneable)MemberwiseClone();

        public Cloneable DeepCopy()
        {
            // MemberwiseClone() is always shallow copy
            var other = (Cloneable)MemberwiseClone();
            other.book = new InMemoryBook(new string(book.Name));
            return other;
        }
    }

    [Fact]
    public void ShallowCloneTest()
    {
        // 1. ICloneable.Clone() implements shallow copy in our case
        var src = new Cloneable(new InMemoryBook("C#"));
        var dst1 = src.Clone() as Cloneable;
        Assert.True(ReferenceEquals(src.book, dst1!.book));
        Assert.True(ReferenceEquals(src.book.Name, dst1.book.Name));

        // 2. Explicitly implement shallow copy
        var dst2 = src.ShallowCopy();
        Assert.True(ReferenceEquals(src.book, dst2.book));
        Assert.True(ReferenceEquals(src.book.Name, dst2.book.Name));

        // 3. Shallow copy with Reflection
        var dst3 = (Cloneable)Activator.CreateInstance(typeof(Cloneable), src.book)!;
        Assert.True(ReferenceEquals(src.book, dst3.book));
        Assert.True(ReferenceEquals(src.book.Name, dst3.book.Name));
    }

    [Fact]
    public void DeepCloneTest()
    {
        // 2. Explicitly implement deep copy
        var src = new Cloneable(new InMemoryBook("C#"));
        var dst2 = src.DeepCopy();
        Assert.False(ReferenceEquals(src.book, dst2.book));
        Assert.False(ReferenceEquals(src.book.Name, dst2.book.Name));

        // 3. Deep copy with Reflection
        var dst3 = (Cloneable)Activator.CreateInstance(typeof(Cloneable), new InMemoryBook(new string(src.book.Name)))!;
        Assert.False(ReferenceEquals(src.book, dst3.book));
        Assert.False(ReferenceEquals(src.book.Name, dst3.book.Name));
    }

    internal class StaticReadonly
    {
        private static readonly string b = c;
        private static readonly string c = "c";
        private readonly string a = "a";
        public readonly string Res;

        public StaticReadonly()
        {
            Res = $"{b} {c} {a}";
        }
    }

    [Fact]
    public void StaticReadonlyPuzzler()
    {
        var r = new StaticReadonly();
        Assert.Equal(" c a", r.Res);
    }

    [Fact]
    public void ExtensionMethodCalledOnNull()
    {
        Assert.Equal("ab", "a".Concat("b"));

        // Call our extension method on null instance
        Assert.Equal("b", ((string?)null).Concat("b"));

        // Call System.String::Concat()
        Assert.Equal("c", String.Concat(null, "c"));
        Assert.Equal("c", String.Concat(null, "c", null));
    }

    [Fact]
    public void NamedArgumentEvaluationTest()
    {
        void func(int x, int y, int z)
        {
            Assert.Equal(0, z);
            Assert.Equal(1, y);
            Assert.Equal(2, x);
        }

        var i = 0;
        func(z: i++, y: i++, x: i++);
        Assert.Equal(3, i);
    }

    public record Record(int Id, int Age);
    public class Class(int Id, int Age);
    public struct Struct(int Id, int Age);

    [Fact]
    public void GetHashCodeTest()
    {
        var rec1 = new Record(1, 2);
        var rec2 = new Record(1, 2);
        // Hash codes of two records are equal if all the fields are equal
        Assert.False(ReferenceEquals(rec1, rec2));
        Assert.Equal(rec1.GetHashCode(), rec2.GetHashCode());

        var rec3 = rec2 with { Age = 3 };
        Assert.NotEqual(rec3.GetHashCode(), rec2.GetHashCode());

        var s1 = new Struct(1, 2);
        var s2 = new Struct(1, 3);

        // Hash codes of two structs are equal if the first non-static fields are equal
        // Hash code of struct is calculated in ValueType.GetHashCode()
        Assert.Equal(s1.GetHashCode(), s2.GetHashCode());

        var c1 = new Class(1, 2);
        var c2 = new Class(1, 2);

        // Hash codes of two classes are NOT equal despite the content is equal
        Assert.False(ReferenceEquals(c1, c2));
        Assert.NotEqual(c1.GetHashCode(), c2.GetHashCode());
    }

    [Fact]
    public void CallLambdaAtDeclarationSite()
    {
        var res = ((Func<string>)(() => "Call me"))();
        Assert.Equal("Call me", res);

        res = new Func<string>(() => "Call me")();
        Assert.Equal("Call me", res);

        res = new Lazy<string>(() => "Call me").Value;
        Assert.Equal("Call me", res);
    }
}