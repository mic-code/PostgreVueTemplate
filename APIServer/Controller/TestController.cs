using Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using Utility;


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

    /// <summary>
    /// Dev-only endpoint: retrieves the last email token captured by EmailService in development mode.
    /// </summary>
    [HttpGet]
    public IActionResult GetDevEmailToken(string email)
    {
        if (!GlobalVar.IsDev)
            return NotFound();

        if (EmailService.DevEmailStore.TryGetValue(email, out var record))
            return Ok(new { record.Email, record.Token, record.Type });

        return NotFound();
    }

    /// <summary>
    /// Dev-only endpoint: clears the dev email token store.
    /// </summary>
    [HttpPost]
    public IActionResult ClearDevEmailStore()
    {
        if (!GlobalVar.IsDev)
            return NotFound();

        EmailService.DevEmailStore.Clear();
        return Ok();
    }

    public record TestDBWriteRequest(string key, string value);
}
