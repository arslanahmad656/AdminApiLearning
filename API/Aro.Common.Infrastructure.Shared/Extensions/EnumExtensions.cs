namespace Aro.Common.Infrastructure.Shared.Extensions;

public static class EnumExtensions
{
    public static T ToFlag<T>(this IEnumerable<T> values) where T : Enum
    {
        long result = 0;
        foreach (var v in values)
            result |= Convert.ToInt64(v);

        return (T)Enum.ToObject(typeof(T), result);
    }

    public static IEnumerable<T> SplitFlags<T>(this T flags) where T : Enum
    {
        long f = Convert.ToInt64(flags);

        foreach (var value in Enum.GetValues(typeof(T)).Cast<T>())
        {
            long v = Convert.ToInt64(value);

            if (v == 0) // ignore None = 0
                continue;

            if ((f & v) == v)
                yield return value;
        }
    }
}
