using Configurations.Db;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Configuration.API;

public class Program
{
    public static IWebHostEnvironment Environment { get; }
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services);
// Add services to the container.

        builder.Services.AddDbContext<ConfigDbContext>(x =>
            x.UseSqlite(builder.Configuration.GetConnectionString(@"C:\Users\Alisa\RiderProjects\Configurations\configuration.db")));


        builder.Services.AddControllers();
        ////builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        ApplyMigrations(app);
// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddDependensies(Environment);
    }
    
    
    // Метод для применения миграций
    private static void ApplyMigrations(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                logger.LogInformation("Start applying migrations");
                var dbContext = scope.ServiceProvider.GetRequiredService<ConfigDbContext>();
                // Применяет миграции к БД
                dbContext.Database.Migrate();
                logger.LogInformation("Finish applying migrations");
            }
            catch (Exception e)
            {
                logger.LogError($"Fail applying migrations: {e.Message}");
                throw; // Позволим приложению упасть, так как это важная операция
            }
        }

    }
}