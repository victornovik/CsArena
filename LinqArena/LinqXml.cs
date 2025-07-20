using System.Xml.Linq;
using LinqArena.Common;

namespace LinqArena
{
    internal class LinqXml
    {
        private static readonly XNamespace Ns = "http://LinqLearning.com/2024";
        private static readonly XNamespace Ex = "http://LinqLearning.com/2024/ex";

        public static void Playground()
        {
            CreateXml();
            QueryXml();
        }

        private static void QueryXml()
        {
            var doc = XDocument.Load(LinqExt.XmlPath);

            var allBmwCars =
                from el in doc.Element(Ns + "Cars")?.Elements(Ex + "Car") 
                                                        ?? Enumerable.Empty<XElement>() // in case Element("Cars") returns null
                where el.Attribute("Manufacturer")?.Value == "BMW"
                select el.Attribute("Name")?.Value;

            foreach (var car in allBmwCars)
            {
                Console.WriteLine($"{car}");
            }
            Console.WriteLine("\n***\n");
        }

        public static void CreateXml()
        {
            var records = FilterOrderProject.ProcessCars(LinqExt.CsvPath);
            var cars = new XElement(Ns + "Cars",
                from r in records
                select new XElement(Ex + "Car",
                    new XAttribute("Name", r.Name ?? string.Empty),
                    new XAttribute("Combined", r.Combined),
                    new XAttribute("Manufacturer", r.Manufacturer ?? string.Empty)));

            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", Ex));

            var doc = new XDocument(cars);
            doc.Save(LinqExt.XmlPath);
        }
    }
}
