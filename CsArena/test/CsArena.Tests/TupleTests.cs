using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using LinqArena;

namespace CsArena.Tests;

public class TupleTests
{
    [Fact]
    public void ValueVsReferenceTuples()
    {
        var valueTuple = (5, "Hello", true);

        Assert.True(valueTuple.GetType().IsValueType);
        Assert.Equal(typeof(ValueTuple<int, string, bool>), valueTuple.GetType());

        // Value tuple is mutable
        valueTuple.Item1 = 1;
        Assert.Equal(1, valueTuple.Item1);
        Assert.Equal("Hello", valueTuple.Item2);

        var referenceTuple = new Tuple<int, string, bool>(1, "Hello", true);

        // Reference tuple is immutable
        // referenceTuple.Item1 = 5;

        Assert.True(referenceTuple.GetType().IsClass);
        Assert.Equal(1, referenceTuple.Item1);
        Assert.Equal("Hello", referenceTuple.Item2);

        Assert.True(valueTuple == referenceTuple.ToValueTuple());
    }

    [Fact]
    public void ITupleTest()
    {
        ITuple tuple = (Manufacturer: "Honda", Name: "Accord");

        Assert.Equal(2, tuple.Length);
        Assert.Equal("Honda", tuple[0]);
    }

    [Fact]
    public void FieldNamesInferredByCompiler()
    {
        var c = new Car { Year = 2024, Name = "Accord", Manufacturer = "Honda" };
        var tuple = (c.Manufacturer, c.Name);

        Assert.Equal("Accord", tuple.Name);
        Assert.Equal("Honda", tuple.Manufacturer);

        // Name erasure occurs at compile time.
        // The names are available only as a syntactic sugar
        // CLR does not know about them.
        var fieldNames = tuple
            .GetType()
            .GetFields()
            .Select(f => f.Name)
            .ToList();
        Assert.Equal(["Item1", "Item2"], fieldNames);
    }

    [Fact]
    public void NamedTupleSerialization()
    {
        (string Manufacturer, string Name) tuple = ("Honda", "Accord");
        var json = JsonSerializer.Serialize(tuple, options: new JsonSerializerOptions { IncludeFields = true });

        // The names of the named tuple are erased by compiler
        Assert.Equal("""{"Item1":"Honda","Item2":"Accord"}""", json);
    }

    [Fact]
    public void DeconstructTuple()
    {
        var (_, car) = ("Honda", "Accord");
        Assert.Equal("Accord", car);

        string mfg;
        (mfg, _) = ("Honda", "Accord");
        Assert.Equal("Honda", mfg);
    }

    [Fact]
    public void DeconstructClassWithCustomDeconstructor()
    {
        var car = new Car { Combined = 50, Name = "Accord", Manufacturer = "Honda" };
        var (_, name, _) = car;

        Assert.Equal("Accord", name);
        Assert.True(car is ("Honda", _, combined: > 40));
    }

    [Fact]
    public void DeconstructKeyValuePair()
    {
        var dic = new Dictionary<int, Car>
        {
            {1, new Car { Combined = 50, Name = "Accord", Manufacturer = "Honda" }},
            {2, new Car { Combined = 60, Name = "Prius", Manufacturer = "Toyota" }},
            {3, new Car { Combined = 51, Name = "Land Cruiser", Manufacturer = "Toyota" }},
        };

        foreach (var (_, car) in dic)
        {
            Assert.True(car.Combined >= 50);
        }
    }

    [Fact]
    public void DeconstructionPuzzler()
    {
        var sb = new StringBuilder("12345");
        var orig = sb;

        (sb, sb.Length) = (new StringBuilder("67890"), 3);

        Assert.Equal("123", orig.ToString());
        Assert.Equal("67890", sb.ToString());
    }

    [Fact]
    public void CreateTupleUsingWithExpression()
    {
        var t1 = (Manufacturer: "Honda", Name: "Accord");
        var t2 = t1 with { Name = "Civic" };

        Assert.IsType<ValueTuple<string, string>>(t2);
        Assert.Equal("Civic", t2.Name);
    }

    [Fact]
    public void TupleExtensionMethod()
    {
        var tuple = (Manufacturer: "Honda", Name: "Accord");
        Assert.Equal(11, tuple.SumLength());
    }

    [Fact]
    public void WompleTest()
    {
        // Any ValueTuple has at maximum 8 elements.
        // The last element of an 8-arity is always another ValueTuple<...>
        var arity8 = (1, 2, 3, 4, 5, 6, 7, 8);

        // ValueType<T> with arity 1 is called `womple`
        Assert.Equal(8, arity8.Item8);
        Assert.Equal(typeof(ValueTuple<int>), arity8.Rest.GetType());

        var arity16 = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);

        Assert.Equal(16, arity16.Item16);
        Assert.Equal(typeof(ValueTuple<int, int>), arity16.Rest.Rest.GetType());
    }

    [Fact]
    public void NupleTest()
    {
        // Non-generic ValueType without payload is called `nuple`
        var tuple = ValueTuple.Create(1, 2);

        Assert.Equal(1, tuple.Item1);
        Assert.Equal(2, tuple.Item2);
    }
}

public static class TupleExt
{
    public static int SumLength(this (string, string) tuple) => tuple.Item1.Length + tuple.Item2.Length;
}