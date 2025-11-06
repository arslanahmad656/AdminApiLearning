using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Admin.Domain.Shared.Exceptions;

public class AroUserPermissionException(Guid userId, string permissionCode, Exception? innerException = null) : AroException(new ErrorCodes().USER_DOES_NOT_HAVE_PERMISSION, $"User {userId} does not have the permission {permissionCode}", innerException)
{
}
