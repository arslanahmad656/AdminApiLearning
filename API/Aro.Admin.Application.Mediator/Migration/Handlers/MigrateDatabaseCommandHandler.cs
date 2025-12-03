using Aro.Admin.Application.Mediator.Migration.Commands;
using Aro.Admin.Application.Mediator.Migration.Notifications;
using Aro.Admin.Application.Services.Migration;
using MediatR;

namespace Aro.Admin.Application.Mediator.Migration.Handlers;

public class MigrateDatabaseCommandHandler(IMigrationService migrationService, IMediator mediator) : IRequestHandler<MigrateDatabaseCommand>
{
    public async Task Handle(MigrateDatabaseCommand request, CancellationToken cancellationToken)
    {
        await migrationService.Migrate(request.dropFirst, cancellationToken).ConfigureAwait(false);

        await mediator.Publish(new DatabaseMigratedNotification(), cancellationToken).ConfigureAwait(false);
    }
}
