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
