using Aro.Booking.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Booking.Presentation.Api.Validators;

public class GetPoliciesByPropertyRouteParametersValidator : AbstractValidator<GetPoliciesByPropertyRouteParameters>
{
    public GetPoliciesByPropertyRouteParametersValidator()
    {
        RuleFor(p => p.PropertyId)
            .NotEmpty();
    }
}

