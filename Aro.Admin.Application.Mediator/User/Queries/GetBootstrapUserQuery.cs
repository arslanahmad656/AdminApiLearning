using Aro.Admin.Application.Mediator.User.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Queries;

public record GetBootstrapUserQuery : IRequest<GetBootstrapUserResponse>
{
    public string BootstrapPassword { get; init; } = string.Empty;
}
