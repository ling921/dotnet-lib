using LingDev.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LingDev.Linq;

/// <summary>
/// Extension methods for <see cref="IQueryable{TSource}"/>.
/// </summary>
public static class QueryableExtensions
{
    #region Find

    /// <summary>
    /// If condition filters a sequence of values based on a predicate, otherwise not.
    /// </summary>
    /// <param name="source">An <see cref="IQueryable{TSource}"/> to filter.</param>
    /// <param name="id">The identity of <typeparamref name="TSource"/> element.</param>
    /// <param name="cancellationToken">
    /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// <inheritdoc cref="EntityFrameworkQueryableExtensions.FirstOrDefaultAsync{TSource}(IQueryable{TSource}, CancellationToken)"/>
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    public static Task<TSource?> FindByIdAsync<TSource, TKey>(this IQueryable<TSource> source, TKey id, CancellationToken cancellationToken = default)
        where TSource : IEntity<TKey>
        where TKey : notnull
    {
        return source.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    #endregion Find

    #region WhereIf

    /// <summary>
    /// If condition filters a sequence of values based on a predicate, otherwise not.
    /// </summary>
    /// <param name="source">An <see cref="IQueryable{TSource}"/> to filter.</param>
    /// <param name="condition">Condition of filter</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <exception cref="ArgumentNullException"/>
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    #endregion WhereIf

    #region Pagination

    /// <summary>
    /// Asynchronously get paged results.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
    /// <param name="offset">The number of elements to skip before returning the remaining elements.</param>
    /// <param name="limit">The number of elements to return.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>The total number of items to be paged, and items of current active page.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static async ValueTask<(int Total, IEnumerable<TSource> Items)> ToPagedAsync<TSource>(
        this IQueryable<TSource> source,
        int offset = 0,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "The number of elements to skip cannot be less than 0.");
        }
        if (limit < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "The number of elements to return cannot be less than 1.");
        }

        var total = await source.CountAsync(cancellationToken);
        var items = await source.Skip(offset)
                                .Take(limit)
                                .ToListAsync(cancellationToken);

        return (total, items);
    }

    /// <summary>
    /// Asynchronously get paged results, and projects each element of a sequence into a new form.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the elements of result.</typeparam>
    /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <param name="offset">The number of elements to skip before returning the remaining elements.</param>
    /// <param name="limit">The number of elements to return.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>The total number of items to be paged, and items of current active page.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static async ValueTask<(int Total, IEnumerable<TResult> Items)> ToPagedAsync<TSource, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TSource, TResult>> selector,
        int offset = 0,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selector);
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "The number of elements to skip cannot be less than 0.");
        }
        if (limit < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "The number of elements to return cannot be less than 1.");
        }

        var total = await source.CountAsync(cancellationToken);
        var items = await source.Skip(offset)
                                .Take(limit)
                                .Select(selector)
                                .ToListAsync(cancellationToken);

        return (total, items);
    }

    /// <summary>
    /// Asynchronously get paged results, and projects each element of a sequence into a new form.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the elements of result.</typeparam>
    /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
    /// <param name="projector">A projector to project each element of a sequence into a new form.</param>
    /// <param name="offset">The number of elements to skip before returning the remaining elements.</param>
    /// <param name="limit">The number of elements to return.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>The total number of items to be paged, and items of current active page.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static async ValueTask<(int Total, IEnumerable<TResult> Items)> ToPagedAsync<TSource, TResult>(
        this IQueryable<TSource> source,
        Func<IQueryable<TSource>, IQueryable<TResult>> projector,
        int offset = 0,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(projector);
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "The number of elements to skip cannot be less than 0.");
        }
        if (limit < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "The number of elements to return cannot be less than 1.");
        }

        var total = await source.CountAsync(cancellationToken);
        var sourceItems = source.Skip(offset)
                                .Take(limit);
        var items = await projector(sourceItems).ToListAsync(cancellationToken);

        return (total, items);
    }

    #endregion Pagination
}
