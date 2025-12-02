using Aro.Booking.Application.Mediator.Amenity.Commands;
using Aro.Booking.Application.Mediator.Amenity.Notifications;
using Aro.Booking.Application.Services.Amenity;
using MediatR;
using DeleteAmenityResponse = Aro.Booking.Application.Mediator.Amenity.DTOs.DeleteAmenityResponse;

namespace Aro.Booking.Application.Mediator.Amenity.Handlers;

public class DeleteAmenityCommandHandler(IAmenityService amenityService, IMediator mediator) : IRequestHandler<DeleteAmenityCommand, DeleteAmenityResponse>
{
    public async Task<DeleteAmenityResponse> Handle(DeleteAmenityCommand request, CancellationToken cancellationToken)
    {
        var req = request.DeleteAmenityRequest;
        var res = await amenityService.DeleteAmenity(
            new(
                req.Id
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new DeleteAmenityResponse(
                res.Id
            );
        await mediator.Publish(new AmenityDeletedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
