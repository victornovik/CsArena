using CollectionArena.models;
using CollectionArena.models.Common;
using CsArena.Tests.models;

namespace CsArena.Tests;

// Reference type constraint    where T : class
// Value type constraint        where T : struct
// Non-nullable type            where T : notnull
// T must be or derive from     where T : U
// T must have no-arg ctor      where T : new()

file class MagicHat<T> where T : class, new()
{
    public void Fill(int count)
    {
        for (var i = 0; i < count; i++)
        {
            Put(new T());
        }
    }

    public void Put(T t) => _queue.Enqueue(t);

    public T? Pull() => _queue.Count == 0 ? null : _queue.Dequeue();

    private readonly Queue<T> _queue = new();
}

public class GenericTests
{
    internal class NoArg;
    //internal class WithArg(int prop);

    [Fact]
    public void ConstraintNew()
    {
        const int count = 5;

        var hat = new MagicHat<NoArg>();
        hat.Fill(count);

        for (var i = 0; i < count; i++)
            Assert.NotNull(hat.Pull());

        Assert.Null(hat.Pull());

        // Will not compile as WithArg does not have no-arg ctor
        // new MagicHat<WithArg>();
    }

    private class Item<T>
    {
        public Item() => ++InstanceCount;
        public static int InstanceCount;
    }

    [Fact]
    public void GenericTypesAreDifferentClasses()
    {
        Assert.Equal(0, Item<int>.InstanceCount);
        Assert.Equal(0, Item<double>.InstanceCount);
        _ = new Item<int>();
        _ = new Item<int>();
        _ = new Item<double>();

        // All Item<int> specializations will share the same Item<int> type created by runtime on the first use.
        // Item<int>, Item<double> and Item<string> are different types created in runtime.
        Assert.Equal(2, Item<int>.InstanceCount);
        Assert.Equal(1, Item<double>.InstanceCount);
        Assert.Equal(0, Item<string>.InstanceCount);
        Assert.Equal(0, Item<Player>.InstanceCount);
        Assert.Equal(0, Item<Country>.InstanceCount);

        _ = new Item<string>();
        _ = new Item<Player>();
        _ = new Item<Country>();
        _ = new Item<Player>();
        Assert.Equal(1, Item<string>.InstanceCount);
        Assert.Equal(2, Item<Player>.InstanceCount);
        Assert.Equal(1, Item<Country>.InstanceCount);
    }

    private class Outer<T>
    {
        public class Inner<U, V>
        {
            static Inner() => StaticCtorCount++;
            public static int StaticCtorCount;
        }
    }

    [Fact]
    public void StaticMembersInGeneric()
    {
        Assert.Equal(1, Outer<int>.Inner<string, DateTime>.StaticCtorCount);
        // Static constructor has already been called for this closed type
        Assert.Equal(1, Outer<int>.Inner<string, DateTime>.StaticCtorCount);

        Assert.Equal(1, Outer<double>.Inner<string, DateTime>.StaticCtorCount);
        Assert.Equal(1, Outer<int>.Inner<string, string>.StaticCtorCount);
        Assert.Equal(1, Outer<int>.Inner<DateTime, string>.StaticCtorCount);
    }
    
    [Fact]
    public void UnboundGenericType()
    {
        Type unboundGenericType = typeof(MagicHat<>);
        Assert.Throws<ArgumentException>(() => Activator.CreateInstance(unboundGenericType));
        Assert.NotNull(Activator.CreateInstance(unboundGenericType.MakeGenericType(typeof(Player))));

        Type closedGenericType = typeof(MagicHat<Player>);
        Assert.NotNull(Activator.CreateInstance(closedGenericType));
    }

    [Fact]
    public void DefaultKeywordForGenericType()
    {
        // For reference type it will return null
        // For value type it will return its default value e.g. 0 for int
        T? GetDefaultFor<T>() => default;

        Assert.Null(GetDefaultFor<Country>());
        Assert.Equal(0, GetDefaultFor<int>());
        Assert.Equal(0.0, GetDefaultFor<double>());
        Assert.Null(GetDefaultFor<string>());
        Assert.Equal(new DateTime(1,1,1,0,0,0), GetDefaultFor<DateTime>());
    }

    [Fact]
    public void Covariance()
    {
        string GetString() => string.Empty;

        // IEnumerable is declared as <out T>.
        // Assign more derived type List<string> to a more general type IEnumerable<object>.
        IEnumerable<object> objects = new List<string>();
        Assert.NotNull(objects);

        // Classes are invariant even if they implement variant interfaces
        // List<object> list = new List<string>();

        // Variance in generic interfaces is supported for reference types only, int is value type
        // objects = (IEnumerable<int>)new List<int>();

        // Func<out TResult> so assign a method returning more derived type to a method returning more general type.
        Func<string> fnMoreDerived = GetString;
        Func<object> fnMoreGeneral = fnMoreDerived;
        Assert.NotNull(fnMoreGeneral);

        // Cannot assign method returning less derived type to method returning more derived type
        // fnMoreDerived = fnLessDerived;
    }

    [Fact]
    public void Contravariance()
    {
        void SetObject(object obj) { }

        // Action<in T> so assign a method taking more general type to a method taking more derived type.
        Action<object> fnMoreGeneral = SetObject;
        Action<string> fnMoreDerived = fnMoreGeneral;
        Assert.NotNull(fnMoreDerived);

        // Cannot assign method taking more derived type to method taking less derived type
        // fnLessDerived = fnMoreDerived;
    }

    // In a generic interface a type parameter can be declared covariant (out) if it satisfies the following conditions:
    //      1. The type parameter is used only as a return type of interface methods and not used as a type of method arguments.
    //      2. The type parameter is not used as a generic constraint for the interface methods.
    private interface ICovariant<out T>
    {
        T Get();
        // void Foo<F>() where F : T;
    }

    // In a generic interface a type parameter can be declared contravariant (in) if it satisfies the following conditions:
    //      1. The type parameter is used only in method parameters and not used as a method return type.
    private interface IContravariant<in T>
    {
        void Set(T t);
        void Foo<F>() where F : T;
    }
    private interface IOmnivariant<in In, out Out>
    {
        Out Convert(In i);
    }

    private interface IInvariant<T> : ICovariant<T>;

    class CovariantClass<T> : ICovariant<T>
    {
        public T Get() => throw new NotImplementedException();
    }
    class ContravariantClass<T> : IContravariant<T>
    {
        public void Set(T t) => throw new NotImplementedException();
        public void Foo<F>() where F : T => throw new NotImplementedException();
    }
    class OmnivariantClass<I, O> : IOmnivariant<I, O>
    {
        public O Convert(I i) => throw new NotImplementedException();
    }
    class InvariantClass<T> : IInvariant<T>
    {
        public T Get() => throw new NotImplementedException();
    }

    [Fact]
    public void CovariantGenericInterface()
    {
        ICovariant<string> str = new CovariantClass<string>();
        ICovariant<object> obj = str;

        // You can assign str to obj because ICovariant interface is covariant to its type parameter T
        Assert.NotNull(obj);
    }

    [Fact]
    public void ContravariantGenericInterface()
    {
        IContravariant<object> obj = new ContravariantClass<object>();
        IContravariant<string> str = obj;

        // You can assign obj to str because IContravariant interface is contravariant to its type parameter T
        Assert.NotNull(str);
    }

    [Fact]
    public void OmnivariantGenericInterface()
    {
        IOmnivariant<object, string> obj = new OmnivariantClass<object, string>();
        IOmnivariant<string, object> str = obj;

        Assert.NotNull(str);
    }

    [Fact]
    public void InvariantGenericInterface()
    {
        IInvariant<string> str = new InvariantClass<string>();
        
        object o = str;
        Assert.NotNull(o);

        // You cannot assign str to obj because IInvariant interface is now invariant to its type parameter T.
        // Even if it extends ICovariant interface
        // IInvariant<object> obj = str;
    }

    [Fact]
    public void ParseEnumByTypeParameter()
    {
        var e1 = "Step1".ParseEnum<Steps>();
        var e2 = "Step2".ParseEnum<Steps>();
        var e3 = "Step3".ParseEnum<Steps>();
        Assert.Equal(Steps.Step1, e1);
        Assert.Equal(Steps.Step2, e2);
        Assert.Equal(Steps.Step3, e3);
    }
    private enum Steps
    {
        Step1,
        Step2,
        Step3,
    }
    
    //Stopwatch watch = Stopwatch.StartNew();
    //watch.Stop();
    //var ms = watch.ElapsedMilliseconds;
}