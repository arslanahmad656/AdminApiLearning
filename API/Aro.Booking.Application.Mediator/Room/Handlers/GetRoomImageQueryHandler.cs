using Aro.Booking.Application.Mediator.Common.DTOs;
using Aro.Booking.Application.Mediator.Room.Queries;
using Aro.Common.Application.Services.FileResource;
using Aro.Common.Application.Services.LogManager;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class GetRoomImageQueryHandler(ILogManager<GetRoomImageQueryHandler> logger, IFileResourceService fileResourceService) : IRequestHandler<GetRoomImageQuery, GetGroupEntityImageResponse>
{
    public async Task<GetGroupEntityImageResponse> Handle(GetRoomImageQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Handling GetRoomImageQuery for RoomId: {RoomId}, ImageId: {ImageId}", request.RoomId, request.ImageId);

        var serviceResponse = await fileResourceService.ReadFileById(request.ImageId, cancellationToken).ConfigureAwait(false);

        return new(serviceResponse.Id, serviceResponse.Description, serviceResponse.Content);
    }
}
