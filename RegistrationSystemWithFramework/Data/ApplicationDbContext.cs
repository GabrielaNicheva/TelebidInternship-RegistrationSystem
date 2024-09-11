using Microsoft.EntityFrameworkCore;

namespace RegistrationSystemWithFramework.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;database=WorkoutApp;user=workoutApp;password=WorkoutApp123;",
                    new MySqlServerVersion(new Version(8, 0, 21)));
            }
        }
    }
}
