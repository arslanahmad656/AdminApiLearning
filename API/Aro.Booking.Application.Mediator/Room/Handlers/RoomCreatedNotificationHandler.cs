using Aro.Booking.Application.Mediator.Room.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class RoomCreatedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<RoomCreatedNotification>
{
    public async Task Handle(RoomCreatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.CreateRoomResponse.Id,
            notification.CreateRoomResponse.RoomName
        };

        await auditService.Log(
            new(
                auditActions.RoomCreated,
                entityTypes.Room,
                notification.CreateRoomResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
