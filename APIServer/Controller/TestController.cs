using Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Service;


namespace Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestController : ControllerBase
{
    readonly ILogger logger;
    readonly ApplicationDbContext context;

    public TestController(
        ILogger<TestController> logger,
        UserService userService,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ApplicationDbContext context
        )
    {
        this.logger = logger;
        this.context = context;
    }


    [HttpGet]
    public async Task<IActionResult> IsUp()
    {
        logger.LogInformation("IsUp");
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> IsUpAuth()
    {
        return Ok(User.Identity.Name);
    }

    [HttpGet]
    [Authorize(Policy = AppPermission.Manage)]
    public async Task<IActionResult> IsUpClaim()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> TestDBWrite(TestDBWriteRequest request)
    {
        var existing = await context.Tests.FirstOrDefaultAsync(x => x.Key == request.key);
        if (existing == null)
        {
            context.Tests.Add(new TestEntity { Key = request.key, Value = request.value });
        }
        else
        {
            existing.Value = request.value;
        }
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route("{key}")]
    public async Task<IActionResult> TestDBRead(string key)
    {
        var item = await context.Tests.FirstOrDefaultAsync(x => x.Key == key);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }


    public record TestDBWriteRequest(string key, string value);
}
