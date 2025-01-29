using Configuration.Db.Entity;
using Microsoft.EntityFrameworkCore;

namespace Configuration.Db;

/// <summary>
/// Контекст для работы с базой данных
/// </summary>
public class ConfigDbContext: DbContext
{
    /// <inheritdoc />
    public ConfigDbContext(DbContextOptions<ConfigDbContext> options)
        : base(options){}

    public ConfigDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=configuration.db");
        }
    }
    
    public DbSet<User> Users { get; init; }
    public DbSet<Entity.Configuration> Configurations { get; set; }
    public DbSet<ConfigurationVersion> ConfigurationVersions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entity.Configuration>()
            .HasIndex(c => new { c.UserId, c.Name })
            .IsUnique();
           
        modelBuilder.Entity<ConfigurationVersion>().Property(c => c.СreationDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        base.OnModelCreating(modelBuilder);
    }
}