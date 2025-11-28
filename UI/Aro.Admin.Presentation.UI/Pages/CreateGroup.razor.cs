using Aro.Admin.Presentation.UI.Extensions;
using Aro.UI.Application.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Aro.Admin.Presentation.UI.Pages
{
    public partial class CreateGroup
    {
        [Parameter] public Guid? GroupId { get; set; } = Guid.Empty;
        private bool IsCreateMode { get; set; } = true;

        #region Models

        // For testing purposes
        //public GroupModel groupModel = new()
        //{
        //    GroupName = "Test group name",
        //    AddressLine1 = "Test address line 1",
        //    AddressLine2 = "Test address line 2",
        //    City = "Test city",
        //    Country = "Ireland",
        //    PostalCode = "F94F590"
        //};
        public GroupModel groupModel = new();
        public GroupModel existingGroupModel = new();

        // For testing purposes
        //public PrimaryContactModel primaryContactModel = new()
        //{
        //    Name = "Jack Coyle",
        //    Email = "jack@mail.com",
        //    CountryCode = "44",
        //    PhoneNumber = "7500458335",
        //    IsEditMode = false
        //};
        public PrimaryContactModel primaryContactModel = new();
        public PrimaryContactModel existingPrimaryContactModel = new();

        public MudForm? groupForm;
        public MudForm? primaryContactForm;

        #endregion

        #region Initialization

        private IEnumerable<string>? countryNames;
        private IEnumerable<string>? countryCodes;

        protected override async Task OnInitializedAsync()
        {
            await CountryMetadataService.InitializeAsync();
            countryNames = CountryMetadataService.GetAllCountryNames();
            countryCodes = CountryMetadataService.GetAllCountryCodes();

            if (GroupId == null || GroupId == Guid.Empty)
                SetCreateMode();
            else
                await SetEditMode();
        }

        protected void SetCreateMode() => IsCreateMode = true;

        protected async Task SetEditMode()
        {
            IsCreateMode = false;
            GroupCreationComplete = false;
            StepTitles[^1] = "Review Changes";

            if (GroupId == null || GroupId == Guid.Empty)
                SetExistingModels();
            else
                await LoadExistingGroup();
        }

        #endregion

        #region Edit Mode Loading

        private async Task LoadExistingGroup()
        {
            try
            {
                var response = await GroupService.GetGroup(new(GroupId!.Value, "PrimaryContact,PrimaryContact.ContactInfo"));

                if (response == null || response.Group.Id == Guid.Empty)
                {
                    NavigationManager?.NavigateTo("/groups");
                    return;
                }

                var eG = response.Group;
                groupModel = new GroupModel
                {
                    Id = eG.Id,
                    GroupName = eG.GroupName,
                    AddressLine1 = eG.AddressLine1,
                    AddressLine2 = eG.AddressLine2,
                    City = eG.City,
                    Country = eG.Country,
                    PostalCode = eG.PostalCode,
                    PrimaryContactId = eG.PrimaryContactId
                };

                primaryContactModel = new PrimaryContactModel()
                {
                    //Name = eG.PrimaryContactName,
                    Email = eG.PrimaryContactEmail,
                    IsEditMode = true
                };

                SetExistingModels();

                // RETRIEVE LOGO FROM BLOB STORAGE
                //if (existingGroup.Logo is not null)
                //{
                //    var base64 = Convert.ToBase64String(existingGroup.Logo);
                //    logoPreview = $"data:image/png;base64,{base64}";
                //}
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading group: {ex.Message}", MudBlazor.Severity.Error);
            }
        }

        private void SetExistingModels()
        {
            existingGroupModel = groupModel with { };
            existingPrimaryContactModel = primaryContactModel with { };
        }

        #endregion

        #region Navigation

        private int ActiveStepIndex { get; set; } = 0;
        public bool GroupCreationComplete { get; private set; }

        private List<string> StepTitles =
        [
            "General Information",
            "Primary Contact Details",
            "Review & Save"
        ];

        private async Task<bool> ValidateStep(int step)
        {
            switch (step)
            {
                case 0:
                    if (groupForm is null)
                    {
                        Snackbar.Add("Form error: group form not initialized", Severity.Error);
                        return false;
                    }

                    await groupForm.Validate();

                    if (!groupForm.IsValid) return false;

                    break;
                case 1:
                    if (primaryContactForm is null)
                    {
                        Snackbar.Add("Form error: primary contact form not initialized", Severity.Error);
                        return false;
                    }

                    if (primaryContactModel.Email == existingPrimaryContactModel.Email) return true;

                    await primaryContactForm.Validate();

                    if (!primaryContactForm.IsValid) return false;
                    break;
                default:
                    break;
            }
            return true;
        }

        private async Task MoveNext() { if (await ValidateStep(ActiveStepIndex)) ActiveStepIndex++; }

        public void GoToPreviousPage()
        {
            if (ActiveStepIndex > 0)
                ActiveStepIndex--;
        }

        public void GoToFirstPage()
        {
            ActiveStepIndex = 0;
        }

        public async Task SwitchToEditView()
        {
            NavigationManager?.NavigateTo($"/group/edit/{groupModel.Id}");

            GoToFirstPage();
            await SetEditMode();
        }

        private void HandleExitGroupCreation() => NavigationManager?.NavigateTo("");

        #endregion

        #region File Upload

        private IBrowserFile? _file;
        private string _fileName = string.Empty;
        private string FileUploadMessage = "No file selected";
        private string? logoPreview;
        private byte[]? _fileBytes;

        private async Task OnFileChanged(IBrowserFile file)
        {
            const long MaxAllowedFileBytes = 5 * 1024 * 1024;

            if (file.Size > MaxAllowedFileBytes)
            {
                FileUploadMessage = "File too large";
                return;
            }

            _file = file;
            _fileName = file.Name;
            FileUploadMessage = file.Name;

            using var ms = new MemoryStream();
            await file.OpenReadStream(MaxAllowedFileBytes).CopyToAsync(ms);

            _fileBytes = ms.ToArray();

            logoPreview = $"data:{file.ContentType};base64,{Convert.ToBase64String(_fileBytes)}";

            StateHasChanged();
        }

        #endregion

        #region Save Logic

        private async Task HandleGroupCreation()
        {
            Snackbar.Clear();

            try
            {
                if (IsCreateMode)
                    await CreateGroupFlow();
                else
                    await HandleUpdateFlow();
            }
            catch (HttpRequestException httpEx)
            {
                Snackbar.Add($"Network/API error: {httpEx.Message}", Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Unexpected error: {ex.Message}", Severity.Error);
            }
        }

        private async Task CreateGroupFlow()
        {
            if (groupModel.PrimaryContactId == null || groupModel.PrimaryContactId == Guid.Empty)
            {
                var user = await UserService.CreateUser(new CreateUserRequest(
                    primaryContactModel.Email,
                    string.Empty,
                    false,
                    primaryContactModel.Name,
                    [],
                    primaryContactModel.CountryCode,
                    primaryContactModel.PhoneNumber
                ));

                if (user == null) return;

                groupModel.PrimaryContactId = user.Id;
                Snackbar.Add($"User {user.Id} successfully created", Severity.Success);
            }

            var createdGroup = await GroupService.CreateGroup(new CreateGroupRequest(
                groupModel.GroupName,
                groupModel.AddressLine1,
                groupModel.AddressLine2,
                groupModel.City,
                groupModel.Country,
                groupModel.PostalCode,
                (Guid)groupModel.PrimaryContactId,
                true
            ));

            if (createdGroup != null)
            {
                groupModel.Id = createdGroup.Id;
                primaryContactModel.IsEditMode = true;
                Snackbar.Add("Group created successfully!", Severity.Success);
                GroupCreationComplete = true;
            }
        }

        private async Task HandleUpdateFlow()
        {
            if (primaryContactModel.Email != existingPrimaryContactModel.Email)
            {
                _ = GetUserByEmail(primaryContactModel.Email!);
            }

            var patch = ModelDiffExtensions.ToPatchGroupRequest(groupModel, existingGroupModel);

            if (patch == null)
            {
                Snackbar.Add("Nothing to update");
            }
            else
            {
                await GroupService.PatchGroup(patch);

                SetExistingModels();
            }
        }

        #endregion

        #region API Operations

        private async Task<GetUserResponse?> GetUserByEmail(string email)
        {
            try
            {
                var user = await UserService.GetUserByEmail(new GetUserByEmailRequest(email));

                //if (user == null)
                //{
                //    Snackbar.Add($"A user with identifier : {email} could not be found. Please enter a valid email address");
                //    return null;
                //}

                groupModel.PrimaryContactId = user!.Id;
                return user;
            }
            catch (Exception ex)
            {
                Snackbar.Add($"An error occured {ex.Message}");
                return null;
            }
        }



        #endregion
    }
}