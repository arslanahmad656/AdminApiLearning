using Aro.Admin.Application.Mediator.Shared.DTOs;
using Aro.Admin.Application.Mediator.UserRole.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Queries;

public record GetUserRolesQuery(GetUserRolesRequest Data) : IRequest<List<GetRoleResponse>>;
