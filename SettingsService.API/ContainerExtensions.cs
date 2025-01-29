using SettingsService.API.Abstractions;
using SettingsService.API.Repositories;

namespace SettingsService.API;

public static class ContainerExtensions
{
    public static void AddDependencies(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
    }
}