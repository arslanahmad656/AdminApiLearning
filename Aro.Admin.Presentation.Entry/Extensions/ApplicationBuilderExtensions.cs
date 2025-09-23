using Aro.Admin.Application.Mediator.Seed.Commands;
using Aro.Admin.Infrastructure.Repository.Context;
using Aro.Admin.Presentation.Entry.ServiceInstallers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Aro.Admin.Presentation.Entry.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task SeedDatabase(this IApplicationBuilder app, string jsonPath)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new SeedApplicationCommand(jsonPath));
    }

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

    public static async Task MigrateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AroAdminApiDbContext>();
        await dbContext.Database.MigrateAsync().ConfigureAwait(false);
    }
}