using Microsoft.AspNetCore.Mvc;

namespace Configuration.API.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class UserController(ILogger<ConfigController> logger, IDbManager manager) : ControllerBase
{
    [HttpPost("AddUser")]
    public async Task<IActionResult> AddUser(User user)
    {
        var result = await manager.AddUser(user);
        
        if (result.Status == Status.Error)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Message);
    }
}