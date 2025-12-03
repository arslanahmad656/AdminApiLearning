namespace Aro.Common.Infrastructure.Shared.Extensions;

public static class PatchExtensions
{
    public static void PatchIfNotNull<T>(this T? value, Action<T> apply)
            where T : class
    {
        if (value is not null)
            apply(value);
    }

    public static void PatchIfNotNull<T>(this T? value, Action<T> apply)
        where T : struct
    {
        if (value.HasValue)
            apply(value.Value);
    }
}