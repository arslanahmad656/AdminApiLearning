using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class RefreshTokenModelValidator : AbstractValidator<RefreshTokenModel>
{
    public RefreshTokenModelValidator()
    {
        RuleFor(m => m.RefreshToken)
            .NotNull()
            .NotEmpty();
    }
}
