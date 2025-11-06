using Aro.Admin.Application.Mediator.Migration.Notifications;
using Aro.Admin.Application.Services.SystemSettings;
using MediatR;

namespace Aro.Admin.Application.Mediator.Migration.Handlers;

public class SetMigratedDatabaseMigratedNotificationHandler(ISystemSettingsService systemSettingsService) : INotificationHandler<DatabaseMigratedNotification>
{
    public async Task Handle(DatabaseMigratedNotification notification, CancellationToken cancellationToken)
    {
        await systemSettingsService.SetMigrationStateToComplete(cancellationToken).ConfigureAwait(false);
    }
}
