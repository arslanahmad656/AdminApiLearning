using Aro.Booking.Presentation.Api.DTOs;
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

        RuleFor(m => m.PostalCode)
            .NotEmpty()
            .Length(2, 20);

        RuleFor(m => m.Country)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(m => m.PrimaryContactId)
            .NotEmpty();

        RuleFor(m => m.Logo)
            .NotNull();

        //RuleFor(m => m.IsActive);
    }
}
