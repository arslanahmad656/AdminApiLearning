using FluentValidation;

namespace Aro.Admin.Presentation.UI.Extensions
{
    public static class FluentValidationExtensions
    {
        public static Func<object, string, Task<IEnumerable<string>>> GetValidateValue<T>(this IValidator<T> validator) where T : class
        {
            return async (model, property) =>
            {
                var result = await validator.ValidateAsync(
                    ValidationContext<T>.CreateWithOptions(
                        (T)model, x => x.IncludeProperties(property)));

                return result.IsValid
                    ? []
                    : result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
