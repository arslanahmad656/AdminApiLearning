namespace Aro.Admin.Domain.Shared.Exceptions;

public class ErrorCodes
{
    public readonly string FILE_NOT_FOUND_ERROR = nameof(FILE_NOT_FOUND_ERROR);
    public readonly string DESERIALIZATION_ERROR = nameof(DESERIALIZATION_ERROR);
    public readonly string DATA_SEED_ERROR = nameof(DATA_SEED_ERROR);
    public readonly string CONFIGURATION_ERROR = nameof(CONFIGURATION_ERROR);
    public readonly string UNKNOWN_ERROR = nameof(UNKNOWN_ERROR);

    #region [SYSTEM SETTINGS]
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
    #endregion

    #region [USERS]
    public readonly string USER_NOT_FOUND = nameof(USER_NOT_FOUND);
    #endregion
}
