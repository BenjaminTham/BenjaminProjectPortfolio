using System.Linq.Expressions;

namespace Ecommerce.Application.Extension;
public static class CustomLinq
{
    //public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
    //{
    //    return items.GroupBy(property).Select(x => x.FirstOrDefault());
    //}

    //public static IQueryable<TSource> DistinctBy2<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
    //{
    //    return source.GroupBy(keySelector).Select(x => x.FirstOrDefault());
    //}

    public static async Task<IEnumerable<T>> WhereAsync<T>(this IEnumerable<T> target, Func<T, Task<bool>> predicateAsync)
    {
        var tasks = target.Select(async x => new { Predicate = await predicateAsync(x).ConfigureAwait(false), Value = x }).ToArray();
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        return results.Where(x => x.Predicate).Select(x => x.Value);
    }

    public static IEnumerable<DateTime> Range(this DateTime startDate, DateTime endDate)
    {
        return Enumerable.Range(0, (endDate - startDate).Days + 1).Select(d => startDate.AddDays(d));
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> whereClause)
    {
        if (condition)
        {
            return query.Where(whereClause);
        }
        return query;
    }
}