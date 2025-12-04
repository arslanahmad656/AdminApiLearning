namespace Aro.Booking.Application.Mediator.Property.DTOs;

public record class GetPropertyByGroupIdAndPropertyIdRequest(Guid GroupId, Guid PropertyId);
