using Aro.Common.Application.Shared;

namespace Aro.Booking.Application.Services.Amenity;

public interface IAmenityService : IService
{
    Task<CreateAmenityResponse> CreateAmenity(CreateAmenityDto amenity, CancellationToken cancellationToken = default);

    Task<GetAmenitiesResponse> GetAmenities(GetAmenitiesDto query, CancellationToken cancellationToken = default);

    Task<GetAmenityResponse> GetAmenityById(GetAmenityDto query, CancellationToken cancellationToken = default);

    Task<PatchAmenityResponse> PatchAmenity(PatchAmenityDto amenity, CancellationToken cancellationToken = default);

    Task<DeleteAmenityResponse> DeleteAmenity(DeleteAmenityDto amenity, CancellationToken cancellationToken = default);
}
