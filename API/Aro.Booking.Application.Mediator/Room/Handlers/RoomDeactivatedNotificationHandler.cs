using Aro.Booking.Application.Mediator.Room.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class RoomDeactivatedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<RoomDeactivatedNotification>
{
    public async Task Handle(RoomDeactivatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.DeactivateRoomResponse.Id,
            notification.DeactivateRoomResponse.IsActive
        };

        await auditService.Log(
            new(
                auditActions.RoomDeactivated,
                entityTypes.Room,
                notification.DeactivateRoomResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}

