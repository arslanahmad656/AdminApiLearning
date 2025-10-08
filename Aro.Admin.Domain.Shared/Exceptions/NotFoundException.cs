namespace Aro.Admin.Domain.Shared.Exceptions;

public abstract class NotFoundException(string errorCode, string entityType, string? entityIdentifier, Exception? innerException) : AroException(errorCode, $"Entity {entityType}{(entityIdentifier is null ? string.Empty : $" identified by {entityIdentifier}")} was not found.", innerException)
{
}
