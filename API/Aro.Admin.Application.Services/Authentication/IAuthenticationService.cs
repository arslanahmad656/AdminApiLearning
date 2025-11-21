using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.Authentication;

public interface IAuthenticationService : IService
{
    Task<CompositeToken> Authenticate(string email, string password, CancellationToken cancellationToken = default);

    Task<bool> Logout(Guid userId, string refreshToken, string accessTokenIdentifier, DateTime accessTokenExpiry, CancellationToken cancellationToken = default);

    Task LogoutAll(Guid userId, CancellationToken cancellationToken = default);
}
