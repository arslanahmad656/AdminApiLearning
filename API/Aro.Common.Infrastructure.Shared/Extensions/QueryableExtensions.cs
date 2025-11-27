using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Aro.Common.Infrastructure.Shared.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
    {
        if (pageSize > 0)
        {
            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        return query;
    }

    public static IQueryable<T> IncludeElements<T>(this IQueryable<T> query, string include) where T : class
    {
        if (!string.IsNullOrWhiteSpace(include))
        {
            var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var inc in includes)
            {
                query = query.Include(inc);
            }
        }

        return query;
    }

    public static IQueryable<T> SortBy<T>(
        this IQueryable<T> query,
        string? propertyName,
        bool ascending = true)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return query;

        var property = typeof(T).GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) ?? throw new ArgumentException($"'{propertyName}' is not a valid property of {typeof(T).Name}");
        var param = Expression.Parameter(typeof(T), "x");
        var body = Expression.Property(param, property.Name);
        var converted = Expression.Convert(body, typeof(object));
        var keySelector = Expression.Lambda<Func<T, object>>(converted, param);

        return ascending ? query.OrderBy(keySelector)
                         : query.OrderByDescending(keySelector);
    }
}