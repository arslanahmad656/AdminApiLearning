using Aro.Admin.Application.Mediator.Authentication.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Commands;

public record AuthenticateUserCommand(AuthenticateUserRequest Data) : IRequest<AuthenticateUserResponse>;

