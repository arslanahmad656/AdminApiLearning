namespace Aro.Booking.Application.Mediator.Common.DTOs;

public record GetGroupEntityImageResponse(Guid ImageId, string Description, Stream Image);
