using LinqArena.Common;

namespace LinqArena
{
    internal class FilterOrderProject
    {
        public static void Playground()
        {
            var cars = ProcessCars(LinqExt.CsvPath);

            var topTenCars = 
                from rec in cars.SelectWithIndex()
                where rec.index < 10
                select rec;

            foreach (var rec in topTenCars)
            {
                Console.WriteLine($"{rec.index} : {rec.item.Manufacturer} {rec.item.Name}");
            }
            Console.WriteLine("***");


            var fuelEfficientMs = cars
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name); // secondary ordering has to use ThenBy

            var fuelEfficient =
                from c in cars
                orderby c.Combined descending, c.Name ascending
                select c;

            var queryProjection = cars
                .Where(c => c is { Manufacturer: "BMW", Year: 2016 })
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name)
                .Select(c => new
                {
                    c.Manufacturer,
                    c.Name,
                    c.Combined
                });

            foreach (var car in queryProjection.Take(3))
            {
                Console.WriteLine($"{car.Manufacturer} {car.Name} : {car.Combined}");
            }
            Console.WriteLine("***");


            var top = cars
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name)
                .First(c => c is { Manufacturer: "BMW", Year: 2016 });

            Console.WriteLine($"The top car {top.Manufacturer} {top.Name} : {top.Combined}");
            Console.WriteLine("***");


            var res = cars.Any(c => c is { Manufacturer: "Ford", Year: 2016 });
            Console.WriteLine($"Is there any Ford: {res}");
            Console.WriteLine("***");

            // SelectMany transforms every car name into characters
            var resFlatten = cars
                .TakeLast(3)
                .SelectMany(c => c.Name);
            foreach (var ch in resFlatten)
            {
                Console.WriteLine($"{ch}");
            }
            Console.WriteLine("***");
        }

        public static List<Car> ProcessCars(string path)
        {
            return File.ReadAllLines(path)
                .Skip(1) /* skip header line */
                .Where(r => r.Length > 1)
                .ToCar()
                .ToList();
        }
    }
}
