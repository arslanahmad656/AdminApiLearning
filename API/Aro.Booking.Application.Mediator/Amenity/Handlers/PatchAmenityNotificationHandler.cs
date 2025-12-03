using Aro.Booking.Application.Mediator.Amenity.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Handlers;

public class AmenityPatchedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<AmenityPatchedNotification>
{
    public async Task Handle(AmenityPatchedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.PatchAmenityResponse.Amenity.Id,
            notification.PatchAmenityResponse.Amenity.Name
        };

        await auditService.Log(
            new(
                auditActions.AmenityPatched,
                entityTypes.Amenity,
                notification.PatchAmenityResponse.Amenity.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
