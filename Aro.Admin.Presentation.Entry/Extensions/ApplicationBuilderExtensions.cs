using Aro.Admin.Application.Mediator.Migration.Commands;
using Aro.Admin.Application.Mediator.Seed.Commands;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Presentation.Entry.ServiceInstallers;
using MediatR;
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

    public static async Task SeedDatabase(this IApplicationBuilder app, string jsonPath)
    {
        ISystemContext? systemContext = null;

        try
        {
            using var scope = app.ApplicationServices.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogManager>();
            logger.LogDebug("Starting database seed.");

            logger.LogWarn("Enabling the system context.");
            systemContext = scope.ServiceProvider.GetRequiredService<ISystemContext>();
            systemContext.IsSystemContext = true;

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

            await mediator.Send(new SeedApplicationCommand(jsonPath));
        }
        finally
        {
            if (systemContext is not null)
            {
                systemContext.IsSystemContext = false;
            }
        }
    }

    public static async Task MigrateDatabase(this IApplicationBuilder app)
    {
        ISystemContext? systemContext = null;

        try
        {
            using var scope = app.ApplicationServices.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogManager>();
            logger.LogDebug($"Starting database migrations.");

            logger.LogWarn("Enabling the system context.");
            systemContext = scope.ServiceProvider.GetRequiredService<ISystemContext>();
            systemContext.IsSystemContext = true;

            logger.LogDebug("Checking if database is already migrated.");
            var systemSettingService = scope.ServiceProvider.GetRequiredService<ISystemSettingsService>();
            var alreadyMigrated = false;
            try
            {
                alreadyMigrated = await systemSettingService.IsMigrationComplete().ConfigureAwait(false);
            }
            catch
            {
                // intentionally empty to catch the case: for the first time when the app is launched, the db won't probably exist, in that case we assume that db needs to be created and migrated.
            }
            if(alreadyMigrated)
            {
                logger.LogWarn("Database is already migrated. No futher action required.");
                return;
            }

            logger.LogDebug($"Database needs to be migrated. Applying the migrations now.");

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new MigrateDatabaseCommand()).ConfigureAwait(false);

            logger.LogDebug("Database migrations completed.");
        }
        finally
        {
            if (systemContext is not null)
            {
                systemContext.IsSystemContext = false;
            }
        }
    }
}