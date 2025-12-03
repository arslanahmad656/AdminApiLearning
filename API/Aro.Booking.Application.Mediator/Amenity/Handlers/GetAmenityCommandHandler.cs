using Aro.Booking.Application.Mediator.Amenity.Queries;
using Aro.Booking.Application.Services.Amenity;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Handlers;

public class GetAmenityCommandHandler(IAmenityService amenityService) : IRequestHandler<GetAmenityQuery, DTOs.GetAmenityResponse>
{
    public async Task<DTOs.GetAmenityResponse> Handle(GetAmenityQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await amenityService.GetAmenityById(
            new(
                req.Id
                ), cancellationToken).ConfigureAwait(false);

        var r = res.Amenity;
        var roomDto = new DTOs.AmenityDto(
            r.Id,
            r.Name
            );

        var result = new DTOs.GetAmenityResponse(roomDto);

        return result;
    }
}
