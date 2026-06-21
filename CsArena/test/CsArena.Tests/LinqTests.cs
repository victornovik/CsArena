using CsArena.Tests.extensions;

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
        var even = LinqExtensions.Random().Where(n => n % 2 == 0).Take(10);
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

    [Fact]
    public void IndexOperator()
    {
        // Index() (.NET 9+): projects each element paired with its 0-based position.
        string[] words = ["alpha", "beta", "gamma", "delta"];

        var indexed = words.Index().ToList();

        Assert.Equal((0, "alpha"), indexed[0]);
        Assert.Equal((1, "beta"),  indexed[1]);
        Assert.Equal((3, "delta"), indexed[3]);
    }

    [Fact]
    public void CountByKey()
    {
        // CountBy (.NET 9+): groups by key selector and returns per-key counts.
        string[] words = ["apple", "banana", "apricot", "avocado", "blackberry"];

        // Groups by the first letter and counts how many words start with each letter
        var counts = words.CountBy(w => w[0]).ToDictionary(p => p.Key, p => p.Value);

        Assert.Equal(3, counts['a']);
        Assert.Equal(2, counts['b']);
    }

    [Fact]
    public void AggregateByKey()
    {
        // AggregateBy (.NET 9+): groups by key and folds each group with a seed + accumulator.
        var orders = new[]
        {
            (Product: "Widget", Revenue: 100),
            (Product: "Gadget", Revenue: 10),
            (Product: "Widget", Revenue: 150),
            (Product: "Gadget", Revenue:  50),
        };

        var revenue = orders
            .AggregateBy(o => o.Product, seed: 0, (total, o) => total + o.Revenue)
            .ToDictionary(p => p.Key, p => p.Value);

        Assert.Equal(250, revenue["Widget"]);
        Assert.Equal(60, revenue["Gadget"]);
    }

    [Fact]
    public void GroupBy()
    {
        var movies = new[]
        {
            new Movie { Title = "The Dark Knight",   Rating = 8.9f, Year = 2008 },
            new Movie { Title = "The King's Speech", Rating = 8.0f, Year = 2010 },
            new Movie { Title = "Casablanca",        Rating = 8.5f, Year = 1942 },
            new Movie { Title = "Star Wars V",       Rating = 8.7f, Year = 1980 },
            new Movie { Title = "Inception",         Rating = 8.8f, Year = 2010 },
        };

        var byDecade = movies
            .GroupBy(m => m.Year / 10 * 10)
            .ToDictionary(g => g.Key, g => g.Select(m => m.Title).OrderBy(t => t).ToList());

        Assert.Equal(4, byDecade.Count);
        Assert.Equal(["Casablanca"],                           byDecade[1940]);
        Assert.Equal(["Star Wars V"],                          byDecade[1980]);
        Assert.Equal(["The Dark Knight"],                      byDecade[2000]);
        Assert.Equal(["Inception", "The King's Speech"],       byDecade[2010]);
    }

    [Fact]
    public void Zip()
    {
        int[]    ranks = [1, 2, 3];
        string[] names = ["Gold", "Silver", "Bronze"];

        var medals = ranks.Zip(names, (rank, name) => $"{rank}:{name}").ToList();

        Assert.Equal(["1:Gold", "2:Silver", "3:Bronze"], medals);
    }

    [Fact]
    public void Aggregate()
    {
        // Factorial via fold
        int factorial = Enumerable.Range(1, 5).Aggregate((acc, n) => acc * n);
        Assert.Equal(120, factorial);

        // Seed-based: join words with spaces
        string[] words = ["Hello", "World", "C#"];
        string sentence = words.Aggregate("", (acc, w) => acc.Length == 0 ? w : $"{acc} {w}");
        Assert.Equal("Hello World C#", sentence);

        // Result selector: collect values then project
        int sumOfSquares = Enumerable.Range(1, 4)
            .Aggregate(0, (acc, n) => acc + n * n, result => result);
        Assert.Equal(30, sumOfSquares); // 1+4+9+16
    }

    [Fact]
    public void Chunk()
    {
        int[] arr = [1, 2, 3, 4, 5, 6, 7];

        var chunks = arr.Chunk(3).ToList();

        Assert.Equal(3, chunks.Count);
        Assert.Equal([1, 2, 3], chunks[0]);
        Assert.Equal([4, 5, 6], chunks[1]);
        Assert.Equal([7],       chunks[2]);
    }

    [Fact]
    public void InnerJoin()
    {
        var developers = new[]
        {
            new Employee { Id = 1, Name = "Scott" },
            new Employee { Id = 2, Name = "Chris" },
            new Employee { Id = 3, Name = "Alice" },
        };
        var assignments = new[]
        {
            (DevId: 1, Project: "LINQ"),
            (DevId: 2, Project: "Roslyn"),
            (DevId: 1, Project: "EF"),
        };

        // Inner join: Alice (Id=3) is excluded — no matching assignment
        var joined = developers
            .Join(assignments, d => d.Id, a => a.DevId, (d, a) => $"{d.Name}:{a.Project}")
            .OrderBy(s => s)
            .ToList();

        Assert.Equal(["Chris:Roslyn", "Scott:EF", "Scott:LINQ"], joined);
    }

    [Fact]
    public void DistinctBy()
    {
        var movies = new[]
        {
            new Movie { Title = "Inception",         Rating = 8.8f, Year = 2010 },
            new Movie { Title = "The King's Speech", Rating = 8.0f, Year = 2010 },
            new Movie { Title = "Casablanca",        Rating = 8.5f, Year = 1942 },
        };

        // One representative per year — first occurrence wins
        var distinctYears = movies.DistinctBy(m => m.Year).Select(m => m.Title).ToList();

        Assert.Equal(["Inception", "Casablanca"], distinctYears);
    }

    [Fact]
    public void MinByMaxBy()
    {
        var movies = new[]
        {
            new Movie { Title = "The Dark Knight",   Rating = 8.9f, Year = 2008 },
            new Movie { Title = "The King's Speech", Rating = 8.0f, Year = 2010 },
            new Movie { Title = "Casablanca",        Rating = 8.5f, Year = 1942 },
        };

        Assert.Equal("The King's Speech", movies.MinBy(m => m.Rating)!.Title);
        Assert.Equal("The Dark Knight",   movies.MaxBy(m => m.Rating)!.Title);
    }
}