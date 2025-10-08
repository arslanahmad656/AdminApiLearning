using Aro.Admin.Application.Mediator.UserRole.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Queries;

public record UserHasRoleQuery(UserHasRoleRequest Data) : IRequest<bool>;
