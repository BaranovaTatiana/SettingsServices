using System.Text.Json;
using Configurations.Db;
using Configurations.Db.Entity;
using Microsoft.EntityFrameworkCore;

namespace Configuration.API;

public class DbManager : IDbManager
{
    const string ErrorMessage = "Непредвиденная ошибка";
    public async Task<Result> AddUser(User user)
    {
        try
        {
            await using var db = new ConfigDbContext();
            db.Users.Add(new Configurations.Db.Entity.User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
            });

            await db.SaveChangesAsync();

            return new Result(Status.Success, "Пользователь добавлен");
        }
        catch (Exception e)
        {
            return new Result(Status.Error, ErrorMessage);
        }
    }
    

    public async Task<Result> CreateConfiguration(CreatedConfigurationModel config)
    {
        var settingsStr = JsonSerializer.Serialize(config.Settings);

        try
        {
            await using (var db = new ConfigDbContext())
            {
                var userId = GetUserId(config, db);
                if (CheckExistConfig(config, db, userId))
                {
                    return new Result(Status.Error, 
                        "Такая конфигурация уже существует для пользователя " +
                        $"{config.User.FirstName} {config.User.MiddleName} {config.User.LastName}");
                }

                if (userId == 0)
                {
                    return new Result(Status.Error, "Некорректное имя пользователя");
                }

                await AddConfig(db, userId);
            }
            
            return new Result(Status.Success, "Конфигурация успешно добавлена");
        }
        catch (Exception e)
        {
            return new Result(Status.Error, ErrorMessage);
        }

        async Task AddConfig(ConfigDbContext db, int userId)
        {
            var guid = Guid.NewGuid();
            db.Configurations.Add(new Configurations.Db.Entity.Configuration
            {
                Guid = guid,
                Name = config.Name,
                UserId = userId,
                Id = 1
            });

            db.ConfigurationVersions.Add(new ConfigurationVersion()
            {
                ConfigurationGuid = guid,
                VersionNumber = 1,
                SettingsData = settingsStr
            });

            await db.SaveChangesAsync();
        }
    }

    public List<FullConfigurationModel> GetConfigurationsByDateAsync(DateTime date)
    {
        Func<ConfigurationVersion, bool> predicate = c => c.СreationDate.Date == date.Date;

        try
        {
            using var db = new ConfigDbContext();
        
            var data = GetData(db)
                .Where(x => x.ConfigurationVersions.Any(predicate));

            var configs = MapViewConfiguration(data, predicate);//

            return configs;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<FullConfigurationModel> GetConfigurationsByNameAsync(string name)
    {
        using var db = new ConfigDbContext();
        
        var data = GetData(db)
            .Where(c => c.Name == name);

        var configs = MapViewConfiguration(data);

        return configs;
    }

    public List<FullConfigurationModel> GetAllConfigurations()
    {
        using var db = new ConfigDbContext();

        var data = GetData(db);
        var configs = MapViewConfiguration(data);

        return configs;
    }

    public async Task<Result> UpdateConfiguration(Guid guid, Settings settings)
    {
        try
        {
            await using var db = new ConfigDbContext();
            {
                var lastVersionNumber = db.ConfigurationVersions
                    .Where(x => x.ConfigurationGuid == guid)
                    .Max(x => x.VersionNumber);

                db.ConfigurationVersions.Add(new ConfigurationVersion()
                {
                    ConfigurationGuid = guid,
                    VersionNumber = lastVersionNumber + 1,
                    SettingsData = JsonSerializer.Serialize(settings)
                });

                await db.SaveChangesAsync();
            }

            return new Result(Status.Success, "Конфигурация обновлена");
        }
        catch (Exception e)
        {
            return new Result(Status.Error, ErrorMessage);
        }
    }

    private static List<FullConfigurationModel> MapViewConfiguration(IEnumerable<Configurations.Db.Entity.Configuration> configurations,
        Func<ConfigurationVersion, bool>? predicate = null)
    {
        return configurations.Select(c => new FullConfigurationModel
            {
                Name = c.Name,
                AllSettings = c.ConfigurationVersions
                    .Where(predicate ?? (_ => true))
                    .Select(x => new VersionSettings()
                        {
                            VersionNumber = x.VersionNumber,
                            Settings = JsonSerializer.Deserialize<Settings>(x.SettingsData)
                        })
                    .ToList(),
                User = new User { FirstName = c.User.FirstName, MiddleName = c.User.MiddleName, LastName = c.User.LastName}
            })
            .ToList();
    }

    private static IEnumerable<Configurations.Db.Entity.Configuration> GetData(ConfigDbContext db)
    {
        return db.Configurations
            .Include(x => x.ConfigurationVersions)
            .Include(x => x.User);
    }

    private static int GetUserId(CreatedConfigurationModel config, ConfigDbContext db)
    {
        var user = db.Users.FirstOrDefault(user => user.FirstName == config.User.FirstName &&
                                               user.MiddleName == config.User.MiddleName &&
                                               user.LastName == config.User.LastName);

        return user?.Id ?? 0;
    }

    private static bool CheckExistConfig(CreatedConfigurationModel config, ConfigDbContext db, int userId)
    {
        return db.Configurations.Any(c => c.Name == config.Name && c.UserId == userId);
    }
}