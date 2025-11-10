namespace Aro.Common.Domain.Shared.Exceptions;

[Serializable]
public class AroException : Exception
{
    public string ErrorCode { get; } = string.Empty;

    public AroException()
    {
    }

    public AroException(string message)
        : base(message)
    {
    }

    public AroException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public AroException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public AroException(string errorCode, string message, Exception? innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}