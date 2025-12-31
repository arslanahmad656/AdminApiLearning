using Aro.UI.Application.DTOs.Room;

namespace Aro.UI.Infrastructure.Services;

public interface IAmenityService
{
    Task<Amenity?> GetAmenity(Guid amenityId);
    Task<List<Amenity>> GetAmenities(List<Guid> amenityIds);
}
