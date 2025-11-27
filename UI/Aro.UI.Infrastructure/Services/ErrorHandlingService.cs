using Aro.UI.Application.Interfaces;
using MudBlazor;

namespace Aro.UI.Infrastructure.Services;

public class ErrorHandlingService : IErrorHandlingService
{
    private readonly ISnackbar _snackbar;

    public ErrorHandlingService(ISnackbar snackbar)
    {
        _snackbar = snackbar;
    }

    public void ShowError(Exception ex, string defaultMessage = "An unexpected error occurred")
    {
        _snackbar.Clear();

        var message = ex switch
        {
            HttpRequestException => "No internet connection. Please check your network and try again.",
            TaskCanceledException => "Request timed out. Please check your network and try again.",
            _ => defaultMessage
        };

        _snackbar.Add(message, Severity.Error);
    }
}
