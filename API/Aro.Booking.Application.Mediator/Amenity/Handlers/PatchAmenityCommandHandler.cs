using Aro.Booking.Application.Mediator.Amenity.Commands;
using Aro.Booking.Application.Mediator.Amenity.Notifications;
using Aro.Booking.Application.Services.Amenity;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Handlers;

public class PatchAmenityCommandHandler(IAmenityService amenityService, IMediator mediator) : IRequestHandler<PatchAmenityCommand, DTOs.PatchAmenityResponse>
{
    public async Task<DTOs.PatchAmenityResponse> Handle(PatchAmenityCommand request, CancellationToken cancellationToken)
    {
        var req = request.PatchAmenityRequest.Amenity;
        var res = await amenityService.PatchAmenity(new(new(
                req.Id,
                req.Name
            )), cancellationToken
        ).ConfigureAwait(false);

        var r = res.Amenity;
        var result = new DTOs.PatchAmenityResponse(new(
                r.Id,
                r.Name
            ));

        await mediator.Publish(new AmenityPatchedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
