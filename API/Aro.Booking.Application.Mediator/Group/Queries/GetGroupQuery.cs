using Aro.Booking.Application.Mediator.Group.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Queries;

public record GetGroupQuery(GetGroupRequest Data) : IRequest<GetGroupResponse>;
