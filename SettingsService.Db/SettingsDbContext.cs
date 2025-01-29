using Microsoft.EntityFrameworkCore;
using SettingsService.Db.Entity;

namespace SettingsService.Db;

/// <summary>
/// Контекст для работы с базой данных
/// </summary>
public class SettingsDbContext: DbContext
{
    /// <inheritdoc />
    public SettingsDbContext(DbContextOptions<SettingsDbContext> options)
        : base(options){}

    public SettingsDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=SettingsService.db");
        }
    }
    
    public DbSet<Person> Person { get; init; }
    public DbSet<SettingsPreset> SettingsPreset { get; set; }
    public DbSet<SettingsPresetVersion> SettingsPresetVersion { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SettingsPreset>()
            .HasIndex(s => new { s.PersonId, s.Name })
            .IsUnique();
           
        modelBuilder.Entity<SettingsPresetVersion>().Property(s => s.CreationDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        modelBuilder.Entity<Person>().Property(s => s.IsDeleted)
            .HasDefaultValue(false);
        
        modelBuilder.Entity<SettingsPreset>().Property(s => s.IsDeleted)
            .HasDefaultValue(false);
        
        modelBuilder.Entity<SettingsPresetVersion>().Property(s => s.IsDeleted)
            .HasDefaultValue(false);
        
        base.OnModelCreating(modelBuilder);
    }
}