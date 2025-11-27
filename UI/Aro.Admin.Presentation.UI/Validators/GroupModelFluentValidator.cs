
using Aro.UI.Application.DTOs;
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
            .Length(5, 100)
            .WithMessage("Group name must be longer than 5 characters.");

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .Length(5, 200)
            .WithMessage("Address must be longer than 5 characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .Length(3, 100)
            .WithMessage("City must be longer than 3 characters.");

        RuleFor(x => x.Country)
            .NotEmpty()
            .Length(2, 100)
            .WithMessage("Please select a valid country.");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .Length(2, 20)
            .Must((model, postalCode) =>
            {
                return true;

                // FIX IMPLEMENTATION
                //if (string.IsNullOrWhiteSpace(model.Country)) return false;
                //return countryMetadataService.ValidatePostalCode(model.Country, postalCode);
            })
            .WithMessage("Please enter a valid postal code.");
    }
}

