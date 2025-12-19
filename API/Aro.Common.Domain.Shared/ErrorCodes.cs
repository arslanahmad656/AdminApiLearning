namespace Aro.Common.Domain.Shared;

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
    public readonly string ACCOUNT_LOCKED = nameof(ACCOUNT_LOCKED);
    public readonly string NO_AVAILABLE_ACTIVE_REFRESH_TOKEN_FOR_USER = nameof(NO_AVAILABLE_ACTIVE_REFRESH_TOKEN_FOR_USER);
    public readonly string NO_AVAILABLE_ACTIVE_REFRESH_TOKEN = nameof(NO_AVAILABLE_ACTIVE_REFRESH_TOKEN);
    public readonly string INVALID_REFRESH_TOKEN = nameof(INVALID_REFRESH_TOKEN);
    public readonly string TOKEN_INFO_RETRIEVAL_ERROR = nameof(TOKEN_INFO_RETRIEVAL_ERROR);
    public readonly string INVALID_SYSTEM_ADMIN_PASSWORD = nameof(INVALID_SYSTEM_ADMIN_PASSWORD);
    #endregion

    #region [USERS]
    public readonly string USER_NOT_FOUND = nameof(USER_NOT_FOUND);
    #endregion

    #region [PASSWORD]
    public readonly string PASSWORD_RESET_TOKEN_NOT_FOUND = nameof(PASSWORD_RESET_TOKEN_NOT_FOUND);
    public readonly string PASSWORD_RESET_TOKEN_EXPIRED = nameof(PASSWORD_RESET_TOKEN_EXPIRED);
    public readonly string PASSWORD_RESET_TOKEN_ERROR = nameof(PASSWORD_RESET_TOKEN_ERROR);
    public readonly string PASSWORD_RESET_TOKEN_ALREADY_USED = nameof(PASSWORD_RESET_TOKEN_ALREADY_USED);
    public readonly string PASSWORD_RESET_TOKEN_INVALID = nameof(PASSWORD_RESET_TOKEN_INVALID);
    public readonly string PASSWORD_NOT_ALLOWED_TO_BE_REUSED = nameof(PASSWORD_NOT_ALLOWED_TO_BE_REUSED);
    public readonly string PASSWORD_COMPLEXITY_ERROR = nameof(PASSWORD_COMPLEXITY_ERROR);
    public readonly string OLD_PASSWORD_INVALID = nameof(OLD_PASSWORD_INVALID);
    #endregion

    #region [EMAIL]
    public readonly string EMAIL_SENDING_ERROR = nameof(EMAIL_SENDING_ERROR);
    public readonly string EMAIL_LINK_GENERATION_ERROR = nameof(EMAIL_LINK_GENERATION_ERROR);
    public readonly string EMAIL_TEMPLATE_NOT_FOUND = nameof(EMAIL_TEMPLATE_NOT_FOUND);
    #endregion

    #region [GROUP]
    public readonly string GROUP_NOT_FOUND = nameof(GROUP_NOT_FOUND);
    #endregion

    #region [PROPERTY]
    public readonly string PROPERTY_NOT_FOUND = nameof(PROPERTY_NOT_FOUND);
    public readonly string PROPERTY_ALREADY_EXISTS = nameof(PROPERTY_ALREADY_EXISTS);
    #endregion

    #region [ROOM]
    public readonly string ROOM_NOT_FOUND = nameof(ROOM_NOT_FOUND);
    public readonly string ROOM_ALREADY_EXISTS = nameof(ROOM_ALREADY_EXISTS);
    #endregion

    #region [AMENITY]
    public readonly string AMENITY_NOT_FOUND = nameof(AMENITY_NOT_FOUND);
    public readonly string AMENITY_ALREADY_EXISTS = nameof(AMENITY_ALREADY_EXISTS);
    #endregion

    #region [POLICY]
    public readonly string POLICY_NOT_FOUND = nameof(POLICY_NOT_FOUND);
    #endregion

    #region [FILE MANAGEMENT]
    public readonly string GENERAL_FILE_ERROR = nameof(GENERAL_FILE_ERROR);
    public readonly string FILE_NOT_FOUND = nameof(FILE_NOT_FOUND);
    public readonly string FILE_CREATION_FAILED = nameof(FILE_CREATION_FAILED);
    public readonly string FILE_DELETE_FAILED = nameof(FILE_DELETE_FAILED);
    public readonly string FILE_READ_ERROR = nameof(FILE_READ_ERROR);
    public readonly string FILE_UPDATE_ERROR = nameof(FILE_UPDATE_ERROR);
    #endregion

    #region [ROLE]
    public readonly string ROLE_NOT_FOUND = nameof(ROLE_NOT_FOUND);
    #endregion
}
