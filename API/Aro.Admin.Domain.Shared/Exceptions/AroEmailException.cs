using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Admin.Domain.Shared.Exceptions;

public class AroEmailException(string errorCode, string message, Exception? innerException) : AroException(errorCode, message, innerException)
{
}
