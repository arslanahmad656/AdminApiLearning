namespace Aro.UI.Application.DTOs;

public record CreatePropertyRequest(
    Guid? GroupId,
    string PropertyName,
    int PropertyTypes,
    int StarRating,
    string Currency,
    string Description
);
