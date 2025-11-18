namespace Aro.UI.Application.DTOs;

public record CreatePropertyResponse(
    Guid Id,
    Guid? GroupId,
    string PropertyName,
    int PropertyTypes,
    int StarRating,
    string Currency,
    string Description
);
