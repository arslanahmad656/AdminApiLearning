using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;
using Aro.Admin.Application.Services.DTOs.ServiceResponses.PasswordReset;

namespace Aro.Admin.Application.Services;

public interface IPasswordResetTokenService
{
    Task<string> GenerateToken(GenerateTokenParameters parameters, CancellationToken ct = default);
    Task<ValidateTokenResult> ValidateToken(string tokenHash, CancellationToken ct = default);
    Task MarkTokenUsed(string tokenHash, CancellationToken ct = default);
    Task DeleteExpiredTokens(CancellationToken ct = default);
}
