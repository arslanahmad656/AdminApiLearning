using Aro.Booking.Application.Mediator.Amenity.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Queries;

public record GetAmenityQuery(GetAmenityRequest Data) : IRequest<GetAmenityResponse>;
