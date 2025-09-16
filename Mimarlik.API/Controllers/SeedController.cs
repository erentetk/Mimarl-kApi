using Microsoft.AspNetCore.Mvc;
using Mimarlik.Infrastructure.Services;

namespace Mimarlik.API.Controllers;

[Route("api/[controller]")]
public class SeedController : BaseApiController
{
    private readonly DataSeedService _seedService;

    public SeedController(DataSeedService seedService)
    {
        _seedService = seedService;
    }

    [HttpPost]
    public async Task<ActionResult> SeedTestData()
    {
        try
        {
            await _seedService.SeedAllDataAsync();
            return Ok(new { message = "Test data seeded successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error seeding data: {ex.Message}" });
        }
    }
}