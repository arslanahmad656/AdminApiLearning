using Aro.Booking.Presentation.Api.DTOs.Group;
using FluentValidation;

namespace Aro.Booking.Presentation.Api.Validators;

public class CreateGroupModelValidator : AbstractValidator<CreateGroupModel>
{
    public CreateGroupModelValidator()
    {
        RuleFor(m => m.GroupName)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(m => m.AddressLine1)
            .NotEmpty()
            .Length(2, 200);

        RuleFor(m => m.City)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(m => m.Country)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(m => m.PostalCode)
            .NotEmpty()
            .Length(2, 20);

        // Primary contact validations

        //RuleFor(m => m.PrimaryContact.Email)
        //    .NotEmpty()
        //    .EmailAddress();

        //RuleFor(m => m.PrimaryContact.Name)
        //    .NotEmpty();

        //RuleFor(m => m.PrimaryContact.CountryCode)
        //    .NotEmpty();

        RuleFor(m => m.Logo)
            .NotNull();

        //RuleFor(m => m.IsActive);
    }
}
