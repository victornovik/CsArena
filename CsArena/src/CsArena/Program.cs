using System.Globalization;
using CsArena;

static void OnGradeAdded(object sender, EventArgs e)
{
    Console.WriteLine("A grade is added");
}

//IBook book = new InMemoryBook("Memory Grades");
IBook book = new DiskBook("Disk Grades");
book.GradeAdded += OnGradeAdded;

EnterGrades(book);

var stat = book.GetStatistics();
Console.WriteLine($"Book named {book.Name}");
Console.WriteLine($"The lowest grade: {stat.Min:N1}");
Console.WriteLine($"The highest grade: {stat.Max:N1}");
Console.WriteLine($"Average grade: {stat.Avg:N1}");
Console.WriteLine($"Letter grade: {stat.Letter}");

void EnterGrades(IBook b)
{
    Console.WriteLine("Enter grade or 'q' to quit");
    for (var input = Console.ReadLine(); !input!.Equals("q", StringComparison.CurrentCultureIgnoreCase); input = Console.ReadLine())
    {
        try
        {
            if (double.TryParse(input, CultureInfo.InvariantCulture, out var floatGrade))
                b.AddGrade(floatGrade);
            else if (char.TryParse(input, out var charGrade))
                b.AddGrade(charGrade);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}