using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingsService.Db.Entity;

public class Person
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    public string? MiddleName { get; set; }
    
    [Required]
    public string LastName { get; set; }

    public bool IsDeleted { get; set; }
}