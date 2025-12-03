using Aro.Booking.Application.Mediator.Property.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Queries;

public record GetPropertyByIdQuery(GetPropertyByIdRequest Request) : IRequest<GetPropertyResponse>;

