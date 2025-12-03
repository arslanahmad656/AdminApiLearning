using Aro.Common.Application.Shared;

namespace Aro.Booking.Application.Services.Property;

public interface IPropertyService : IService
{
    Task<CreatePropertyResponse> CreateProperty(CreatePropertyDto propertyDto, CancellationToken cancellationToken = default);
    Task<GetPropertyResponse> GetPropertyById(Guid propertyId, CancellationToken cancellationToken = default);
    Task<GetPropertyResponse> GetPropertyByGroupAndId(Guid groupId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<List<GetPropertyResponse>> GetPropertiesByGroupId(Guid groupId, CancellationToken cancellationToken = default);
}
