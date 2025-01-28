namespace Configuration.API;

public class VersionSettings
{
    public int VersionNumber { get; set; }

    public Settings? Settings { get; set; } = new();
}