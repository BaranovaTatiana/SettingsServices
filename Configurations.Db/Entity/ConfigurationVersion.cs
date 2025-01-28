using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Configurations.Db.Entity;

public class ConfigurationVersion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public Guid ConfigurationGuid { get; init; }

    [Required]
    public int VersionNumber { get; set; }

    [Required]
    public string SettingsData { get; set; }
        
    public DateTime СreationDate { get; set; }
    
    public bool IsDeleted { get; set;}

    [ForeignKey("ConfigurationGuid")]
    public Configuration Configuration { get; set; }
}