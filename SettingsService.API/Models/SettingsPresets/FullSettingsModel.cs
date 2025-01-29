using SettingsService.API.Models.Settings;
using SettingsService.API.Models.SettingsPresets;

namespace SettingsService.API;

public class FullSettingsModel: BaseSettingsModel
{
    public List<VersionSettings> AllSettings{ get; set; }
}