using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SettingsService.API.Abstractions;
using SettingsService.API.Models.Settings;
using SettingsService.API.Models.SettingsPresets;
using SettingsService.Db;
using SettingsService.Db.Entity;
using Person = SettingsService.API.Models.Person;

namespace SettingsService.API.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private static SettingsDbContext _dbContext;
    public SettingsRepository(SettingsDbContext dbContext)
    {
       _dbContext = dbContext;
    }

    public async Task<Result> CreateSettings(CreatedSettingsModel settingsModel)
    {
        var settingsStr = JsonSerializer.Serialize(settingsModel.Settings);
        
        var personId = GetUserId(settingsModel);
        if (CheckExistSettings(settingsModel, personId))
        {
            return new Result(Status.Error, 
                "Такая конфигурация уже существует для пользователя " +
                $"{settingsModel.User.FirstName} {settingsModel.User.MiddleName} {settingsModel.User.LastName}");
        }

        if (personId == 0)
        {
            return new Result(Status.Error, "Некорректное имя пользователя");
        }

        await AddSettingsPreset();
            
        return new Result(Status.Success, "Конфигурация успешно добавлена");

        async Task AddSettingsPreset()
        {
            var guid = Guid.NewGuid();
            _dbContext.SettingsPreset.Add(new SettingsPreset
            {
                Guid = guid,
                Name = settingsModel.Name,
                PersonId = personId
            });

            _dbContext.SettingsPresetVersion.Add(new SettingsPresetVersion
            {
                SettingsPresetGuid = guid,
                VersionNumber = 1,
                Settings = settingsStr
            });

            await _dbContext.SaveChangesAsync();
        }
    }

    public List<FullSettingsModel> GetSettingsByDate(DateTime date)
    {
        Func<SettingsPresetVersion, bool> predicate = c => c.CreationDate.Date == date.Date;
        
        var data = GetData()
            .Where(x => x.SettingsPresetVersions.Any(predicate));

        var settings = MapViewSettingsModel(data, predicate);//

        return settings;
    }

    public List<FullSettingsModel> GetSettingsByNameAsync(string name)
    {
        var data = GetData()
            .Where(c => c.Name == name);

        var configs = MapViewSettingsModel(data);

        return configs;
    }

    public List<FullSettingsModel> GetAllSettings()
    {
        var data = GetData();
        var configs = MapViewSettingsModel(data);

        return configs;
    }

    public async Task<Result> UpdateSettings(Guid guid, Settings settings)
    {
        var lastVersionNumber = _dbContext.SettingsPresetVersion
            .Where(x => x.SettingsPresetGuid == guid)
            .Max(x => x.VersionNumber);

        _dbContext.SettingsPresetVersion.Add(new SettingsPresetVersion()
        {
            SettingsPresetGuid = guid,
            VersionNumber = lastVersionNumber + 1,
            Settings = JsonSerializer.Serialize(settings)
        });

        await _dbContext.SaveChangesAsync();

        return new Result(Status.Success, "Конфигурация обновлена");
    }

    private static List<FullSettingsModel> MapViewSettingsModel(IEnumerable<SettingsPreset> configurations,
        Func<SettingsPresetVersion, bool>? predicate = null)
    {
        return configurations.Select(c => new FullSettingsModel
            {
                Name = c.Name,
                AllSettings = c.SettingsPresetVersions
                    .Where(predicate ?? (_ => true))
                    .Select(x => new VersionSettings()
                        {
                            VersionNumber = x.VersionNumber,
                            Settings = JsonSerializer.Deserialize<Settings>(x.Settings)
                        })
                    .ToList(),
                User = new Person { FirstName = c.Person.FirstName, MiddleName = c.Person.MiddleName, LastName = c.Person.LastName}
            })
            .ToList();
    }

    private static IEnumerable<SettingsPreset> GetData()
    {
        return _dbContext.SettingsPreset
            .Include(x => x.SettingsPresetVersions)
            .Include(x => x.Person);
    }

    private static int GetUserId(CreatedSettingsModel settingsModel)
    {
        var user = _dbContext.Person.FirstOrDefault(user => user.FirstName == settingsModel.User.FirstName &&
                                                           user.MiddleName == settingsModel.User.MiddleName &&
                                                           user.LastName == settingsModel.User.LastName);

        var t = _dbContext.Person.Select(x => settingsModel.User.Equals(x));////
        return user?.Id ?? 0;
    }

    private static bool CheckExistSettings(CreatedSettingsModel settingsModel, int personId)
    {
        return _dbContext.SettingsPreset.Any(c => c.Name == settingsModel.Name && c.PersonId == personId);
    }
}