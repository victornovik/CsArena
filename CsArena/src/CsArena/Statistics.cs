namespace CsArena
{
    public struct Statistics
    {
        public double Avg { get; }
        public double Max { get; }
        public double Min { get; }
        public char Letter
        {
            get
            {
                return Avg switch
                {
                    >= 90.0 => 'A',
                    >= 80.0 => 'B',
                    >= 70.0 => 'C',
                    >= 60.0 => 'D',
                    _ => 'F'
                };
            }
        }

        public Statistics(List<double> grades)
        {
            Avg = grades.Count > 0 ? grades.Average() : 0.0;
            Max = grades.Count > 0 ? grades.Max() : 0.0;
            Min = grades.Count > 0 ? grades.Min() : 0.0;
        }
    }
}