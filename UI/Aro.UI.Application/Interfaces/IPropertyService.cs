using Aro.UI.Application.DTOs;

namespace Aro.UI.Application.Interfaces;

public interface IPropertyService
{
    Task<CreatePropertyResponse?> CreatePropertyAsync(CreatePropertyRequest request);
}
