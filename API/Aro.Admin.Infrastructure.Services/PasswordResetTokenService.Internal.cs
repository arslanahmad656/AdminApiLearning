using Aro.Admin.Application.Services.Password;

namespace Aro.Admin.Infrastructure.Services;

public partial class PasswordResetTokenService
{
    private string GenerateStructuredToken(GenerateTokenParameters parameters, DateTime now)
    {
        // Create a token that embeds the raw UserId, RequestIp, and UserAgent values
        // We'll use a structured approach: contextHash.randomPart

        // Generate random part first with the configured length
        // TokenLength refers to the random part only, not the entire token
        var randomPart = randomValueGenerator.GenerateString(passwordResetSettings.TokenLength);

        // Create context data with the raw values and random part
        var contextData = new TokenContextData(
            parameters.UserId,
            parameters.RequestIp,
            parameters.UserAgent,
            now,
            randomPart
        );

        // Serialize the context data once with everything included
        var contextJson = serializer.Serialize(contextData);
        var contextBytes = System.Text.Encoding.UTF8.GetBytes(contextJson);
        var contextBase64 = Convert.ToBase64String(contextBytes);

        // Create final token: contextBase64.randomPart
        var token = $"{contextBase64}.{randomPart}";

        logger.LogDebug("Generated structured token with embedded context, length: {TokenLength}, userId: {UserId}, ip: {IpAddress}",
            token.Length, parameters.UserId, parameters.RequestIp);

        return token;
    }
}

file record TokenContextData(
        Guid UserId,
        string IpAddress,
        string UserAgent,
        DateTime Timestamp,
        string RandomPart
);
