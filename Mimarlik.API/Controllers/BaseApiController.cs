using Microsoft.AspNetCore.Mvc;

namespace Mimarlik.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult HandleResult<T>(T result)
    {
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    protected ActionResult HandleBoolResult(bool result, string successMessage = "Operation completed successfully")
    {
        if (result)
            return Ok(new { message = successMessage });
            
        return BadRequest(new { message = "Operation failed" });
    }
}