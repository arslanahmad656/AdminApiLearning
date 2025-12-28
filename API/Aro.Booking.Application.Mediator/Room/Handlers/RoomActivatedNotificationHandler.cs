using Aro.Booking.Application.Mediator.Room.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class RoomActivatedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<RoomActivatedNotification>
{
    public async Task Handle(RoomActivatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.ActivateRoomResponse.Id,
            notification.ActivateRoomResponse.IsActive
        };

        await auditService.Log(
            new(
                auditActions.RoomActivated,
                entityTypes.Room,
                notification.ActivateRoomResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}

