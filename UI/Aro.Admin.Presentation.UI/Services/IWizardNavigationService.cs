namespace Aro.Admin.Presentation.UI.Services;

public interface IWizardNavigationService
{
    Task<bool> NavigateWithUnsavedChangesCheckAsync(
        bool isEditMode,
        bool hasUnsavedChanges,
        Func<Task> saveAction,
        string destinationUrl);
}
