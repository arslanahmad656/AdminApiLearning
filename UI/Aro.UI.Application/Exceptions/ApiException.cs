namespace Aro.UI.Application.Exceptions;

public class ApiException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }

    public ApiException(string errorCode, string message, int statusCode = 500)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    public ApiException(string errorCode, string message, int statusCode, Exception? innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}
