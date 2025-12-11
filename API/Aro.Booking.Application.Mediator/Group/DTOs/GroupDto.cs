namespace Aro.Booking.Application.Mediator.Group.DTOs;

public record GroupDto(
    Guid Id,
    string? GroupName,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? PostalCode,
    string? Country,
    Guid? LogoId,
    Guid? PrimaryContactId,
    string? PrimaryContactName,
    string? PrimaryContactEmail,
    string? PrimaryContactCountryCode,
    string? PrimaryContactPhoneNumber,
    bool? IsActive
);