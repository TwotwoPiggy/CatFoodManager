using System.Linq.Expressions;
using CatFoodManager.Domain.Interfaces;

namespace CatFoodManager.Domain.Specifications;

/// <summary>
/// 规范基类，提供查询规范的通用实现。
/// Specification base class, providing common implementation for query specifications.
/// </summary>
/// <typeparam name="T">实体类型 / Entity type</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    /// <summary>
    /// 查询条件表达式。
    /// Query criteria expression.
    /// </summary>
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <summary>
    /// 包含的导航属性表达式列表。
    /// List of include expressions for navigation properties.
    /// </summary>
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    /// <summary>
    /// 升序排序表达式。
    /// Order by expression.
    /// </summary>
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <summary>
    /// 降序排序表达式。
    /// Order by descending expression.
    /// </summary>
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <summary>
    /// 跳过的记录数。
    /// Number of records to skip.
    /// </summary>
    public int? Skip { get; private set; }

    /// <summary>
    /// 获取的记录数。
    /// Number of records to take.
    /// </summary>
    public int? Take { get; private set; }

    /// <summary>
    /// 是否禁用更改跟踪。
    /// Whether to disable change tracking.
    /// </summary>
    public bool AsNoTracking { get; private set; }

    /// <summary>
    /// 添加查询条件。
    /// Adds query criteria.
    /// </summary>
    /// <param name="criteria">查询条件表达式 / Query criteria expression</param>
    protected void AddCriteria(Expression<Func<T, bool>> criteria) => Criteria = criteria;

    /// <summary>
    /// 添加包含的导航属性。
    /// Adds an include expression.
    /// </summary>
    /// <param name="include">包含表达式 / Include expression</param>
    protected void AddInclude(Expression<Func<T, object>> include) => Includes.Add(include);

    /// <summary>
    /// 应用升序排序。
    /// Applies order by.
    /// </summary>
    /// <param name="orderBy">排序表达式 / Order by expression</param>
    protected void ApplyOrderBy(Expression<Func<T, object>> orderBy) => OrderBy = orderBy;

    /// <summary>
    /// 应用降序排序。
    /// Applies order by descending.
    /// </summary>
    /// <param name="orderByDescending">排序表达式 / Order by descending expression</param>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescending) => OrderByDescending = orderByDescending;

    /// <summary>
    /// 应用分页。
    /// Applies paging.
    /// </summary>
    /// <param name="skip">跳过的记录数 / Number of records to skip</param>
    /// <param name="take">获取的记录数 / Number of records to take</param>
    protected void ApplyPaging(int skip, int take) { Skip = skip; Take = take; }

    /// <summary>
    /// 应用无跟踪模式。
    /// Applies no tracking mode.
    /// </summary>
    protected void ApplyAsNoTracking() => AsNoTracking = true;
}
