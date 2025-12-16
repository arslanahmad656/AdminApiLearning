using FluentValidation;

namespace Aro.Admin.Presentation.UI.Extensions
{
    public static class FluentValidationExtensions
    {
        public static Func<object, string, Task<IEnumerable<string>>> GetValidateValue<T>(this IValidator<T> validator) where T : class
        {
            return async (model, property) =>
            {
                var typedModel = (T)model;
                var normalizedProperty = property?.Split('.').Last();

                // Determine which properties to validate
                IValidationContext context;
                if (string.IsNullOrEmpty(normalizedProperty))
                {
                    // CRITICAL FIX: If 'property' is empty (which happens when MudForm.Validate() is called 
                    // for the whole form), validate the entire model.
                    context = new ValidationContext<T>(typedModel);
                }
                else
                {
                    // Otherwise, validate only the specific property (for real-time field validation).
                    context = ValidationContext<T>.CreateWithOptions(
                        typedModel, x => x.IncludeProperties(normalizedProperty));
                }

                var result = await validator.ValidateAsync(context);

                // Return all error messages associated with the property or the form
                return result.Errors
                    // The filter ensures that errors are shown even during full form validation
                    .Where(e =>
                        string.IsNullOrEmpty(normalizedProperty) ||
                        e.PropertyName == normalizedProperty)
                    .Select(e => e.ErrorMessage);
            };
        }
    }
}