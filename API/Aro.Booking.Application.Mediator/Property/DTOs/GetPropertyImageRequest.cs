namespace Aro.Booking.Application.Mediator.Property.DTOs;

public record GetPropertyImageRequest(Guid GroupId, Guid PropertyId, Guid ImageId);
