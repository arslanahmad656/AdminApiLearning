using Aro.UI.Application.DTOs.Group;
using Aro.UI.Infrastructure.Services;
using FluentValidation;

namespace Aro.Admin.Presentation.UI.Validators;

public class GroupModelFluentValidator : AbstractValidator<GroupModel>
{
    private readonly ICountryMetadataService _countryMetadataService;

    public GroupModelFluentValidator(ICountryMetadataService countryMetadataService)
    {
        _countryMetadataService = countryMetadataService;

        RuleFor(x => x.GroupName)
            .NotEmpty()
            .Length(5, 100);

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .Length(5, 200);

        RuleFor(x => x.City)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Country)
            .NotEmpty()
            .Length(2, 100)
            .WithMessage("Please select a valid country.");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .Length(2, 20)
            .Must((model, postalCode) =>
            {
                if (string.IsNullOrWhiteSpace(model.Country))
                    return false;

                return countryMetadataService.ValidatePostalCode(model.Country, postalCode);
            })
            .WithMessage("Please enter a valid postal code.");
    }
}

