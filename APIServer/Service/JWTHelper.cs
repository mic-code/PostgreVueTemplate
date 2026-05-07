using Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service;

public class JWTHelper
{
    readonly string issuer;
    readonly byte[] signKey;
    readonly ILogger logger;

    Dictionary<string, string> refreshTokenToUserMap;


    public JWTHelper(ILogger<JWTHelper> logger, IConfiguration configuration)
    {
        this.logger = logger;
        issuer = configuration.GetValue<string>("Jwt:Issuer");
        var key = configuration.GetValue<string>("Jwt:Key");

        var envIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        if (envIssuer != null)
            issuer = envIssuer;

        var envKey = Environment.GetEnvironmentVariable("JWT_KEY");
        if (envKey != null)
            key = envKey;

        signKey = Encoding.UTF8.GetBytes(key);
        refreshTokenToUserMap = new Dictionary<string, string>();
    }

    public async Task<(SecurityToken, string)> GenerateToken(
        string refreshToken,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager
        )
    {
        var userName = refreshTokenToUserMap[refreshToken];
        var user = await userManager.FindByEmailAsync(userName);
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
        foreach (var roleName in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await roleManager.FindByNameAsync(roleName);
            var roleClaims = await roleManager.GetClaimsAsync(role);

            if (roleClaims != null)
                foreach (var roleClaim in roleClaims)
                    claims.Add(roleClaim);
        }

        logger.LogInformation("Generate Token for {user}", userName);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(4),
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(signKey),
                SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return (token, tokenString);
    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        //logger.LogInformation("ValidateRefreshToken");
        return refreshToken != null && refreshTokenToUserMap.ContainsKey(refreshToken);
    }

    public string GenerateRefreshToken(string userName)
    {
        logger.LogInformation("Generate RefreshToken for {user}", userName);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                //new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim("roles","user")
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(signKey),
                SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        refreshTokenToUserMap[tokenString] = userName;

        return tokenString;
    }
}