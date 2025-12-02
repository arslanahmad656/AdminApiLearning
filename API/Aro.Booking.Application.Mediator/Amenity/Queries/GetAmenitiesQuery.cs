using Aro.Booking.Application.Mediator.Amenity.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Queries;

public record GetAmenitiesQuery(GetAmenitiesRequest Data) : IRequest<GetAmenitiesResponse>;
