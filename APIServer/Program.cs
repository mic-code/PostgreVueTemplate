using Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using System.Text;
using Utility;
using Services;

namespace PropertyAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            GlobalVar.Init(builder.Configuration);

            //ColorConsole.ConfigOption(builder.Logging);

            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(builder.Configuration)
               .CreateLogger();

            builder.Host.UseSerilog();

            var key = Encoding.UTF8.GetBytes(GlobalVar.TokenKey);

            builder.Services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(GlobalVar.DBConn));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = GlobalVar.Issuer,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/userHub"))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var claimsID = context.Principal.Identity as ClaimsIdentity;

                        if (!claimsID.Claims.Any())
                            context.Fail("This token has no claims.");
                        return Task.CompletedTask;
                    },
                };
            });

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });

            //builder.Services.TryAddSingleton(typeof(ILogger<>), typeof(LoggerEx<>));
            builder.Services.AddSingleton<JWTHelper>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<EmailService>();

            // Storage service
            builder.Services.AddSingleton<IStorageServiceFactory, StorageServiceFactory>();


            //if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddSwaggerGen();
                builder.Services.AddOpenApi();
            }
            //else
            {
                builder.Services.AddResponseCompression();
            }

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                builder =>
                {
                    builder
                    .WithOrigins("https://localhost:3050")
                    //.AllowAnyOrigin()
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AppPermission.Manage, policy => policy.RequireClaim(nameof(AppPermission), AppPermission.Manage));
            });
            builder.Services.AddSignalR().AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });
            builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

            var app = builder.Build();
            GlobalVar.IsDev = app.Environment.IsDevelopment();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    await DatabaseUtility.AutoMigrateDatabaseAsync(context, logger);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while setting up the database.");
                    throw;
                }

                try
                {
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                    var userService = services.GetRequiredService<UserService>();
                    await userService.InitRoleAndClaim(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while initializing roles and claims.");
                    throw;
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseResponseCompression();
                app.UseHttpsRedirection();
                app.UseDefaultFiles();
                app.UseStaticFiles();
                app.MapFallbackToFile("index.html");
            }

            // Serve uploaded files from /uploads
            var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
                RequestPath = "/uploads"
            });

            app.UseCors("AllowAll");
            app.UseAuthorization();
            //app.MapHub<UserHub>("/api/userhub", options => { options.AllowStatefulReconnects = true; });
            app.MapControllers();

            app.Run();
        }
    }
}

// Make Program class accessible for integration testing
public partial class Program { }
