// ReSharper disable UnusedVariable

using System.Data.Entity;
using LinqArena.Common;

namespace LinqArena
{
    internal class LinqEf
    {
        public static void Playground()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();
        }

        private static void QueryData()
        {
            var db = CreateDatabase();

            var topTenFuelEfficient =
            (
                from c in db.Cars //.SelectWithIndex()
                orderby c.Combined descending, c.Name
                select c
            ).Take(10);

            var topTenFuelEfficientMs = db.Cars
                .Where(c => c.Manufacturer.ToUpper() == "BMW")
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name)
                .Take(10);

            foreach (var car in topTenFuelEfficientMs)
            {
                Console.WriteLine($"{car.Manufacturer} {car.Name} : {car.Combined}");
            }
            Console.WriteLine("***");

            // Will repeat the SQL Select again despite it already knows how many records have returned
            // Every terminal LINQ function call will entail another SQL query
            //Console.WriteLine(topTenFuelEfficientMs.Count());

            var twoMostFuelEfficientByMfgQs = 
                    from c in db.Cars
                    group c by c.Manufacturer into m
                    select new
                    {
                        Name = m.Key,
                        Cars = (from c in m
                               orderby c.Combined descending
                               select c).Take(2)
                    };

            var twoMostFuelEfficientByMfgMs = db.Cars
                .GroupBy(c => c.Manufacturer)
                .Select(gr => new
                {
                    Name = gr.Key,
                    Cars = gr.OrderByDescending(c => c.Combined).Take(2)
                });

            foreach (var gr in twoMostFuelEfficientByMfgQs)
            {
                Console.WriteLine(gr.Name);
                foreach (var car in gr.Cars)
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
            Console.WriteLine("***");
        }

        private static CarDb CreateDatabase()
        {
            var db = new CarDb();

            // Log SQL queries that EF issues
            db.Database.Log = Console.WriteLine;

            return db;
        }

        private static void InsertData()
        {
            var db = CreateDatabase();

            if (!db.Cars.Any())
            {
                var cars = FilterOrderProject.ProcessCars(LinqExt.CsvPath);
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }
    }
}
