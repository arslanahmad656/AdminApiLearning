using Aro.Booking.Application.Mediator.Amenity.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Handlers;

public class AmenityDeletedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<AmenityDeletedNotification>
{
    public async Task Handle(AmenityDeletedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.DeleteAmenityResponse.Id
        };

        await auditService.Log(
            new(
                auditActions.AmenityDeleted,
                entityTypes.Amenity,
                notification.DeleteAmenityResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
