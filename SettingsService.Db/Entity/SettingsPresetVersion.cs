using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingsService.Db.Entity;

public class SettingsPresetVersion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public Guid SettingsPresetGuid { get; init; }

    [Required]
    public int VersionNumber { get; set; }

    [Required]
    public string Settings { get; set; }
        
    public DateTime CreationDate { get; set; }
    
    public bool IsDeleted { get; set;}
}