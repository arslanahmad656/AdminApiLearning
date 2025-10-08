namespace Aro.Admin.Domain.Shared.Exceptions;

public class UserNotFoundException(Guid userId, Exception? innerException) : NotFoundException(new ErrorCodes().USER_NOT_FOUND, new EntityTypes().User, userId.ToString(), innerException)
{
    public UserNotFoundException(Guid userId) : this(userId, null)
    {
        
    }
}
