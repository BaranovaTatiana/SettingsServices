using Microsoft.AspNetCore.Mvc;

namespace Configuration.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ConfigController(ILogger<ConfigController> logger, IDbManager manager)
    : ControllerBase
{
    private readonly ILogger<ConfigController> _logger = logger;
    
    [HttpPut("CreateConfiguration")]
    public async Task<IActionResult> CreateConfiguration([FromBody]CreatedConfigurationModel config)
    {
        var result = await manager.CreateConfiguration(config);
        if (result.Status == Status.Error)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Message);
    }
    
    [HttpGet("GetConfigurationsByDate")]
    public List<FullConfigurationModel> GetConfigurationsByDate([FromQuery]DateTime date)
    {
        return manager.GetConfigurationsByDateAsync(date);
    }

    [HttpGet("GetConfigurationsByName")]
    public List<FullConfigurationModel> GetConfigurationsByName([FromQuery]string name)
    {
        return manager.GetConfigurationsByNameAsync(name);
    }
    
    [HttpGet("GetAllConfigurations")]
    public List<FullConfigurationModel> GetAllConfigurations()
    {
        return manager.GetAllConfigurations();
    }
    
    
    [HttpPut("UpdateConfiguration")]
    public async Task<IActionResult> UpdateConfiguration([FromQuery]Guid guid, [FromBody] Settings settings)
    {
        var result = await manager.UpdateConfiguration(guid, settings);
        if (result.Status == Status.Error)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Message);
    }
}