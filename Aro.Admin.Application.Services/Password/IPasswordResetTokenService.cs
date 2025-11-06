namespace Aro.Admin.Application.Services.Password;

public interface IPasswordResetTokenService
{
    Task<string> GenerateToken(GenerateTokenParameters parameters, CancellationToken ct = default);
    Task<ValidateTokenResult> ValidateToken(string tokenHash, CancellationToken ct = default);
    Task MarkTokenUsed(string tokenHash, CancellationToken ct = default);
    Task DeleteExpiredTokens(CancellationToken ct = default);
}
