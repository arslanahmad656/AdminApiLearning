using Aro.Booking.Application.Mediator.Amenity.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Commands;

public record DeleteAmenityCommand(DeleteAmenityRequest DeleteAmenityRequest) : IRequest<DeleteAmenityResponse>;

