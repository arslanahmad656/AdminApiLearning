using Aro.Admin.Application.Mediator.Group.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Commands;

public record PatchGroupCommand(PatchGroupRequest PatchGroupRequest) : IRequest<PatchGroupResponse>;

