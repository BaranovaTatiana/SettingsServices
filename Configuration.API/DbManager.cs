using System.Text.Json;
using Configuration.Db;
using Configuration.Db.Entity;
using Microsoft.EntityFrameworkCore;

namespace Configuration.API;

public class DbManager : IDbManager
{
    private static ConfigDbContext _dbContext;
    public DbManager(ConfigDbContext dbContext)
    {
       _dbContext = dbContext;
    }

    public async Task<Result> AddUser(User user)
    {
            _dbContext.Users.Add(new Db.Entity.User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
            });

            await _dbContext.SaveChangesAsync();

            return new Result(Status.Success, "Пользователь добавлен");
    }
    

    public async Task<Result> CreateConfiguration(CreatedConfigurationModel config)
    {
        var settingsStr = JsonSerializer.Serialize(config.Settings);

        
                var userId = GetUserId(config);
                if (CheckExistConfig(config, userId))
                {
                    return new Result(Status.Error, 
                        "Такая конфигурация уже существует для пользователя " +
                        $"{config.User.FirstName} {config.User.MiddleName} {config.User.LastName}");
                }

                var t = _dbContext.Users.Select(x => config.User.Equals(x));

                if (userId == 0)
                {
                    return new Result(Status.Error, "Некорректное имя пользователя");
                }

                await AddConfig();
            
            return new Result(Status.Success, "Конфигурация успешно добавлена");

        async Task AddConfig()
        {
            var guid = Guid.NewGuid();
            _dbContext.Configurations.Add(new Db.Entity.Configuration
            {
                Guid = guid,
                Name = config.Name,
                UserId = userId
            });

            _dbContext.ConfigurationVersions.Add(new ConfigurationVersion()
            {
                ConfigurationGuid = guid,
                VersionNumber = 1,
                SettingsData = settingsStr
            });

            await _dbContext.SaveChangesAsync();
        }
    }

    public List<FullConfigurationModel> GetConfigurationsByDateAsync(DateTime date)
    {
        Func<ConfigurationVersion, bool> predicate = c => c.СreationDate.Date == date.Date;
            var data = GetData()
                .Where(x => x.ConfigurationVersions.Any(predicate));

            var configs = MapViewConfiguration(data, predicate);//

            return configs;
    }

    public List<FullConfigurationModel> GetConfigurationsByNameAsync(string name)
    {
        var data = GetData()
            .Where(c => c.Name == name);

        var configs = MapViewConfiguration(data);

        return configs;
    }

    public List<FullConfigurationModel> GetAllConfigurations()
    {
        using var db = new ConfigDbContext();

        var data = GetData();
        var configs = MapViewConfiguration(data);

        return configs;
    }

    public async Task<Result> UpdateConfiguration(Guid guid, Settings settings)
    {
                var lastVersionNumber = _dbContext.ConfigurationVersions
                    .Where(x => x.ConfigurationGuid == guid)
                    .Max(x => x.VersionNumber);

                _dbContext.ConfigurationVersions.Add(new ConfigurationVersion()
                {
                    ConfigurationGuid = guid,
                    VersionNumber = lastVersionNumber + 1,
                    SettingsData = JsonSerializer.Serialize(settings)
                });

                await _dbContext.SaveChangesAsync();

            return new Result(Status.Success, "Конфигурация обновлена");
    }

    private static List<FullConfigurationModel> MapViewConfiguration(IEnumerable<Db.Entity.Configuration> configurations,
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

    private static IEnumerable<Db.Entity.Configuration> GetData()
    {
        return _dbContext.Configurations
            .Include(x => x.ConfigurationVersions)
            .Include(x => x.User);
    }

    private static int GetUserId(CreatedConfigurationModel config)
    {
        var user = _dbContext.Users.FirstOrDefault(user => user.FirstName == config.User.FirstName &&
                                                           user.MiddleName == config.User.MiddleName &&
                                                           user.LastName == config.User.LastName);

        return user?.Id ?? 0;
    }

    private static bool CheckExistConfig(CreatedConfigurationModel config, int userId)
    {
        return _dbContext.Configurations.Any(c => c.Name == config.Name && c.UserId == userId);
    }
}