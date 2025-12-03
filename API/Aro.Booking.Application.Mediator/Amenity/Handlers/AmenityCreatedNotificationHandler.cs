using Aro.Booking.Application.Mediator.Amenity.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Handlers;

public class AmenityCreatedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<AmenityCreatedNotification>
{
    public async Task Handle(AmenityCreatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.CreateAmenityResponse.Id,
            notification.CreateAmenityResponse.Name
        };

        await auditService.Log(
            new(
                auditActions.AmenityCreated,
                entityTypes.Amenity,
                notification.CreateAmenityResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
