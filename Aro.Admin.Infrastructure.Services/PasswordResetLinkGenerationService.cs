using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordLink;
using Aro.Admin.Application.Shared.Options;

namespace Aro.Admin.Infrastructure.Services;

public class PasswordResetLinkGenerationService(IUserService userService, IRequestInterpretorService requestInterpretorService, IPasswordResetTokenService passwordResetTokenService, PasswordResetSettings passwordResetSettings) : IPasswordResetLinkService
{
    public async Task<Uri> GenerateLink(GenerateLinkParameters parameters, CancellationToken ct = default)
    {
        var user = await userService.GetUserByEmail(parameters.Email, false, false, ct).ConfigureAwait(false);

        var ipAddress = requestInterpretorService.RetrieveIpAddress() 
            ?? throw new InvalidOperationException($"Could not retrieve IP address for password reset link generation for user with email {parameters.Email}.");
        var userAgent = requestInterpretorService.GetUserAgent() 
            ?? throw new InvalidOperationException($"Could not retrieve User Agent for password reset link generation for user with email {parameters.Email}.");

        var token = await passwordResetTokenService.GenerateToken(new(user.Id, ipAddress, userAgent), ct).ConfigureAwait(false);

        var resetLink = new Uri($"{passwordResetSettings.FrontendResetPasswordUrl}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(parameters.Email)}");

        return resetLink;
    }
}
