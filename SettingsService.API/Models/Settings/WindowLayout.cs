namespace SettingsService.API.Models.Settings;

public class WindowLayout
{
    public Dictionary<string, WindowPosition> WindowPositions { get; set; } = new Dictionary<string, WindowPosition>();
}