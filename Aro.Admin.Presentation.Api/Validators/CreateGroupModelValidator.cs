using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class CreateGroupModelValidator : AbstractValidator<CreateGroupModel>
{
    public CreateGroupModelValidator()
    {
        RuleFor(m => m.GroupName)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.AddressLine1)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.City)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.PostalCode)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.Country)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.PrimaryContactId)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.IsActive)
            .NotNull()
            .NotEmpty();
    }
}
