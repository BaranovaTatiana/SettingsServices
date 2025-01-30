using Microsoft.EntityFrameworkCore;
using SettingsService.Db;

namespace SettingsService.API;

public class Program
{
    public static IWebHostEnvironment Environment { get; }
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services);
        
        builder.Services.AddDbContext<SettingsDbContext>(x =>
            x.UseSqlite(builder.Configuration.GetConnectionString("DbConnectionString")));

        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        AddCors(builder);
        builder.Services.AddSignalR();
        
        var app = builder.Build();

        ApplyMigrations(app);
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        app.UseCors("CorsPolicy");
        app.MapHub<SettingsMessageHub>(builder.Configuration.GetSection("SettingsHub").Value!);
        
        app.Run();
    }

    private static void AddCors(WebApplicationBuilder builder)
    {
        // Получаем список разрешенных URL из конфигурации
        var allowedOrigins = builder.Configuration.GetSection("ClientUrs").Get<string[]>();

        if (allowedOrigins is {Length: > 0})
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    corsPolicyBuilder =>
                    {
                        corsPolicyBuilder.WithOrigins(allowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });
        }
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddDependencies(Environment);
    }
    
    private static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            logger.LogInformation("Start applying migrations");
            var dbContext = scope.ServiceProvider.GetRequiredService<SettingsDbContext>();
            
            dbContext.Database.Migrate();
            logger.LogInformation("Finish applying migrations");
        }
        catch (Exception e)
        {
            logger.LogError($"Fail applying migrations: {e.Message}");
            throw;
        }
    }
}