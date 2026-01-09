using System.Globalization;

namespace CsArena
{
    public delegate void GradeAddedDelegate(object sender, EventArgs args);

    public class NamedObject(string name)
    {
        public string Name { get; set; } = name; // Auto-property
    }

    public interface IBook
    {
        void AddGrade(double grade);
        void AddGrade(char grade);
        Statistics GetStatistics();
        string Name { get; }
        event GradeAddedDelegate? GradeAdded;
    }

    public abstract class Book(string name) : NamedObject(name), IBook
    {
        public abstract void AddGrade(double grade);

        public void AddGrade(char grade)
        {
            AddGrade(grade switch
            {
                'a' or 'A' => 90,
                'b' or 'B' => 80,
                'c' or 'C' => 70,
                'd' or 'D' => 60,
                _ => 0
            });
        }

        public abstract Statistics GetStatistics();
        public abstract event GradeAddedDelegate? GradeAdded;
    }

    public class DiskBook : Book
    {
        public DiskBook(string name) : base(name)
        {
            using (File.CreateText($"{Name}.txt"))
            {}
        }

        public override void AddGrade(double grade)
        {
            using (var writer = File.AppendText($"{Name}.txt"))
            {
                writer.WriteLine(grade);
            }

            GradeAdded?.Invoke(this, EventArgs.Empty);
        }

        public override Statistics GetStatistics()
        {
            var grades = File.ReadAllLines($"{Name}.txt")
                .Select(grade => double.Parse(grade, CultureInfo.InvariantCulture))
                .ToList();

            return new Statistics(grades);
        }

        public override event GradeAddedDelegate? GradeAdded;
    }

    public class InMemoryBook(string name) : Book(name)
    {
        public override void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException($"Invalid {nameof(grade)}");

            grades.Add(grade);
            GradeAdded?.Invoke(this, EventArgs.Empty);
        }

        public override event GradeAddedDelegate? GradeAdded;

        public override Statistics GetStatistics()
        {
            return new Statistics(grades);
        }

        private readonly List<double> grades = [];
    }
}