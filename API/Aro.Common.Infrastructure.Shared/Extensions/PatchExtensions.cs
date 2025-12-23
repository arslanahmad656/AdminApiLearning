using Aro.Common.Application.Services.LogManager;

namespace Aro.Common.Infrastructure.Shared.Extensions;

public static class PatchExtensions
{
    public static void PatchIfNotNull<TValue, TLogger>(
        this TValue? value,
        Action<TValue> apply,
        ILogManager<TLogger> logger,
        string propertyName)
        where TValue : class
    {
        if (value is not null)
        {
            logger.LogDebug(
                "Patching property {PropertyName} with value: {Value}",
                propertyName,
                value
            );

            apply(value);
        }
    }

    public static void PatchIfNotNull<TValue, TLogger>(
        this TValue? value,
        Action<TValue> apply,
        ILogManager<TLogger> logger,
        string propertyName)
        where TValue : struct
    {
        if (value.HasValue)
        {
            logger.LogDebug(
                "Patching property {PropertyName} with value: {Value}",
                propertyName,
                value.Value
            );

            apply(value.Value);
        }
    }
}