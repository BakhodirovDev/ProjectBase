namespace Domain.Abstraction.Base;

public class PaginationParameters
{
    private const int MaxPageSize = 1000;
    private int _pageSize = 10;
    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? _pageSize : value;
    }

    public string? SortBy { get; set; }
    public bool IsSortAscending { get; set; } = false;
    public string SearchTerm { get; set; } = "";
    public string SortOrder { get; set; } = "asc";
}
