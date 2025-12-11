namespace Domain.Abstraction.Base;

public class PaginatedResult<T>
{
    public List<T> Items { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public int CurrentPage { get; }
    public int PageSize { get; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public PaginatedResult(List<T> items, int totalCount, int pageSize, int currentPage = 0)
    {
        Items = items ?? new List<T>();
        TotalCount = totalCount;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        CurrentPage = currentPage;
    }
}
