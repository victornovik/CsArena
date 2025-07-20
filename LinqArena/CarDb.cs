using System.Data.Entity;

namespace LinqArena
{
    public class CarDb : DbContext
    {
        public DbSet<Car> Cars { get; set; } = null!;
    }
}
