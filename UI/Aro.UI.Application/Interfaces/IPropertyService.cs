using Aro.UI.Application.DTOs;

namespace Aro.UI.Application.Interfaces;

public interface IPropertyService
{
    Task<CreatePropertyResponse?> CreatePropertyAsync(CreatePropertyRequest request);

    /// <summary>
    /// Saves property data for any wizard step (create draft or update existing)
    /// </summary>
    Task<SavePropertyResponse?> SavePropertyAsync(SavePropertyRequest request);
}
