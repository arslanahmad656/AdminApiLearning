using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

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

    public static IQueryable<T> SortBy<T>(this IQueryable<T> query, string sortBy, bool ascending)
    {
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = ascending
                ? query.OrderBy(e => EF.Property<Object>(e, sortBy))
                : query.OrderByDescending(e => EF.Property<Object>(e, sortBy));
        }
        return query;
    }
}