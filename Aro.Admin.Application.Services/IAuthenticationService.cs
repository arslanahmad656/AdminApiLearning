using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services;

public interface IAuthenticationService
{
    Task<CompositeToken> Authenticate(string email, string password, CancellationToken cancellationToken = default);

    Task<bool> Logout(Guid userId, string refreshToken, CancellationToken cancellationToken = default);

    Task LogoutAll(Guid userId, CancellationToken cancellationToken = default);
}
