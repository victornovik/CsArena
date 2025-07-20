using System.Globalization;

namespace LinqArena;

public class Car
{
    public void Deconstruct(out string name, out int combined) => (name, combined) = (Name, Combined);

    public void Deconstruct(out string manufacturer, out string? name, out int combined) => (manufacturer, name, combined) = (Manufacturer, Name, Combined);

    public int Id { get; set; } // EF primary key
    public int Year { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Displacement { get; set; }
    public int Cylinders { get; set; }
    public int City { get; set; }
    public byte Highway { get; set; }
    public int Combined { get; set; }
}

public class CarStatistics
{
    public CarStatistics Accumulate(Car car)
    {
        ++Count;
        Total += car.Combined;
        Max = Math.Max(Max, car.Combined);
        Min = Math.Min(Min, car.Combined);

        return this;
    }

    public CarStatistics Compute()
    {
        Avg = Total / Count;
        return this;
    }

    public int Max { get; set; } = int.MinValue;
    public int Min { get; set; } = int.MaxValue;
    public int Total { get; set; }
    public int Count { get; set; }
    public double Avg { get; set; }

        
}

public static class CarExtensions
{
    internal static IEnumerable<Car> ToCar(this IEnumerable<string> src)
    {
        foreach (var line in src)
        {
            var columns = line.Split(',');

            yield return new Car
            {
                Year = int.Parse(columns[0]),
                Manufacturer = columns[1],
                Name = columns[2],
                Displacement = double.Parse(columns[3], CultureInfo.InvariantCulture),
                Cylinders = int.Parse(columns[4]),
                City = int.Parse(columns[5]),
                Highway = byte.Parse(columns[6]),
                Combined = int.Parse(columns[7])
            };
        }
    }
}