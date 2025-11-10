using Aro.Admin.Application.Mediator.Authentication.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Commands;

public record LogoutUserCommand(LogoutUserRequest Data) : IRequest<bool>;

