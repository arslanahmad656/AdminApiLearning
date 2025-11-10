using Aro.Admin.Application.Mediator.Seed.Commands;
using Aro.Admin.Application.Mediator.Seed.Notifications;
using Aro.Admin.Application.Services.Email;
using Aro.Admin.Application.Services.PermissionSeeder;
using MediatR;

namespace Aro.Admin.Application.Mediator.Seed.Handlers;

public class SeedApplicationCommandHandler
(
    IPermissionSeeder seedService,
    IEmailTemplateSeeder emailTemplateSeeder,
    IMediator mediator
)
: IRequestHandler<SeedApplicationCommand>
{
    public async Task Handle(SeedApplicationCommand request, CancellationToken cancellationToken)
    {
        await seedService.Seed(request.JsonFilePath, cancellationToken).ConfigureAwait(false);
        await emailTemplateSeeder.Seed(request.templatesDirectory, cancellationToken).ConfigureAwait(false);

        await mediator.Publish(new ApplicationSeededNotification(), cancellationToken).ConfigureAwait(false);
    }
}
