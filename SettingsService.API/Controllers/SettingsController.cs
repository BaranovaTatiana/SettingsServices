using Microsoft.AspNetCore.Mvc;
using SettingsService.API.Abstractions;
using SettingsService.API.Models.Settings;
using SettingsService.API.Models.SettingsPresets;

namespace SettingsService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SettingsController(ILogger<SettingsController> logger, ISettingsRepository repository)
    : ControllerBase
{
    private readonly ILogger<SettingsController> _logger = logger;//
    
    [HttpPut("CreateSettings")]
    public async Task<IActionResult> CreateSettings([FromBody]CreatedSettingsModel config)
    {
        var result = await repository.CreateSettings(config);
        if (result.Status == Status.Error)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Message);
    }
    
    [HttpGet("GetSettingsByDate")]
    public List<FullSettingsModel> GetSettingsByDate([FromQuery]DateTime date)
    {
        return repository.GetSettingsByDate(date);
    }

    [HttpGet("GetSettingsByName")]
    public List<FullSettingsModel> GetSettingsByName([FromQuery]string name)
    {
        return repository.GetSettingsByNameAsync(name);
    }
    
    [HttpGet("GetAllSettings")]
    public List<FullSettingsModel> GetAllSettings()
    {
        return repository.GetAllSettings();
    }
    
    [HttpPut("UpdateSettings")]
    public async Task<IActionResult> UpdateSettings([FromQuery]Guid guid, [FromBody] Settings settings)
    {
        var result = await repository.UpdateSettings(guid, settings);
        if (result.Status == Status.Error)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Message);
    }
}