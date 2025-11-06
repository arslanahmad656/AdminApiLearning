namespace Aro.Common.Domain.Shared.Exceptions;

public class AroInvalidOperationException(string errorCode, string message, Exception? innerException = null)
    : AroException(errorCode, message, innerException)
{
}
