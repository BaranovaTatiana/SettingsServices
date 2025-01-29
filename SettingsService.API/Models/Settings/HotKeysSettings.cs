namespace SettingsService.API.Models.Settings;

public class HotKeysSettings
{
    // Словарь для хранения сочетаний клавиш
    public Dictionary<string, string> KeyBindings { get; set; } = new Dictionary<string, string>();
}