// ReSharper disable UnusedVariable
using LinqArena.Common;

namespace LinqArena
{
    internal class JoinGroupAggregate
    {
        public static void Playground()
        {
            var cars = FilterOrderProject.ProcessCars(LinqExt.CsvPath);
            var mfg = ProcessManufacturers(LinqExt.CsvMfgPath);

            var innerJoin =
                from c in cars
                join m in mfg
                    /* join on composite key */
                    on new { c.Manufacturer, c.Year }
                        equals
                        new { Manufacturer = m.Name, m.Year }
                orderby c.Combined descending, c.Name
                select new
                {
                    m.Headquarters,
                    c.Name,
                    c.Combined
                };

            var innerJoinMs = cars
                    .Join(mfg,
                        c => new { c.Manufacturer },
                        m => new { Manufacturer = m.Name },
                        (c, m) => new
                        {
                            m.Headquarters,
                            c.Name,
                            c.Combined
                        })
                    .OrderByDescending(c => c.Combined)
                    .ThenBy(c => c.Name);

            foreach (var car in innerJoinMs.Take(10))
            {
                Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            }
            Console.WriteLine("\n***\n");

            var groupBy =
                from c in cars
                group c by c.Manufacturer.ToUpper() into gr
                select new
                {
                    Name = gr.Key,
                    Cnt = gr.Count(),
                    Max = gr.Max(c => c.Combined),
                    Min = gr.Min(c => c.Combined),
                    Avg = gr.Average(c => c.Combined)
                } into res
                orderby res.Max descending
                select res;

            var groupByMs = cars
                    .GroupBy(c => c.Manufacturer.ToUpper())
                    .Select(gr =>
                    {
                        var res = gr.Aggregate(
                            new CarStatistics(),
                            (acc, c) => acc.Accumulate(c),
                            acc => acc.Compute()
                        );
                        return new
                        {
                            Name = gr.Key,
                            res.Count,
                            res.Max,
                            res.Min,
                            res.Avg
                        };
                    })
                    .OrderByDescending(r => r.Max);

            Console.WriteLine("Most fuel efficient manufacturers\n");
            foreach (var gr in groupByMs)
            {
                Console.WriteLine(
                    $"Company {gr.Name} has {gr.Count} cars \n" +
                    $"\tMax fuel efficiency: {gr.Max}\n" +
                    $"\tMin fuel efficiency: {gr.Min}\n" +
                    $"\tAvg fuel efficiency: {gr.Avg}\n");
            }
            Console.WriteLine("\n***\n");


            var groupJoin =
                from m in mfg
                join c in cars on m.Name equals c.Manufacturer
                    into carGroup
                orderby m.Name
                select new
                {
                    Manufacturer = m,
                    Cars = carGroup
                };

            var groupJoinMs = mfg
                .GroupJoin(cars,
                    m => m.Name,
                    c => c.Manufacturer,
                    (m, g) => new
                    {
                        Manufacturer = m,
                        Cars = g
                    })
                .OrderBy(m => m.Manufacturer.Name);

            foreach (var group in groupJoinMs)
            {
                Console.WriteLine($"{group.Manufacturer.Name} : {group.Manufacturer.Headquarters}");
                foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
            Console.WriteLine("\n***\n");


            var topFuelEfficientCarsByCountry1 =
                from c in cars
                join m in mfg
                    on c.Manufacturer equals m.Name
                group c by m.Headquarters into gr
                orderby gr.Key
                select gr;

            Console.WriteLine("\nTop 3 Fuel Efficient Cars By Country\n");
            foreach (var group in topFuelEfficientCarsByCountry1)
            {
                Console.WriteLine($"{group.Key}");
                foreach (var car in group.OrderByDescending(c => c.Combined).Take(3))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
            Console.WriteLine("\n***\n");


            var topFuelEfficientCarsByCountry2 =
                from m in mfg
                join c in cars on m.Name equals c.Manufacturer
                    into carGroup
                select new
                {
                    Manufacturer = m,
                    Cars = carGroup
                } into result
                group result by result.Manufacturer.Headquarters;


            var topFuelEfficientCarsByCountry2Ms = mfg
                 .GroupJoin(cars,
                     m => m.Name,
                     c => c.Manufacturer,
                     (m, g) => new
                     {
                         Manufacturer = m,
                         Cars = g
                     })
                 .GroupBy(m => m.Manufacturer.Headquarters)
                 .OrderBy(group => group.Key);

            Console.WriteLine("\nTop 3 Fuel Efficient Cars By Country\n");
            foreach (var group in topFuelEfficientCarsByCountry2Ms)
            {
                Console.WriteLine($"{group.Key}");
                foreach (var car in group.SelectMany(g => g.Cars)
                                         .OrderByDescending(c => c.Combined)
                                         .Take(3))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
            Console.WriteLine("\n***\n");


            //var query = cars
            //    .OrderByDescending(c => c.Combined)
            //    .ThenBy(c => c.Name)
            //    .Select(c => new
            //    {
            //        c.Manufacturer,
            //        c.Name,
            //        c.Combined
            //    });
        }



        public static List<Manufacturer> ProcessManufacturers(string path)
        {
            return File
                .ReadAllLines(path)
                .Where(r => r.Length > 1)
                .Select(r =>
                {
                    var columns = r.Split(',');
                    return new Manufacturer
                    {
                        Name = columns[0],
                        Headquarters = columns[1],
                        Year = int.Parse(columns[2])
                    };
                })
                .ToList();
        }

    }
}
