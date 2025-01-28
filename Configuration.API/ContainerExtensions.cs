namespace Configuration.API;

public static class ContainerExtensions
{
    public static void AddDependensies(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddScoped<IDbManager, DbManager>();
    }
}