namespace Configuration.API;

public class Settings
{
    public HotKeysSettings HotKeys { get; set; }
    public ColorSchemeSettings ColorScheme { get; set; }
    public FontSettings Font { get; set; }
    public Dictionary<string, WindowPosition> WindowPositions { get; set; }
}