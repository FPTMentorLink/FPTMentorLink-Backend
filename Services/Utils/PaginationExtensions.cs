using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Services.Utils;

public static class PaginationExtensions
{
    public static async Task<PaginationResult<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> source,
        PaginationParams paginationParams)
    {
        var totalItems = await source.CountAsync();

        var items = await source
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return new PaginationResult<T>(
            items,
            totalItems,
            paginationParams.PageNumber,
            paginationParams.PageSize
        );
    }

    public static async Task<PaginationResult<TResult>> ToPaginatedListAsync<TSource, TResult>(
        this IQueryable<TSource> source,
        PaginationParams paginationParams,
        Func<TSource, TResult> selector)
    {
        var totalItems = await source.CountAsync();

        var items = await source
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(x => selector(x))
            .ToListAsync();

        return new PaginationResult<TResult>(
            items,
            totalItems,
            paginationParams.PageNumber,
            paginationParams.PageSize
        );
    }
    /// <summary>
    /// Use AutoMapper to project the source IQueryable to a paginated list of TResult
    /// </summary>
    /// <param name="source"></param>
    /// <param name="paginationParams"></param>
    /// <param name="configurationProvider"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<PaginationResult<TResult>> ProjectToPaginatedListAsync<TSource, TResult>(
        this IQueryable<TSource> source,
        PaginationParams paginationParams,
        IConfigurationProvider configurationProvider)
    {
        var totalItems = await source.CountAsync();

        var items = await source
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ProjectTo<TResult>(configurationProvider)
            .ToListAsync();

        return new PaginationResult<TResult>(
            items,
            totalItems,
            paginationParams.PageNumber,
            paginationParams.PageSize
        );
    }
}