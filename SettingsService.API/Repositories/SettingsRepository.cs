using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SettingsService.API.Abstractions;
using SettingsService.API.Models;
using SettingsService.API.Models.Settings;
using SettingsService.API.Models.SettingsPresets;
using SettingsService.Db;
using SettingsService.Db.Entity;

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
        
        var personId = GetPersonId(settingsModel.Person);
        if (CheckExistSettings(settingsModel, personId))
        {
            return new Result(Status.Error, 
                "Такая конфигурация уже существует для пользователя " +
                $"{settingsModel.Person.FirstName} {settingsModel.Person.MiddleName} {settingsModel.Person.LastName}");
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

    public async Task<Result> UpdateSettings(CreatedSettingsModel settings)
    {
        var personId = GetPersonId(settings.Person);
        
        var guid = _dbContext.SettingsPreset
            .FirstOrDefault(s => s.Name == settings.Name && s.PersonId == personId)?.Guid;

        if (guid == default)
        {
            return new Result(Status.Error, "Такой конфигурации не существует");
        }
        
        var lastVersionNumber = _dbContext.SettingsPresetVersion
            .Where(x => x.SettingsPresetGuid == guid)
            .Max(x => x.VersionNumber);

        _dbContext.SettingsPresetVersion.Add(new SettingsPresetVersion
        {
            SettingsPresetGuid = guid.Value,
            VersionNumber = lastVersionNumber + 1,
            Settings = JsonSerializer.Serialize(settings)
        });

        await _dbContext.SaveChangesAsync();

        return new Result(Status.Success, $"Конфигурация {settings.Name} обновлена " +
                                          $"для пользователя {settings.Person.FirstName} {settings.Person.MiddleName} {settings.Person.LastName}");
    }

    public async Task<Result> RemoveSettings(string name, PersonModel person)
    {
        var personId = GetPersonId(person);
        var settingsPreset = _dbContext.SettingsPreset
            .FirstOrDefault(s => s.Name == name && s.PersonId == personId);

        if (settingsPreset == null)
        {
            return new Result(Status.Error, "Такой конфигурации не существует");
        }

        _dbContext.SettingsPreset.Remove(settingsPreset);
        await _dbContext.SaveChangesAsync();

        return new Result(Status.Success, $"Конфигурация {name} удалена " +
                                          $"для пользователя {person.FirstName} {person.MiddleName} {person.LastName}");
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
                Person = new PersonModel { FirstName = c.Person.FirstName, MiddleName = c.Person.MiddleName, LastName = c.Person.LastName}
            })
            .ToList();
    }

    private static IEnumerable<SettingsPreset> GetData()
    {
        return _dbContext.SettingsPreset
            .Include(x => x.SettingsPresetVersions)
            .Include(x => x.Person);
    }

    private static int GetPersonId(PersonModel person)
    {
        var user = _dbContext.Person.FirstOrDefault(user => user.FirstName == person.FirstName &&
                                                           user.MiddleName == person.MiddleName &&
                                                           user.LastName == person.LastName);
        
        return user?.Id ?? 0;
    }

    private static bool CheckExistSettings(CreatedSettingsModel settingsModel, int personId)
    {
        return _dbContext.SettingsPreset.Any(c => c.Name == settingsModel.Name && c.PersonId == personId);
    }
}