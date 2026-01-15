using Aro.Admin.Presentation.UI.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Aro.Admin.Presentation.UI.Services;

public class WizardNavigationService(IDialogService dialogService, NavigationManager navigationManager) : IWizardNavigationService
{
    private readonly IDialogService _dialogService = dialogService;
    private readonly NavigationManager _navigationManager = navigationManager;

    public async Task<bool> NavigateWithUnsavedChangesCheckAsync(
        bool isEditMode,
        bool hasUnsavedChanges,
        Func<Task> saveAction,
        string destinationUrl)
    {
        if (isEditMode && hasUnsavedChanges)
        {
            var parameters = new DialogParameters();
            var options = new DialogOptions { CloseButton = false, MaxWidth = MaxWidth.Small };
            var dialog = await _dialogService.ShowAsync<UnsavedChangesModal>("Unsaved Changes", parameters, options);
            var result = await dialog.Result;

            if (result == null || result.Canceled)
                return false;

            var action = (UnsavedChangesModal.UnsavedChangesResult)result.Data!;
            switch (action)
            {
                case UnsavedChangesModal.UnsavedChangesResult.SaveAndContinue:
                    await saveAction();
                    _navigationManager.NavigateTo(destinationUrl);
                    return true;
                case UnsavedChangesModal.UnsavedChangesResult.Discard:
                    _navigationManager.NavigateTo(destinationUrl);
                    return true;
                case UnsavedChangesModal.UnsavedChangesResult.Cancel:
                    return false;
            }
            return false;
        }

        await saveAction();
        _navigationManager.NavigateTo(destinationUrl);
        return true;
    }
}
