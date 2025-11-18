using Aro.Common.Application.Services;

namespace Aro.Booking.Application.Services.Property;

public interface IPropertyService : IService
{
    Task<CreatePropertyResponse> CreateProperty(CreatePropertyDto propertyDto, CancellationToken cancellationToken = default);
    Task<GetPropertyResponse> GetPropertyById(Guid propertyId, CancellationToken cancellationToken = default);
    Task<List<GetPropertyResponse>> GetPropertiesByGroupId(Guid groupId, CancellationToken cancellationToken = default);
}
