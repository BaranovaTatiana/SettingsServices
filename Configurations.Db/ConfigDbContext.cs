using Configurations.Db.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Configurations.Db;

/// <summary>
/// Контекст для работы с базой данных
/// </summary>
public class ConfigDbContext: DbContext
{
    public string DbPath { get; }
    
    /// <inheritdoc />
    public ConfigDbContext(DbContextOptions<ConfigDbContext> options, string dbPath)
        : base(options)
    {
        DbPath = dbPath;
    }

    public ConfigDbContext()
    {
        // const string folder = "Configuration.API";
        // var path = Environment.GetFolderPath(folder);
        // DbPath = Path.Join(path, "configurations.db");
        
        DbPath = @"C:\Users\Alisa\RiderProjects\Configurations\configuration.db";
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=configurations.db");
        }
    }
    
    public DbSet<User> Users { get; init; }
    public DbSet<Color> Colors { get; init; }
    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<ConfigurationVersion> ConfigurationVersions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка уникального индекса для конфигураций (user_id, name)
        modelBuilder.Entity<Configuration>()
            .HasIndex(c => new { c.UserId, c.Name })
            .IsUnique();
           
        modelBuilder.Entity<ConfigurationVersion>().Property(c => c.СreationDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        base.OnModelCreating(modelBuilder);
    }
    
    private string GetConnectionString()
    {
        // Build config
        // IConfiguration config = new ConfigurationBuilder()
        //     .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BimInspector.Api"))
        //     .AddJsonFile("appsettings.json", false, false)
        //     .AddEnvironmentVariables()
        //     .AddUserSecrets(WebAppUserSecretsId)
        //     .Build();
        //
        // return config.GetConnectionString(ConnectionStringName);

        return string.Empty;
    }
}