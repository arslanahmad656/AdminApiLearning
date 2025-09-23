using Aro.Admin.Application.Mediator.Seed.Commands;
using MediatR;

namespace Aro.Admin.Presentation.Entry.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task SeedDatabase(this IApplicationBuilder app, string jsonPath)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new SeedApplicationCommand(jsonPath));
    }
}