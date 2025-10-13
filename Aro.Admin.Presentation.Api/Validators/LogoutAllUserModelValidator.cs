using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class LogoutAllUserModelValidator : AbstractValidator<LogoutAllUserModel>
{
    public LogoutAllUserModelValidator()
    {
        RuleFor(m => m.UserId)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
