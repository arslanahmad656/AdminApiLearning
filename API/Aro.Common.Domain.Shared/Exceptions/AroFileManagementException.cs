namespace Aro.Common.Domain.Shared.Exceptions;

public class AroFileManagementException(AroFileManagementErrorCode errorCode, string? message = null, Exception? innerException = null)
    : AroException
    (
        errorCode: errorCode switch
        {
            AroFileManagementErrorCode.FILE_READ_ERROR => new ErrorCodes().FILE_READ_ERROR,
            AroFileManagementErrorCode.FILE_NOT_FOUND => new ErrorCodes().FILE_NOT_FOUND,
            AroFileManagementErrorCode.FILE_CREATION_FAILED => new ErrorCodes().FILE_CREATION_FAILED,
            AroFileManagementErrorCode.FILE_DELETE_FAILED => new ErrorCodes().FILE_DELETE_FAILED,
            AroFileManagementErrorCode.FILE_UPDATE_ERROR => new ErrorCodes().FILE_UPDATE_ERROR,
            _ => new ErrorCodes().GENERAL_FILE_ERROR,
        },
        message: message ?? "An error occurred in the file management system.",
        innerException: innerException
    )
{
}

public enum AroFileManagementErrorCode
{
    GENERAL_FILE_ERROR,
    FILE_NOT_FOUND,
    FILE_CREATION_FAILED,
    FILE_DELETE_FAILED,
    FILE_READ_ERROR,
    FILE_UPDATE_ERROR,
}
