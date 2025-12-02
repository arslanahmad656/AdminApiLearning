using Aro.Booking.Application.Mediator.Amenity.Queries;
using Aro.Booking.Application.Services.Amenity;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Handlers;

public class GetAmenitiesCommandHandler(IAmenityService amenityService) : IRequestHandler<GetAmenitiesQuery, DTOs.GetAmenitiesResponse>
{
    public async Task<DTOs.GetAmenitiesResponse> Handle(GetAmenitiesQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await amenityService.GetAmenities(
            new(
                req.Filter,
                req.Include ?? string.Empty,
                req.Page,
                req.PageSize,
                req.SortBy,
                req.Ascending
                ), cancellationToken).ConfigureAwait(false);

        var roomDtos = res.Amenities?
            .Select(r => new DTOs.AmenityDto(
                r.Id,
                r.Name
            ))
            .ToList() ?? [];

        var result = new DTOs.GetAmenitiesResponse(roomDtos, res.TotalCount);

        return result;
    }
}
