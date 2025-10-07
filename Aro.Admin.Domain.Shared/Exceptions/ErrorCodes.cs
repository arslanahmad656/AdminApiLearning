namespace Aro.Admin.Domain.Shared.Exceptions;

public class ErrorCodes
{
    public readonly string FILE_NOT_FOUND_ERROR = nameof(FILE_NOT_FOUND_ERROR);
    public readonly string DESERIALIZATION_ERROR = nameof(DESERIALIZATION_ERROR);
    public readonly string DATA_SEED_ERROR = nameof(DATA_SEED_ERROR);

    #region [SYSTEM SETTINGS]
    public readonly string SYSTEM_ALREADY_INITIALIZED = nameof(SYSTEM_ALREADY_INITIALIZED);
    public readonly string SYSTEM_INITIALIZATION_ERROR = nameof(SYSTEM_INITIALIZATION_ERROR);
    #endregion

    #region [AUTH]
    public readonly string USER_DOES_NOT_HAVE_PERMISSION = nameof(USER_DOES_NOT_HAVE_PERMISSION);
    public readonly string USER_NOT_AUTHENTICATED = nameof(USER_NOT_AUTHENTICATED);
    #endregion
}
