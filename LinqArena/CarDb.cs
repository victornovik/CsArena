using Microsoft.EntityFrameworkCore;

namespace LinqArena
{
    public class CarDb : DbContext
    {
        public DbSet<Car> Cars { get; set; } = null!;
    }
}
