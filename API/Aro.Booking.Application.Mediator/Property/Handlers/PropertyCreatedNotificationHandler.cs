using Aro.Booking.Application.Mediator.Property.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Handlers;

public class PropertyCreatedNotificationHandler(
    IAuditService auditService,
    AuditActions auditActions,
    EntityTypes entityTypes
) : INotificationHandler<PropertyCreatedNotification>
{
    public async Task Handle(PropertyCreatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.PropertyId,
            notification.GroupId,
            notification.PropertyName,
            notification.PropertyTypes,
            notification.StarRating,
            notification.Currency
        };

        await auditService.Log(
            new(
                auditActions.PropertyCreated,
                entityTypes.Property,
                notification.PropertyId.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
