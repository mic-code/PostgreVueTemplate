using Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Service;

namespace Controller;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    readonly ILogger logger;
    readonly JWTHelper tokenHelper;
    readonly UserManager<AppUser> userManager;
    readonly RoleManager<AppRole> roleManager;

    public AuthController(
        ILogger<AuthController> logger,
        JWTHelper tokenHelper,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager
        )
    {
        this.logger = logger;
        this.tokenHelper = tokenHelper;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    [HttpPost]
    public async Task<IActionResult> Signin(SigninRequest request)
    {
        logger.LogInformation("User {email} attempt signin", request.Email);
        var userCount = userManager.Users.Count();
        logger.LogInformation("UserCount {count}", userCount);

        if (userCount == 0)
        {
            //register first user if none exist
            await Register(request);
        }

        var user = await userManager.FindByNameAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(AppResult.IncorrectCredential);

        if (!user.EmailConfirmed)
            return Unauthorized(AppResult.NeedEmailConfirm);

        logger.LogInformation("User {user} signed in", request.Email);

        var refreshToken = tokenHelper.GenerateRefreshToken(request.Email);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

        return Ok(AppResult.Success);
    }

    async Task<IActionResult> Register(SigninRequest request)
    {
        logger.LogInformation("Registering {userName}", request.Email);
        var user = await userManager.FindByNameAsync(request.Email);
        if (user == null)
        {
            user = new AppUser { UserName = request.Email, Email = request.Email };
            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                //confirm email directly
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                await userManager.ConfirmEmailAsync(user, token);
                //SendConfirmEmail(user);
                logger.LogInformation("User {userName} registered", request.Email);
                return Ok(AppResult.Success);
            }

            logger.LogInformation("Registration error: {error}", result.Errors.First().Description);
            return BadRequest(new { result = result.Errors.First().Description });
        }
        else
        {
            //if (!user.EmailConfirmed)
            //    SendConfirmEmail(user);

            logger.LogInformation("Account {userName} exist", request.Email);
            return BadRequest(AppResult.UserExist);
        }
    }

    [HttpGet]
    public IActionResult Signout()
    {
        Response.Cookies.Delete("refreshToken");
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetToken()
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (tokenHelper.ValidateRefreshToken(refreshToken))
            {
                var (meta, token) = await tokenHelper.GenerateToken(refreshToken, userManager,roleManager);

                return Ok(new { meta, token });
            }
            else
                return BadRequest();
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetToken Error");
            return BadRequest();
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var user = await userManager.FindByEmailAsync(User.Identity.Name);
        var Roles = await userManager.GetRolesAsync(user);
        return Ok(new { user.UserName, Roles, user.EmailConfirmed });
    }
}

public record SigninRequest(string Email, string Password);
