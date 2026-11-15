using System.Globalization;

namespace CsArena.Tests.models;

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