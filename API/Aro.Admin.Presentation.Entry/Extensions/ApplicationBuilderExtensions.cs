using Aro.Admin.Application.Mediator.Migration.Commands;
using Aro.Admin.Application.Mediator.Seed.Commands;
using Aro.Admin.Application.Mediator.SystemSettings.Commands;
using Aro.Admin.Application.Services.SystemSettings;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Presentation.Entry.ServiceInstallers;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.SystemContext;
using MediatR;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Aro.Admin.Presentation.Entry.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void InstallServices(this WebApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var installers = assembly.DefinedTypes
            .Where(t => typeof(IServiceInstaller).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>();

        foreach (var installer in installers)
        {
            installer.Install(builder);
        }
    }

    public static async Task SeedDatabase(this IApplicationBuilder app, string jsonPath, string emailTemplatesDirectory)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogManager>();
        logger.LogDebug("Starting database seed.");

        logger.LogWarn("Enabling the system context.");
        var systemContextFactory = scope.ServiceProvider.GetRequiredService<ISystemContextFactory>();
        using var systemContext = systemContextFactory.Create();

        logger.LogDebug("Checking if database is already seeded.");
        var systemSettingService = scope.ServiceProvider.GetRequiredService<ISystemSettingsService>();
        var alreadySeeded = await systemSettingService.IsApplicationSeededAtStartup().ConfigureAwait(false);
        if (alreadySeeded)
        {
            logger.LogWarn("Database is already seeded. No futher action required.");
            return;
        }

        logger.LogDebug($"Database needs to be seeded. Seeding now.");

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new SeedApplicationCommand(jsonPath, emailTemplatesDirectory));
    }

    public static async Task MigrateDatabase(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogManager>();
        logger.LogDebug($"Starting database migrations.");

        logger.LogWarn("Enabling the system context.");
        var systemContextFactory = scope.ServiceProvider.GetRequiredService<ISystemContextFactory>();
        using var systemContext = systemContextFactory.Create();

        logger.LogDebug("Checking if database is already migrated.");
        var systemSettingService = scope.ServiceProvider.GetRequiredService<ISystemSettingsService>();
        //var alreadyMigrated = false;  // no need to check this since EF Core handles this automatically and incrementally.
        //try
        //{
        //    alreadyMigrated = await systemSettingService.IsMigrationComplete().ConfigureAwait(false);
        //}
        //catch
        //{
        //    // intentionally empty to catch the case: for the first time when the app is launched, the db won't probably exist, in that case we assume that db needs to be created and migrated.
        //}
        //if (alreadyMigrated)
        //{
        //    logger.LogWarn("Database is already migrated. No futher action required.");
        //    return;
        //}

        logger.LogDebug($"Database needs to be migrated. Applying the migrations now.");

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var dropFirst = configuration.GetValue<bool>("MigrationOptions:DropDatabaseFirst");
        await mediator.Send(new MigrateDatabaseCommand(dropFirst)).ConfigureAwait(false);

        logger.LogDebug("Database migrations completed.");
    }

    public static async Task CreateBootstrapUser(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogManager>();
        logger.LogDebug("Creating bootstrap user.");

        logger.LogWarn("Enabling the system context.");
        var systemContextFactory = scope.ServiceProvider.GetRequiredService<ISystemContextFactory>();
        using var systemContext = systemContextFactory.Create();

        logger.LogDebug("Checking if bootstrap user already exists.");
        var systemSettingService = scope.ServiceProvider.GetRequiredService<ISystemSettingsService>();
        var alreadyExists = await systemSettingService.IsSystemInitialized().ConfigureAwait(false);
        if (alreadyExists)
        {
            logger.LogWarn("Bootstrap user already exists. No futher action required.");
            return;
        }

        logger.LogDebug($"Creating bootstrap user.");

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var bootstrapUserSettings = scope.ServiceProvider.GetRequiredService<IOptions<BootstrapUserSettings>>().Value;
        var adminSettings = scope.ServiceProvider.GetRequiredService<IOptions<AdminSettings>>().Value;
        await mediator.Send(new InitializeSystemCommand(new(bootstrapUserSettings.Email, bootstrapUserSettings.Password, bootstrapUserSettings.DisplayName, adminSettings.BootstrapPassword)));

        logger.LogDebug("Bootstrap user created.");

        await systemSettingService.SetSystemStateToInitialized().ConfigureAwait(false);

        logger.LogDebug("System state set to initialized.");
    }
}