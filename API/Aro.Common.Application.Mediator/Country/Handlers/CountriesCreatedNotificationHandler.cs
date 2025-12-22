using Aro.Common.Application.Mediator.Country.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Common.Application.Mediator.Country.Handlers;

public class CountriesCreatedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<CountriesCreatedNotification>
{
    public async Task Handle(CountriesCreatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            Ids = notification.Data.Data.Ids.ToList()
        };

        await auditService.Log(new(
                auditActions.CountriesCreated,
                entityTypes.Country,
                string.Empty,
                log), cancellationToken)
            .ConfigureAwait(false);
    }
}
