using SettingsService.API.Models.Settings;
using SettingsService.API.Models.SettingsPresets;

namespace SettingsService.API.Abstractions;

public interface ISettingsRepository
{
    Task<Result> CreateSettings(CreatedSettingsModel settings);
    List<FullSettingsModel> GetSettingsByDate(DateTime date);
    List<FullSettingsModel> GetSettingsByNameAsync(string name);
    List<FullSettingsModel> GetAllSettings();
    Task<Result> UpdateSettings(Guid guid, Settings settings);
}