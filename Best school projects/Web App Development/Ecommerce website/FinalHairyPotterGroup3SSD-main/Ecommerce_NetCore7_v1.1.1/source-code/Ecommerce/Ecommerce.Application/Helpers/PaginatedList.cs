using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Helpers;

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public int PerPageItem { get; }
    public int StartCount { get; }
    public int EndCount { get; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        PerPageItem = pageSize;
        Items = items;
        StartCount = (pageIndex * pageSize) - pageSize + 1;
        EndCount = ((pageIndex - 1) * pageSize) + items.Count();
        //EndCount = count < pageSize ? count : pageIndex * pageSize;
    }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        pageSize = pageSize == 0 ? 10 : pageSize;
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
    {
        pageSize = pageSize == 0 ? 10 : pageSize;
        var count = source.Count();
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    public static PaginatedList<T> CreateStatic(List<T> source, int count, int pageIndex, int pageSize)
    {
        pageSize = pageSize == 0 ? 10 : pageSize;
        var items = source.ToList();

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}
