using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Shared.Exceptions;

namespace Aro.Admin.Infrastructure.Services;

public partial class AuthenticationService(IPasswordHasher passwordHasher, IUserService userService, ITokenService tokenService, ErrorCodes errorCodes) : IAuthenticationService
{
    public async Task<TokenResponse> Authenticate(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserByEmail(email, false, true, cancellationToken).ConfigureAwait(false);
        var isPasswordCorrect = passwordHasher.Verify(password, user.PasswordHash);

        if (!isPasswordCorrect)
        {
            throw new AroException(errorCodes.INVALID_PASSWORD, $"Invalid password for user {email}.");
        }

        var tokenResponse = await tokenService.GenerateToken(user.Id, cancellationToken).ConfigureAwait(false);

        return tokenResponse;
    }
}
