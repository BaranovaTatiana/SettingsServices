using System.ComponentModel.DataAnnotations;

namespace SettingsService.API.Models;

public class PersonModel
{
    [Required]
    public string FirstName { get; set; }
    
    public string? MiddleName { get; set; }
    
    [Required]
    public string LastName { get; set; }
}