using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Admin.Domain.Shared.Exceptions;

[Serializable]
public class AroTokenValidationException : AroException
{
    public AroTokenValidationException()
    {
    }

    public AroTokenValidationException(string message)
        : base(message)
    {
    }

    public AroTokenValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public AroTokenValidationException(string errorCode, string message)
        : base(errorCode, message)
    {
    }

    public AroTokenValidationException(string errorCode, string message, Exception? innerException)
        : base(errorCode, message, innerException)
    {
    }
}
