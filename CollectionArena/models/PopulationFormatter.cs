namespace CollectionArena.models;

class PopulationFormatter
{
    public static string FormatPopulation(int population)
    {
        if (population == 0)
            return "(Unknown)";

        var popRounded = RoundPopulation4(population);

        return $"{popRounded:### ### ### ###}".Trim();
    }

    // Rounds the population to 4 significant figures
    private static int RoundPopulation4(int population)
    {
        // work out what rounding accuracy we need if we are to round to 4 significant figures
        var accuracy = Math.Max((int)(GetHighestPowerOfTen(population) / 10_000L), 1);

        // now do the rounding
        return RoundToNearest(population, accuracy);

    }

    /// <summary>
    /// Rounds the number to the specified accuracy
    /// For example, if the accuracy is 10, then we round to the nearest 10:
    /// 23 -> 20
    /// 25 -> 30
    /// etc.
    /// </summary>
    /// <param name="exact"></param>
    /// <param name="accuracy"></param>
    /// <returns></returns>
    public static int RoundToNearest(int exact, int accuracy)
    {
        int adjusted = exact + accuracy / 2;
        return adjusted - adjusted % accuracy;
    }

    /// <summary>
    /// Returns the highest number that is a power of 10 and is no larger than the number supplied
    /// Examples:
    /// GetHighestPowerOfTen(11) = 10
    /// GetHighestPowerOfTen(99) = 10
    /// GetHighestPowerOfTen(100) = 100
    /// GetHighestPowerOfTen(843) = 100
    /// GetHighestPowerOfTen(1000) = 1000
    /// </summary>
    public static long GetHighestPowerOfTen(int x)
    {
        long result = 1;
        while (x > 0)
        {
            x /= 10;
            result *= 10;
        }
        return result;
    }
}
