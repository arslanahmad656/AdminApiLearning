using Aro.Booking.Application.Mediator.Amenity.Commands;
using Aro.Booking.Application.Mediator.Amenity.Notifications;
using Aro.Booking.Application.Services.Amenity;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Handlers;

public class CreateAmenityCommandHandler(IAmenityService amenityService, IMediator mediator) : IRequestHandler<CreateAmenityCommand, DTOs.CreateAmenityResponse>
{
    public async Task<DTOs.CreateAmenityResponse> Handle(CreateAmenityCommand request, CancellationToken cancellationToken)
    {
        var r = request.CreateAmenityRequest;
        var response = await amenityService.CreateAmenity(
            new CreateAmenityDto(
                r.Name
            ), cancellationToken
        ).ConfigureAwait(false);

        var amenity = response.Amenity;

        var result = new DTOs.CreateAmenityResponse(amenity.Id, amenity.Name);
        await mediator.Publish(new AmenityCreatedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
