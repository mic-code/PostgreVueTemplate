using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service;

namespace Utility;

public static class DatabaseUtility
{
    public static async Task AutoMigrateDatabaseAsync(ApplicationDbContext context, ILogger logger)
    {
        // Check if we have migrations in the assembly first (doesn't require DB connection)
        var migrationsAssembly = context.Database.GetMigrations().ToList();
        
        if (migrationsAssembly.Any())
        {
            // Migrations exist in code - use migration system
            logger.LogInformation("Found {Count} migrations in assembly.", migrationsAssembly.Count);
            
            // Check if database can connect before querying migrations history
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                logger.LogInformation("Cannot connect to database. Creating and applying migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database created and migrations applied successfully.");
                return;
            }
            
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();
            var appliedMigrations = (await context.Database.GetAppliedMigrationsAsync()).ToList();
            
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count);
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
            else if (appliedMigrations.Any())
            {
                // Migrations applied but verify tables exist
                try
                {
                    // Try a simple query to verify schema exists
                    await context.Roles.AnyAsync();
                    logger.LogInformation("Database is up to date. No migrations needed.");
                }
                catch
                {
                    // Tables don't exist but migrations say they do - reset
                    logger.LogWarning("Migration history exists but tables are missing. Recreating database...");
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Database recreated successfully.");
                }
            }
            else
            {
                // No migrations applied yet - apply them
                logger.LogInformation("Applying initial migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
        }
        else
        {
            // No migrations in assembly - use EnsureCreated
            logger.LogInformation("No migrations found in assembly. Setting up database...");
            
            // Check if database can connect
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                logger.LogInformation("Cannot connect to database. Creating...");
                var created = await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Database created: {Created}", created);
            }
            else
            {
                logger.LogInformation("Database connection successful. Checking tables...");
                
                // Check if AspNetRoles table exists
                try
                {
                    var roleCount = await context.Roles.CountAsync();
                    logger.LogInformation("Schema verified. {Count} roles found.", roleCount);
                }
                catch
                {
                    // Tables don't exist - create schema using GenerateCreateScript
                    logger.LogInformation("Tables not found. Creating schema...");
                    var createScript = context.Database.GenerateCreateScript();
                    await context.Database.ExecuteSqlRawAsync(createScript);
                    logger.LogInformation("Schema created successfully.");
                }
            }
        }
    }
}
