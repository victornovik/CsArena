using System.Reflection;
using Xunit.v3;

namespace CsArena.Tests;

// Keeps the category scheme complete: every test must have at least one `category` trait, so that
// `--filter-trait` / `--filter-not-trait "category=..."` never silently leaves a test out.
[Category(TestCategories.Bcl)]
public class TestCategoriesTests
{
    private static readonly string[] KnownCategories =
    [
        TestCategories.Network, TestCategories.Gc, TestCategories.Concurrency, TestCategories.Slow,
        TestCategories.LeetCode, TestCategories.Language, TestCategories.Collections, TestCategories.Linq,
        TestCategories.Bcl,
    ];

    private static IEnumerable<string> CategoriesOf(MemberInfo member) =>
        member.GetCustomAttributes(inherit: true)
            .OfType<ITraitAttribute>()
            .SelectMany(attribute => attribute.GetTraits())
            .Where(trait => trait.Key == TestCategories.TraitName)
            .Select(trait => trait.Value);

    private static IEnumerable<MethodInfo> TestMethods() =>
        typeof(TestCategoriesTests).Assembly.GetTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Where(method => method.GetCustomAttributes(inherit: true).OfType<IFactAttribute>().Any());

    [Fact]
    public void EveryTestHasACategory()
    {
        var untagged = TestMethods()
            .Where(method => !CategoriesOf(method).Concat(CategoriesOf(method.DeclaringType!)).Any())
            .Select(method => $"{method.DeclaringType!.FullName}.{method.Name}")
            .Order()
            .ToList();

        Assert.True(untagged.Count == 0,
            $"Tests without a [Category]: add [Category(TestCategories.X)] to the class or method:{Environment.NewLine}"
            + string.Join(Environment.NewLine, untagged));
    }

    [Fact]
    public void EveryCategoryInUseIsKnown()
    {
        var used = TestMethods()
            .SelectMany(method => CategoriesOf(method).Concat(CategoriesOf(method.DeclaringType!)))
            .Distinct()
            .Order()
            .ToList();

        Assert.Empty(used.Except(KnownCategories));
        // Every known category is used by at least one test
        Assert.Empty(KnownCategories.Except(used));
    }
}
