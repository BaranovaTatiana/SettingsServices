using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Configuration.Db.Entity;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    public string? MiddleName { get; set; }
    
    [Required]
    public string LastName { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; }
}