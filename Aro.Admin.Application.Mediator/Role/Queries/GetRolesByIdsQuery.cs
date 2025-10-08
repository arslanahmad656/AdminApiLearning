using Aro.Admin.Application.Mediator.Role.DTOs;
using Aro.Admin.Application.Mediator.Shared.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.Role.Queries;

public record GetRolesByIdsQuery(GetRolesByIdRequest Data) : IRequest<List<GetRoleResponse>>;
