using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Configurations.Db.Entity;

public class Configuration
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Key]
    public Guid Guid { get; init; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    public bool IsDeleted {get; set;}


    [ForeignKey("UserId")]
    public User User { get; set; }

    public List<ConfigurationVersion> ConfigurationVersions { get; set; }
}