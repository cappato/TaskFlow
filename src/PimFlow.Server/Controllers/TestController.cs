using Microsoft.AspNetCore.Mvc;

namespace PimFlow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("Test endpoint working!");
    }

    [HttpGet("health")]
    public ActionResult<object> GetHealth()
    {
        return Ok(new { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            message = "Test controller is working"
        });
    }
}
