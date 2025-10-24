namespace Aro.Admin.Domain.Shared.Exceptions;

public class ErrorCodes
{
    public readonly string FILE_NOT_FOUND_ERROR = nameof(FILE_NOT_FOUND_ERROR);
    public readonly string DESERIALIZATION_ERROR = nameof(DESERIALIZATION_ERROR);
    public readonly string DATA_SEED_ERROR = nameof(DATA_SEED_ERROR);
    public readonly string CONFIGURATION_ERROR = nameof(CONFIGURATION_ERROR);
    public readonly string UNKNOWN_ERROR = nameof(UNKNOWN_ERROR);
    public readonly string OPERATION_CANCELLED_ERROR = nameof(OPERATION_CANCELLED_ERROR);

    #region [SYSTEM SETTINGS]
    public readonly string DATABASE_ALREADY_MIGRATED = nameof(DATABASE_ALREADY_MIGRATED);
    public readonly string SYSTEM_ALREADY_INITIALIZED = nameof(SYSTEM_ALREADY_INITIALIZED);
    public readonly string SYSTEM_INITIALIZATION_ERROR = nameof(SYSTEM_INITIALIZATION_ERROR);
    #endregion

    #region [AUTH]
    public readonly string USER_DOES_NOT_HAVE_PERMISSION = nameof(USER_DOES_NOT_HAVE_PERMISSION);
    public readonly string USER_NOT_AUTHENTICATED = nameof(USER_NOT_AUTHENTICATED);
    public readonly string INVALID_PASSWORD = nameof(INVALID_PASSWORD);
    public readonly string NO_AVAILABLE_ACTIVE_REFRESH_TOKEN_FOR_USER = nameof(NO_AVAILABLE_ACTIVE_REFRESH_TOKEN_FOR_USER);
    public readonly string NO_AVAILABLE_ACTIVE_REFRESH_TOKEN = nameof(NO_AVAILABLE_ACTIVE_REFRESH_TOKEN);
    public readonly string INVALID_REFRESH_TOKEN = nameof(INVALID_REFRESH_TOKEN);
    public readonly string TOKEN_INFO_RETRIEVAL_ERROR = nameof(TOKEN_INFO_RETRIEVAL_ERROR);
    public readonly string INVALID_SYSTEM_ADMIN_PASSWORD = nameof(INVALID_SYSTEM_ADMIN_PASSWORD);
    #endregion

    #region [USERS]
    public readonly string USER_NOT_FOUND = nameof(USER_NOT_FOUND);
    #endregion

    #region [PASSWORD RESET]
    public readonly string PASSWORD_RESET_TOKEN_NOT_FOUND = nameof(PASSWORD_RESET_TOKEN_NOT_FOUND);
    public readonly string PASSWORD_RESET_TOKEN_EXPIRED = nameof(PASSWORD_RESET_TOKEN_EXPIRED);
    public readonly string PASSWORD_RESET_TOKEN_ALREADY_USED = nameof(PASSWORD_RESET_TOKEN_ALREADY_USED);
    public readonly string PASSWORD_RESET_TOKEN_INVALID = nameof(PASSWORD_RESET_TOKEN_INVALID);
    #endregion

    #region [EMAIL]
    public readonly string EMAIL_SENDING_ERROR = nameof(EMAIL_SENDING_ERROR);
    public readonly string EMAIL_LINK_GENERATION_ERROR = nameof(EMAIL_LINK_GENERATION_ERROR);
    #endregion
}
