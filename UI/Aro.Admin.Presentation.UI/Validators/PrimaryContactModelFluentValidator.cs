using Aro.UI.Application.DTOs.Group;
using Aro.UI.Infrastructure.Services;
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
            .Length(5, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Please enter a valid email address, e.g., user@example.com.")
            .MustAsync(async (model, email, cancellation) =>
            {
                if (string.IsNullOrEmpty(email)) return false;

                var user = await _userService.UserEmailExists(new(email));

                if (model.Id == Guid.Empty)
                {
                    // Create mode
                    return !user;
                }
                else
                {
                    // Edit mode
                    return user;
                }
            })
            .WithMessage(model => model.Id == Guid.Empty
                ? "A user with this email already exists."
                : "Please enter an existing user's email");


        RuleFor(x => x.CountryCode)
            .NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Must((model, phone) =>
            {
                if (string.IsNullOrWhiteSpace(model.CountryCode))
                    return false;

                return countryMetadataService.ValidateTelephone(model.CountryCode, phone);
            })
            .WithMessage("Invalid phone number for selected country");
    }
}

