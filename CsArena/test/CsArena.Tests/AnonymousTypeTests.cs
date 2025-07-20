using LinqArena;

namespace CsArena.Tests;

// Anonymous type is a reference type
// Values are stored in properties with backing fields
// Immutable
public class AnonymousTypeTests
{
   [Fact]
    public void TypeEquality()
    {
        var t1 = new { i = 1, j = 2 };
        var t2 = new { i = 1, j = 2 };

        Assert.True(t1.Equals(t2));
        Assert.False(t1 == t2);
    }

    [Fact]
    public void PropertyNamesInferredByCompiler()
    {
        var c = new Car { Year = 2024, Name = "Accord", Manufacturer = "Honda" };
        var anonType = new { c.Manufacturer, c.Name };

        var fieldNames = anonType
            .GetType()
            .GetProperties()
            .Select(f => f.Name)
            .ToList();
        Assert.Equal(["Manufacturer", "Name"], fieldNames);
    }

    [Fact]
    public void CreateAnonymousTypeUsingWithExpression()
    {
        var a1 = new { Manufacturer = "Honda", Name = "Accord" };
        var a2 = a1 with { Name = "Civic" };

        Assert.Equal("Civic", a2.Name);
        Assert.Equal("Honda", a2.Manufacturer);
    }

    [Fact]
    public void ReturnAnonymousType()
    {
        object GetNewType() => new { Manufacturer = "Honda", Name = "Accord" };
        var anonTypeInstance = GetNewType();

        var fieldNames = anonTypeInstance
            .GetType()
            .GetProperties()
            .Select(f => f.Name)
            .ToList();

        Assert.Equal(["Manufacturer", "Name"], fieldNames);

        var fieldValues = anonTypeInstance
            .GetType()
            .GetProperties()
            .Select(f => f.GetValue(anonTypeInstance))
            .ToList();

        Assert.Equal(["Honda", "Accord"], fieldValues);
    }
}