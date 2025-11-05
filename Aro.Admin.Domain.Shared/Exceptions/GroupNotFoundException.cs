namespace Aro.Admin.Domain.Shared.Exceptions;

public class AroGroupNotFoundException(string groupIdenifier, Exception? innerException) : AroNotFoundException(new ErrorCodes().GROUP_NOT_FOUND, new EntityTypes().Group, groupIdenifier.ToString(), innerException)
{
    public AroGroupNotFoundException(string groupIdenifier) : this(groupIdenifier, null)
    {
        
    }
}
