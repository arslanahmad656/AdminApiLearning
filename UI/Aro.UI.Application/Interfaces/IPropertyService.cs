using Aro.UI.Application.DTOs;

namespace Aro.UI.Application.Interfaces;

public interface IPropertyService
{
    /// <summary>
    /// Creates a new property with all wizard data including file uploads.
    /// Sends multipart/form-data to the API.
    /// </summary>
    Task<CreatePropertyResponse?> CreatePropertyAsync(PropertyWizardModel wizardData);

    /// <summary>
    /// Gets all properties for a specific group.
    /// </summary>
    Task<List<PropertyListItemResponse>> GetPropertiesByGroupIdAsync(Guid groupId);

    /// <summary>
    /// Gets a property by its group ID and property ID.
    /// </summary>
    Task<GetPropertyByIdResponse?> GetPropertyByIdAsync(Guid groupId, Guid propertyId);
}
