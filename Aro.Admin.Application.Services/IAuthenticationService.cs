using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services;

public interface IAuthenticationService : IService
{
    Task<CompositeToken> Authenticate(string email, string password, CancellationToken cancellationToken = default);

    Task<bool> Logout(Guid userId, string refreshToken, string accessTokenIdentifier, DateTime accessTokenExpiry, CancellationToken cancellationToken = default);

    Task LogoutAll(Guid userId, CancellationToken cancellationToken = default);
}
