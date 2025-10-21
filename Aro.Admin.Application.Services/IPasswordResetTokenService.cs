using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;

namespace Aro.Admin.Application.Services;

public interface IPasswordResetTokenService
{
    Task<string> GenerateToken(GenerateTokenParameters parameters, CancellationToken ct = default);
    Task<bool> ValidateToken(string rawToken, CancellationToken ct = default);
    Task MarkTokenUsed(string rawToken, CancellationToken ct = default);
    Task DeleteExpiredTokens(CancellationToken ct = default);
}
