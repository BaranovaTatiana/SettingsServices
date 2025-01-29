using System.ComponentModel.DataAnnotations;

namespace SettingsService.Db.Entity;

public class SettingsPreset
{
    [Key]
    public Guid Guid { get; init; }

    [Required]
    public int PersonId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    public bool IsDeleted {get; set;}

    public Person Person { get; set; }

    public List<SettingsPresetVersion> SettingsPresetVersions { get; set; }
}