namespace Aro.Common.Domain.Shared.Exceptions;

public class AroRoleNotFoundException(string roleIdentifier, Exception? innerException) : AroNotFoundException(new ErrorCodes().ROLE_NOT_FOUND, new EntityTypes().Role, roleIdentifier.ToString(), innerException)
{
    public AroRoleNotFoundException(string roleIdentifier) : this(roleIdentifier, null)
    {

    }
}
