namespace SettingsService.API.Models.SettingsPresets;

public class BaseSettingsModel
{
    public string Name { get; init; }
    
    public Person User { get; init; }
}