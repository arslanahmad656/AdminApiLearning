namespace Aro.Common.Domain.Shared.Exceptions;

public abstract class AroNotFoundException : AroException
{
    protected AroNotFoundException(string errorCode, string entityType, string? entityIdentifier, Exception? innerException)
        : base(errorCode, $"Entity {entityType}{(entityIdentifier is null ? string.Empty : $" identified by {entityIdentifier}")} was not found.", innerException)
    {

    }

    protected AroNotFoundException(string errorCode, string message, Exception? innerException)
        : base(errorCode, message, innerException)
    {

    }
}
