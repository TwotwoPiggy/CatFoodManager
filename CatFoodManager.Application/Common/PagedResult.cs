namespace CatFoodManager.Application.Common;

/// <summary>
/// 分页结果类，用于封装分页查询的结果。
/// Paged result class, used to encapsulate the results of paginated queries.
/// </summary>
/// <typeparam name="T">数据项类型 / Data item type</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 数据项列表。
    /// List of data items.
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = [];

    /// <summary>
    /// 总记录数。
    /// Total count of records.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 当前页码。
    /// Current page number.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每页大小。
    /// Page size.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数。
    /// Total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}
