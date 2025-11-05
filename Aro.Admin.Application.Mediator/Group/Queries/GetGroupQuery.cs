using Aro.Admin.Application.Mediator.Group.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Queries;

public record GetGroupQuery(GetGroupRequest Data) : IRequest<GetGroupResponse>;
