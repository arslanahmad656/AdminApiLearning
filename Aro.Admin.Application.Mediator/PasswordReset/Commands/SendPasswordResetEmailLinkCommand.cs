using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Commands;

public record SendPasswordResetEmailLinkCommand(SendPasswordResetEmailRequest Data) : IRequest<bool>;
