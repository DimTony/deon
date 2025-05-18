using Microsoft.EntityFrameworkCore;

namespace YourAppName.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define your DbSet properties here
        // public DbSet<YourModel> YourModels { get; set; }
    }
}