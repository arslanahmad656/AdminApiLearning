using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Admin.Domain.Shared.Exceptions;

public class AroRefreshTokenNotFoundException(string identifier, AroRefreshTokenNotFoundException.IdentifierType IdentifierType, Exception? innerException = null) : AroNotFoundException(
        IdentifierType switch
            {
                IdentifierType.User => new ErrorCodes().NO_AVAILABLE_ACTIVE_REFRESH_TOKEN_FOR_USER,
                IdentifierType.Token => new ErrorCodes().NO_AVAILABLE_ACTIVE_REFRESH_TOKEN,
                _ => throw new ArgumentOutOfRangeException(nameof(IdentifierType), IdentifierType, null)
            },
        IdentifierType switch
            {
                IdentifierType.User => $"No available active refresh token found for user with ID '{identifier}'.",
                IdentifierType.Token => $"No available active refresh token found with token '{identifier}'.",
                _ => throw new ArgumentOutOfRangeException(nameof(IdentifierType), IdentifierType, null)
            }, 
        innerException
        )
{
    public enum IdentifierType { User, Token }
}
