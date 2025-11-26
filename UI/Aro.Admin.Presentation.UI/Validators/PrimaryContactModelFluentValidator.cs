using Aro.Admin.Presentation.UI.Models;
using Aro.Admin.Presentation.UI.Services;
using FluentValidation;

namespace Aro.Admin.Presentation.UI.Validators;

public class PrimaryContactModelFluentValidator : AbstractValidator<PrimaryContactModel>
{
    private readonly ICountryMetadataService _countryMetadataService;
    private readonly IUserService _userService;

    public PrimaryContactModelFluentValidator(ICountryMetadataService countryMetadataService, IUserService userService)
    {
        _countryMetadataService = countryMetadataService;
        _userService = userService;

        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(5, 100)
            .WithMessage("Name must be longer than 5 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (model, email, cancellation) =>
            {
                if (string.IsNullOrEmpty(email)) return false;

                var user = await _userService.GetUserByEmail(new(email));

                if (model.IsEditMode)
                {
                    return user != null;
                }

                return user == null;
            })
            .WithMessage(model => model.IsEditMode ? "Please enter an existing email" : "A user with this email already exists.");

        RuleFor(x => x.CountryCode)
            .NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must((model, phone) =>
            {
                return true;

                // FIX IMPLEMENTATION
                //if (string.IsNullOrWhiteSpace(model.CountryCode)) return false;
                //return countryMetadataService.ValidateTelephone(model.CountryCode, phone);
            })
            .WithMessage("Invalid phone number for selected country.");
    }
}

