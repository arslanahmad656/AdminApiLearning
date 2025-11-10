using Aro.Admin.Application.Mediator.UserRole.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Commands;

public record RevokeRolesByIdCommand(RevokeRolesByIdRequest Data) : IRequest<RevokeRolesByIdResponse>;

