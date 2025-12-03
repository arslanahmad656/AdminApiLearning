using Aro.Booking.Application.Mediator.Property.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Queries;

public record GetPropertyByGroupAndPropertyIdQuery(GetPropertyByGroupIdAndPropertyIdRequest Request) : IRequest<GetPropertyResponse>;
