namespace Aro.Admin.Domain.Shared.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Merge<T>(this IEnumerable<T>? collection1, IEnumerable<T>? collection2)
    {
        if (collection1 is null && collection2 is null)
            return [];

        if (collection1 is null)
            return collection2!;

        if (collection2 is null)
            return collection1;

        return collection1.Concat(collection2);
    }

    public static IEnumerable<T> Merge<T>(this IEnumerable<T>? collection1, params T[] collection)
        => collection1.Merge((IEnumerable<T>?)collection);
}
