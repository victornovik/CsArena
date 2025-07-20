using CollectionArena.models.Common;

namespace CollectionArena.models;

using CountriesByRegion = Dictionary<string, List<Country>>;

internal class CsvReader(string csvFilePath)
{
    public List<Country> ReadCountries()
    {
        using var @in = new StreamReader(csvFilePath);
        @in.ReadLine(); // skip header

        var countries = new List<Country>();

        while (@in.ReadLine() is { } line)
            countries.Add(ParseLine(line));

        // string? line = null;
        // while ((line = sr.ReadLine()) != null)

        return countries;
    }

    public CountriesByRegion ReadCountriesInDictionary()
    {
        using var @in = new StreamReader(csvFilePath);
        @in.ReadLine(); // skip header

        CountriesByRegion countries = [];

        while (@in.ReadLine() is { } line)
        {
            var c = ParseLine(line);
            if (countries.TryGetValue(c.Region, out var list))
                list.Add(c);
            else
                countries.Add(c.Region, [c]);
        }

        return countries;
    }

    private static Country ParseLine(string line)
    {
        var tokens = line.SplitCsv().ToArray();
        _ = int.TryParse(tokens[3], out var population);
        return new Country(tokens[0], tokens[1], tokens[2], population);
    }

    public static void RemoveCommaCountries(List<Country> countries)
    {
        // Option #1
        //for (var i = countries.Count - 1; i >= 0; --i)
        //{
        //    if (countries[i].Name.Contains(','))
        //        countries.RemoveAt(i);
        //}

        // Option #2
        countries.RemoveAll(c => c.Name!.Contains(','));
    }
}