using SettingsService.API.Models.SettingsPresets;
using Person = SettingsService.API.Models.Person;

namespace SettingsService.API.Abstractions;

public interface ISettingsRepository
{
    List<FullSettingsModel> GetSettingsByDate(DateTime date);
    List<FullSettingsModel> GetSettingsByNameAsync(string name);
    List<FullSettingsModel> GetAllSettings();
    Task<Result> CreateSettings(CreatedSettingsModel settings);
    Task<Result> UpdateSettings(CreatedSettingsModel settings);
    Task<Result> RemoveSettings(string name, Person person);
}