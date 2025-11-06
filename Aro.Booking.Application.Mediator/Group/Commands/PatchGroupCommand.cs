using Aro.Booking.Application.Mediator.Group.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Commands;

public record PatchGroupCommand(PatchGroupRequest PatchGroupRequest) : IRequest<PatchGroupResponse>;

