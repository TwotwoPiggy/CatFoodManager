namespace CatFoodManager.Commons;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize == 0 ? 1 : (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResult() { }

    public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static PagedResult<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var enumerable = source as IList<T> ?? source.ToList();
        var totalCount = enumerable.Count;
        var items = enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }
}
