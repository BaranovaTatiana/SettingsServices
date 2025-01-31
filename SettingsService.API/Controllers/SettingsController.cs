using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SettingsService.API.Abstractions;
using SettingsService.API.Models;
using SettingsService.API.Models.SettingsPresets;

namespace SettingsService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SettingsController(ISettingsRepository repository, IHubContext<SettingsMessageHub> hubContext)
    : ControllerBase
{
    [HttpPut("CreateSettings")]
    public async Task<IActionResult> CreateSettings([FromBody]CreatedSettingsModel config)
    {
        var result = await repository.CreateSettings(config);
        if (result.Status == Status.Error)
        {
            await hubContext.Clients.Group("CreateSettings").SendAsync("ReceiveCreateSettingsMessage", result.Message);
            return BadRequest(result.Message);
        }
        
        await hubContext.Clients.Group("CreateSettings").SendAsync("ReceiveCreateSettingsMessage", result.Message);
        return Ok(result.Message);
    }
    
    [HttpPut("UpdateSettings")]
    public async Task<IActionResult> UpdateSettings([FromBody] CreatedSettingsModel settings)
    {
        var result = await repository.UpdateSettings(settings);
        if (result.Status == Status.Error)
        {
            return BadRequest(result.Message);
        }

        await hubContext.Clients.Group("UpdateSettings").SendAsync("ReceiveUpdateSettingsMessage", result.Message);
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
    
    [HttpDelete("RemoveSettings")]
    public async Task<Result> RemoveSettings(string name, PersonModel person)
    {
        var result =  await repository.RemoveSettings(name, person);
        await hubContext.Clients.Group("RemoveSettings").SendAsync("ReceiveRemoveSettingsMessage", result.Message);
        
        return result;
    }
}