using Aro.Booking.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Booking.Presentation.Api.Validators;

public class CreatePropertyModelValidator : AbstractValidator<CreatePropertyModel>
{
    public CreatePropertyModelValidator()
    {
        RuleFor(p => p.PropertyName)
            .NotEmpty()
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(p => p.PropertyTypes)
            .NotNull()
            .NotEmpty();

        RuleForEach(p => p.PropertyTypes)
            .NotEqual(Domain.Shared.PropertyTypes.None);

        RuleFor(p => p.StarRating)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(5);

        RuleFor(p => p.Currency)
            .Length(3);

        RuleFor(p => p.Description)
            .NotNull()
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(p => p.KeySellingPoints)
            .Must(points => points != null && points.Count <= 4);

        RuleFor(p => p.MarketingTitle)
            .NotNull()
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(p => p.MarketingDescription)
            .NotNull()
            .NotEmpty()
            .MaximumLength(160);

        RuleFor(p => p.Files)
            .NotNull();
    }
}
