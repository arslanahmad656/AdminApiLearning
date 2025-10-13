using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class LogoutUserModelValidator : AbstractValidator<LogoutUserModel>
{
    public LogoutUserModelValidator()
    {
        RuleFor(m => m.UserId)
            .NotEmpty()
            .NotEqual(Guid.Empty);

        RuleFor(m => m.RefreshToken)
            .NotEmpty()
            .NotNull();
    }
}
