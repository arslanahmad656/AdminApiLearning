using Aro.Booking.Application.Mediator.Group.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Commands;

public record DeleteGroupCommand(DeleteGroupRequest DeleteGroupRequest) : IRequest<DeleteGroupResponse>;

