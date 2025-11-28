namespace Aro.Booking.Application.Mediator.Group.DTOs;

public record GroupDto(
    Guid Id,
    string? GroupName,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? PostalCode,
    string? Country,
    byte[]? Logo,
    Guid? PrimaryContactId,
    string? PrimaryContactName,
    string? PrimaryContactEmail,
    bool? IsActive
);