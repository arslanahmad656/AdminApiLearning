using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services;

public interface IAuthenticationService
{
    Task<TokenResponse> Authenticate(string email, string password, CancellationToken cancellationToken = default);
}
