namespace Aro.Booking.Application.Mediator.Property.DTOs;

public record GetPropertyImageResponse(Guid ImageId, string Description, Stream Image);
