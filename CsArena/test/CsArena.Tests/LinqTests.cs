using LinqArena.Common;

namespace CsArena.Tests;

public class LinqTests
{
    /* Immediate query operators: Count, Max, Average, First, ToArray(), ToList(), ...
     *
     * Deferred query operators: Distinct, Select, Take, GroupBy, OrderBy, ...
     *
     *      Streaming operators don't have to read all the source data before they yield elements. 
     *      Take(), Distinct(), Select(), Skip(), Where(), ...
     *
     *      Nonstreaming operators must read all the source data before they can yield a result element.
     *      OrderBy(), GroupBy(), Except(), Intersect(), ThenBy(), ...
     */

    [Fact]
    public void QuerySyntax()
    {
        int[] numbers = [5, 6, 3, 2, 1, 5, 6, 7, 8, 4, 234, 54, 14, 653, 3, 4, 5, 6, 7];

        var query =
            from n in numbers
            where n < 5
            orderby n
            select n;
        var res = string.Join(",", query.Distinct());

        Assert.Equal("1,2,3,4", res);
    }

    [Fact]
    public void MethodSyntax()
    {
        int[] numbers = [5, 6, 3, 2, 1, 5, 6, 7, 8, 4, 234, 54, 14, 653, 3, 4, 5, 6, 7];

        var query = numbers
            .Where(n => n < 5)
            .OrderBy(n => n)
            .Distinct();
        var res = string.Join(",", query);

        Assert.Equal("1,2,3,4", res);
    }

    public class Employee
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    [Fact]
    public void BasicQueries()
    {
        var developers = new List<Employee>()
        {
            new() { Id = 1, Name = "Scott" },
            new() { Id = 2, Name = "Chris" }
        };
        Assert.Equal(2, developers.MyCount());

        var startsWithS = developers.Single(e => e.Name!.StartsWith('S'));
        Assert.Equal("Scott", startsWithS.Name);

        var methodSyntax =
            developers.Where(e => e.Name!.Length == 5)
                .OrderBy(e => e.Name)
                .ToList();
        Assert.Equal(2, methodSyntax.Count);

        var querySyntax =
            (from e in developers
             where e.Name!.Length == 5
             orderby e.Name
             select e).ToList();
        Assert.Equal(methodSyntax, querySyntax);
    }

    [Fact]
    public void ExtensionMethods()
    {
        var sales = new List<Employee>
        {
            new() { Id = 3, Name = "Alex" },
            new() { Id = 3, Name = "Victor" }
        };
        Assert.Equal(2, sales.MyCount());
    }

    public class Movie
    {
        public string? Title { get; set; }
        public float Rating { get; set; }
        public int Year { get; set; }
    }

    [Fact]
    public void CustomLinqOperator()
    {
        var movies = new List<Movie>
        {
            new() { Title = "The Dark Knight", Rating = 8.9f, Year = 2008 },
            new() { Title = "The King's Speech", Rating = 8.0f, Year = 2010 },
            new() { Title = "Casablanca", Rating = 8.5f, Year = 1942 },
            new() { Title = "Star Wars V", Rating = 8.7f, Year = 1980 }
        };

        var methodSyntax = movies.Where(m => m.Year > 2000);

        var customMethodSyntax = movies.Filter(m => m.Year > 2000);

        var querySyntax = from m in movies
                          where m.Year > 2000
                          select m;

        var referenceValue = methodSyntax.ToList();
        Assert.Equal(referenceValue, customMethodSyntax);
        Assert.Equal(referenceValue, querySyntax);
    }

    [Fact]
    public void Cast()
    {
        var objects = new List<object>
        {
            new Movie { Title = "The Dark Knight", Rating = 8.9f, Year = 2008 },
            new Movie { Title = "The King's Speech", Rating = 8.0f, Year = 2010 },
            new Movie { Title = "Casablanca", Rating = 8.5f, Year = 1942 },
            new Movie { Title = "Star Wars V", Rating = 8.7f, Year = 1980 }
        };

        var stronglyTypedList = objects.Cast<Movie>().ToList();

        Assert.Equal(4, stronglyTypedList.Count);

        var starWars = stronglyTypedList.Find(m => m.Year == 1980);

        Assert.NotNull(starWars);
        Assert.Equal("Star Wars V", starWars.Title);

    }

    [Fact]
    public void InfiniteOperator()
    {
        var even = LinqExt.Random().Where(n => n % 2 == 0).Take(10);
        foreach (var n in even)
            Assert.Equal(0, n % 2);
    }

    [Fact]
    public void SelectMany()
    {
        PetOwner[] petOwners =
        [
            new() { Name = "Riga", Pets = ["Scruffy", "Sam"] },
            new() { Name = "Ashkenazi", Pets = ["Walker", "Sugar"] },
            new() { Name = "Price", Pets = ["Scratches", "Diesel"] },
            new() { Name = "Hines", Pets = ["Dusty"] }
        ];

        // Select 1-to-many and flatten the lists
        IEnumerable<string> query1 = petOwners
            .SelectMany(owner => owner.Pets!)
            .Where(pet => pet.StartsWith("S"));
        Assert.Equal(["Scruffy", "Sam", "Sugar", "Scratches"], query1.ToList());

        var query3 = petOwners
            .SelectMany(owner => owner.Pets!, (owner, pet) => (Owner: owner.Name, Pet: pet))
            .Where(p => p.Pet.StartsWith("Scr"));
        Assert.Equal([("Riga", "Scruffy"), ("Price", "Scratches")], query3.ToList());

        // Single Select returns 4 lists in IEnumerable
        IEnumerable<List<string>> query2 = petOwners.Select(petOwner => petOwner.Pets)!;
        Assert.Equal(4, query2.ToList().Count);
    }

    class PetOwner
    {
        public string? Name { get; init; }
        public List<string>? Pets { get; init; }
    }

    [Fact]
    public void TotalEvenNumbers()
    {
        int[] arr = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        const int evenNumbersTotal = 2 + 4 + 6 + 8 + 10;
        
        var res = arr.Sum(n => int.IsEvenInteger(n) ? n : 0);
        Assert.Equal(evenNumbersTotal, res);

        res = arr.Where(int.IsEvenInteger).Sum();
        Assert.Equal(evenNumbersTotal, res);

        // Count all even numbers
        res = arr.Count(i => i % 2 == 0);
        Assert.Equal(5, res);
    }

    [Fact]
    public void Paginate()
    {
        int[] arr = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        const int page = 2;
        const int pageSize = 3;
        Assert.Equal([4, 5, 6], arr.Skip((page - 1) * pageSize).Take(pageSize));
    }
    
    [Fact]
    public void TakeLast()
    {
        int[] arr = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        Assert.Equal([8, 9, 10], arr.TakeLast(3));
    }

    [Fact]
    public void CheckIfOnlySingleElementSatisfies()
    {
        int[] arr = [1, 2, 3, 4, 5, 6, 6, 7, 8, 9, 10];

        Assert.Equal(5, arr.Single((i) => i == 5));

        // More than one elements exist
        Assert.Throws<InvalidOperationException>(() => arr.Single((i) => i == 6));
    }
}