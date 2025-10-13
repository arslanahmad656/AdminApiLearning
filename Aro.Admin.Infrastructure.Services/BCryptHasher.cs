using Aro.Admin.Application.Services;

namespace Aro.Admin.Infrastructure.Services;

public class BCrypttPasswordHasher(ILogManager<BCrypttPasswordHasher> logger) : IHasher
{
    public string Hash(string text)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Hash));
        
        var hash = BCrypt.Net.BCrypt.HashPassword(text);
        logger.LogDebug("Password hashed successfully");
        
        logger.LogDebug("Completed {MethodName}", nameof(Hash));
        return hash;
    }

    public bool Verify(string text, string hash)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Verify));
        
        var isValid = BCrypt.Net.BCrypt.Verify(text, hash);
        logger.LogDebug("Password verification completed, isValid: {IsValid}", isValid);
        
        logger.LogDebug("Completed {MethodName}", nameof(Verify));
        return isValid;
    }
}
