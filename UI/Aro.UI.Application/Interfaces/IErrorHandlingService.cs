namespace Aro.UI.Application.Interfaces;

/// <summary>
/// Service for handling and displaying errors to users via snackbar notifications
/// </summary>
public interface IErrorHandlingService
{
    /// <summary>
    /// Displays an error message based on the exception type
    /// </summary>
    /// <param name="ex">The exception to handle</param>
    /// <param name="defaultMessage">The default message to display for unexpected exceptions</param>
    void ShowError(Exception ex, string defaultMessage = "An unexpected error occurred");
}
