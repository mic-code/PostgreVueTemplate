using Models;
using Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using Identity;
using System.ComponentModel.DataAnnotations;

namespace Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    readonly JWTHelper tokenHelper;
    readonly ILogger logger;
    readonly EmailService emailService;
    readonly UserManager<AppUser> userManager;
    readonly RoleManager<AppRole> roleManager;

    public AuthController(
        JWTHelper tokenHelper,
        EmailService emailService,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ILogger<AuthController> logger)
    {
        this.tokenHelper = tokenHelper;
        this.emailService = emailService;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Register(SignInRequest request)
    {
        //if (GlobalVar.IsDevelopment)
        //    return BadRequest(AppResult.RegistrationClosed);

        logger.LogInformation("Registering {userName}", request.Email);
        var user = await userManager.FindByNameAsync(request.Email);
        if (user == null)
        {
            user = new AppUser { UserName = request.Email, Email = request.Email };
            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await SendConfirmEmailAsync(user);
                logger.LogInformation("User {userName} registered", request.Email);
                return Ok(AppResult.Success);
            }

            logger.LogInformation("Registration error: {error}", result.Errors.First().Description);
            return BadRequest(new { result = result.Errors.First().Description });
        }
        else
        {
            if (!user.EmailConfirmed)
            {
                await SendConfirmEmailAsync(user);
                logger.LogInformation("Account {userName} exist but not confirmed, confirmation email sent", request.Email);
                return Ok(AppResult.UserExistNotConfirmed);
            }

            logger.LogInformation("Account {userName} exist and confirmed", request.Email);
            return BadRequest(AppResult.UserExist);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        logger.LogInformation("Confirming Email {email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            logger.LogInformation("Confirming Email {email} user not exist", request.Email);
            return BadRequest(AppResult.UserNotExist);
        }

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (result.Succeeded)
        {
            logger.LogInformation("Confirming Email {email} successful", request.Email);

            return Ok(AppResult.Success);
        }
        else
        {
            logger.LogInformation("Confirming Email {email} error:{error}", request.Email, result.Errors.First().Description);
            return BadRequest(new { result = result.Errors.First().Description });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(PasswordResetRequest request)
    {
        logger.LogInformation("ResetPassword {email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(AppResult.UserNotExist);
        var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (result.Succeeded)
            return Ok(AppResult.PassResetComplete);
        else
        {
            if (result.Errors.First().Description == "Invalid token.")
                return BadRequest(AppResult.InvalidToken);
            else
                return BadRequest(new { result = result.Errors.First().Description });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ForgetPassword(ResetRequest request)
    {
        logger.LogInformation("Request Password Reset {email}", request.Email);
        var user = await userManager.FindByNameAsync(request.Email);
        if (user != null)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var payload = new { email = user.Email, token };
            var endpoint = GlobalVar.Endpoint ?? "http://localhost:3050";
            var link = $"{endpoint}/resetPass{URLUtility.GetQueryString(payload)}";

            await emailService.SendForgetPasswordEmail(user.Email, link);

            return Ok(AppResult.Success);
        }
        else
        {
            return BadRequest(AppResult.UserNotExist);
        }
    }

    async Task SendConfirmEmailAsync(AppUser user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = GetLink(new { token, email = user.Email });
        logger.LogInformation(link);
        logger.LogInformation("SendConfirmEmail to {email}", user.Email);
        await emailService.SendConfirmEmail(user.Email, link);
    }

    string GetLink(object payload)
    {
        var endpoint = GlobalVar.Endpoint ?? "http://localhost:3050";
        return $"{endpoint}/confirmEmail{URLUtility.GetQueryString(payload)}";
    }

    [HttpPost]
    public async Task<IActionResult> Signin(SignInRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, request.Password))
        {
            if (user.EmailConfirmed)
            {
                logger.LogInformation("User {user} signed in", request.Email);

                var refreshToken = tokenHelper.GenerateRefreshToken(request.Email);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Path = "/api/Auth/GetToken",
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

                return Ok(AppResult.Success);
            }
            return Unauthorized(AppResult.NeedEmailConfirm);
        }
        return Unauthorized(AppResult.IncorrectCredential);
    }


    [HttpGet]
    public IActionResult Signout()
    {
        Response.Cookies.Delete("refreshToken");
        return Ok();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var user = await userManager.FindByEmailAsync(User.Identity.Name);
        var Roles = await userManager.GetRolesAsync(user);
        return Ok(new { user.UserName, Roles, user.EmailConfirmed });
    }

    [HttpGet]
    public async Task<IActionResult> GetToken()
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (tokenHelper.ValidateRefreshToken(refreshToken))
            {
                var (meta, token) = await tokenHelper.GenerateToken(refreshToken, userManager, roleManager);

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
}

public class SignInRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class ConfirmEmailRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Token { get; set; } = string.Empty;
}

public class PasswordResetRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Token { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class ResetRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}