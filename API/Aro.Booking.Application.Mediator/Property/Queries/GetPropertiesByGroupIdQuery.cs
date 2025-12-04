using Aro.Booking.Application.Mediator.Property.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Queries;

public record GetPropertiesByGroupIdQuery(GetPropertiesByGroupIdRequest Request) : IRequest<List<GetPropertyResponse>>;
