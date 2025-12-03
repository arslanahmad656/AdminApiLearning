using Aro.Booking.Application.Mediator.Room.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class RoomPatchedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<RoomPatchedNotification>
{
    public async Task Handle(RoomPatchedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.PatchRoomResponse.Room.Id,
            notification.PatchRoomResponse.Room.RoomName
        };

        await auditService.Log(
            new(
                auditActions.RoomPatched,
                entityTypes.Room,
                notification.PatchRoomResponse.Room.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
