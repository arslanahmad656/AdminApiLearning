using Aro.Admin.Application.Mediator.SystemSettings.Commands;
using Aro.Admin.Application.Mediator.SystemSettings.DTOs;
using Aro.Admin.Application.Mediator.SystemSettings.Notifications;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Application.Mediator.SystemSettings.Handlers;

public class InitializeSystemCommandHandler(IUserService userService, ISystemSettingsService systemSettingsService, IMediator mediator, ErrorCodes errorCodes,
    IOptions<AdminSettings> adminSettings)
    : IRequestHandler<InitializeSystemCommand, InitializeSystemResponse>
{
    private readonly string adminRoleName = adminSettings.Value.AdminRoleName;
    private readonly string adminPassword = adminSettings.Value.BootstrapPassword;

    public async Task<InitializeSystemResponse> Handle(InitializeSystemCommand request, CancellationToken cancellationToken)
    {
        //TODO: An improvement can be done. If user is authenticated, we can skip the password check since the service is going to check for the permissions anyway.
        if (request.User.BootstrapPassword != adminPassword)
        {
            throw new AroUnauthorizedException(errorCodes.INVALID_SYSTEM_ADMIN_PASSWORD, $"The provided password does not match the required bootstrap password.");
        }

        var isSystemAlreadyInitialized = await systemSettingsService.IsSystemInitialized(cancellationToken).ConfigureAwait(false);
        if (isSystemAlreadyInitialized)
        {
            throw new AroInvalidOperationException(errorCodes.SYSTEM_ALREADY_INITIALIZED, $"Cannot re-initialize the system once it has been initialized.");
        }

            var userForService = new Services.DTOs.ServiceParameters.CreateUserDto(request.User.Email, request.User.PhoneNumber, true, true, request.User.Password, request.User.DisplayName, [adminRoleName]);

        var response = await userService.CreateUser(userForService, cancellationToken).ConfigureAwait(false);

        var result = new InitializeSystemResponse(response.Id.ToString(), response.Email ?? string.Empty, adminRoleName);

        await systemSettingsService.SetSystemStateToInitialized(cancellationToken).ConfigureAwait(false);

        await mediator.Publish(new SystemInitializedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
