namespace MinimalAPI.Infrastructure.Db;

    using Microsoft.EntityFrameworkCore;

    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {
        }

        // Define your DbSets here
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
