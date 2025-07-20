using CollectionArena.models;

namespace CsArena.Tests;

public class CollectionUtil
{
    public static List<Country> CreateCountryList()
    {
        return
        [
            new Country("Russia", "RUS", "Europe", 145_000_000),
            new Country("Norway", "NOR", "Europe", 5_282_223),
            new Country("Finland", "FIN", "Europe", 5_511_303)
        ];
    }

    public static IEnumerable<Country> CreateEnumerable()
    {
        return CreateCountryList();
    }

    public static Dictionary<string, Country> CreateCountryDictionary()
    {
        return CreateCountryList().ToDictionary(c => c.Code, StringComparer.OrdinalIgnoreCase);
    }

    public static SortedDictionary<string, Country> CreateSortedCountryDictionary()
    {
        var dict = CreateCountryList().ToDictionary(c => c.Code, StringComparer.OrdinalIgnoreCase);
        return new SortedDictionary<string, Country>(dict);
    }

    public static SortedList<string, Country> CreateSortedCountryList()
    {
        var dict = CreateCountryList().ToDictionary(c => c.Code, StringComparer.OrdinalIgnoreCase);
        return new SortedList<string, Country>(dict);
    }
}