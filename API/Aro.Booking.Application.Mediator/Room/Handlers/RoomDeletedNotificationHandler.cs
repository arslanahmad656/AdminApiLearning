using Aro.Booking.Application.Mediator.Room.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class RoomDeletedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<RoomDeletedNotification>
{
    public async Task Handle(RoomDeletedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.DeleteRoomResponse.Id
        };

        await auditService.Log(
            new(
                auditActions.RoomDeleted,
                entityTypes.Room,
                notification.DeleteRoomResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
