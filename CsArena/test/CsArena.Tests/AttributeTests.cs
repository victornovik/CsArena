using System.Reflection;

namespace CsArena.Tests;

public class AttributeTests
{
    [Fact]
    public void CustomAttribute()
    {
        bool CanUse(Product p)
        {
            Type type = p.GetType();
            var attrs = type.GetCustomAttributes(typeof(ActualProductAttribute), false);
            if (attrs.Length == 0)
                return false;

            var attr = (ActualProductAttribute)attrs[0];
            return attr.CanUse;
        }

        Assert.True(CanUse(new Product()));
        Assert.False(CanUse(new Book()));
        Assert.False(CanUse(new Food()));
        Assert.True(CanUse(new Computer()));
    }


    [Fact]
    public void EnumAttribute()
    {
        string GetEnumText<T>(T t) where T : Enum
        {
            Type type = t.GetType();
            FieldInfo? memberInfo = type.GetField(t.ToString());

            var attrs = memberInfo?.GetCustomAttributes(typeof(EnumTextAttribute), false);
            if (attrs == null || attrs.Length == 0)
                return string.Empty;

            var attr = (EnumTextAttribute)attrs[0];
            return attr.Text;
        }

        Assert.Equal("In stock", GetEnumText(ProductStatus.InStock));
        Assert.Equal("Out of stock", GetEnumText(ProductStatus.OutOfStock));
        Assert.Equal("No longer available", GetEnumText(ProductStatus.Discontinuted));
        Assert.Empty(GetEnumText(ProductStatus.NotYetLaunched));
    }
}

[AttributeUsage((AttributeTargets.Class | AttributeTargets.Struct))]
file sealed class ActualProductAttribute : Attribute
{
    public bool CanUse { get; set; }
}


[ActualProduct(CanUse = true)]
file class Product;

file class Book : Product;

[ActualProduct(CanUse = true)]
file class Computer : Product;

[ActualProduct(CanUse = false)]
file class Food : Product;


[AttributeUsage((AttributeTargets.Field))]
file sealed class EnumTextAttribute : Attribute
{
    public required string Text { get; set; }
}

file enum ProductStatus
{
    [EnumText(Text = "In stock")]
    InStock,
    [EnumText(Text = "Out of stock")]
    OutOfStock,
    [EnumText(Text = "No longer available")]
    Discontinuted,
    NotYetLaunched,
}