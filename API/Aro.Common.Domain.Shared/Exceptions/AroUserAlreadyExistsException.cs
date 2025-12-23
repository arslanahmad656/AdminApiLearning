namespace Aro.Common.Domain.Shared.Exceptions;

public class AroUserAlreadyExistsException(string email, Exception? innerException = null) : AroException(
        new ErrorCodes().USER_ALREADY_EXISTS,
        $"User with email '{email}' already exists.",
        innerException)
{
}