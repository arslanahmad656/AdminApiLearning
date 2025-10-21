using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;
using Aro.Admin.Application.Services.DTOs.ServiceResponses.PasswordReset;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public partial class PasswordResetTokenService
{
    private (Guid UserId, string IpAddress, string UserAgent, DateTime Timestamp)? ExtractTokenContext(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token) || !token.Contains('.'))
                return null;

            var parts = token.Split('.', 2);
            if (parts.Length != 2)
                return null;

            var contextBytes = Convert.FromBase64String(parts[0]);
            var contextJson = System.Text.Encoding.UTF8.GetString(contextBytes);
            var tokenData = serializer.Deserialize<TokenContextData>(contextJson);
            
            if (tokenData == null)
                return null;
            
            return (tokenData.UserId, tokenData.IpAddress, tokenData.UserAgent, tokenData.Timestamp);
        }
        catch
        {
            return null;
        }
    }

    private record TokenContextData(
        Guid UserId,
        string IpAddress,
        string UserAgent,
        DateTime Timestamp,
        string RandomPart
    );

    private string GenerateStructuredToken(GenerateTokenParameters parameters, DateTime now)
    {
        // Generate structured token with user context
        var randomPart = randomValueGenerator.GenerateString(passwordResetSettings.TokenLength - 100);
        var tokenData = new TokenContextData(
            parameters.UserId,
            parameters.RequestIp,
            parameters.UserAgent,
            now,
            randomPart
        );
        
        // Create structured token by serializing context and adding random part
        var contextJson = serializer.Serialize(tokenData);
        var contextBytes = System.Text.Encoding.UTF8.GetBytes(contextJson);
        var contextBase64 = Convert.ToBase64String(contextBytes);
        var finalRandomPart = randomValueGenerator.GenerateString(passwordResetSettings.TokenLength - contextBase64.Length);
        var rawToken = $"{contextBase64}.{finalRandomPart}";
        
        return rawToken;
    }
}
