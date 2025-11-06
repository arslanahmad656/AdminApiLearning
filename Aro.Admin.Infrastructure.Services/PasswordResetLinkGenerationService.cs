using Aro.Admin.Application.Services.Password;
using Aro.Admin.Application.Services.User;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.RequestInterpretor;
using Aro.Common.Domain.Shared;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Infrastructure.Services;

public class PasswordResetLinkGenerationService(IUserService userService, IRequestInterpretorService requestInterpretorService, IPasswordResetTokenService passwordResetTokenService, IOptionsSnapshot<PasswordResetSettings> passwordResetSettings, ErrorCodes errorCodes, ILogManager<PasswordResetLinkGenerationService> logger) : IPasswordResetLinkService
{
    private readonly PasswordResetSettings passwordResetSettings = passwordResetSettings.Value;

    public async Task<Uri> GenerateLink(GenerateLinkParameters parameters, CancellationToken ct = default)
    {
		try
		{
            logger.LogInfo("Starting password reset link generation for user {Email}", parameters.Email);

            var user = await userService.GetUserByEmail(parameters.Email, false, false, ct).ConfigureAwait(false);

            var ipAddress = requestInterpretorService.RetrieveIpAddress()
                ?? throw new InvalidOperationException($"Could not retrieve IP address for password reset link generation for user with email {parameters.Email}.");
            var userAgent = requestInterpretorService.GetUserAgent()
                ?? throw new InvalidOperationException($"Could not retrieve User Agent for password reset link generation for user with email {parameters.Email}.");

            logger.LogInfo("Retrieved user information for password reset link generation. UserId={UserId}, IpAddress={IpAddress}", user.Id, ipAddress);

            var token = await passwordResetTokenService.GenerateToken(new(user.Id, ipAddress, userAgent), ct).ConfigureAwait(false);

            var resetLink = new Uri($"{passwordResetSettings.FrontendResetPasswordUrl}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(parameters.Email)}");

            logger.LogInfo("Successfully generated password reset link for user {Email}. ResetLink={ResetLink}", parameters.Email, resetLink);

            return resetLink;
        }
		catch (Exception ex)
		{
            logger.LogError(ex, "Failed to generate password reset link for user {Email}", parameters.Email);
            throw new AroEmailException(errorCodes.EMAIL_LINK_GENERATION_ERROR, $"Error occurred while generating the password reset email link for user {parameters.Email}.", ex);
		}
    }
}
