using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Configuration.Db.Entity;

public class Configuration
{
    [Key]
    public Guid Guid { get; init; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted {get; set;}

    public User User { get; set; }

    public List<ConfigurationVersion> ConfigurationVersions { get; set; }
}