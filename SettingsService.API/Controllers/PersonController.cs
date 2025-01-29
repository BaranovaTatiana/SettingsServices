using Microsoft.AspNetCore.Mvc;
using SettingsService.API.Abstractions;
using SettingsService.API.Models;

namespace SettingsService.API.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class PersonController(ILogger<SettingsController> logger, IPersonRepository repository) : ControllerBase
{
    [HttpPost("CreatePerson")]
    public async Task<IActionResult> CreatePerson(Person person)
    {
        var result = await repository.CreatePerson(person);
        
        if (result.Status == Status.Error)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Message);
    }
}