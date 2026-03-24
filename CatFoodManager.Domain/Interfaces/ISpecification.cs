using System.Linq.Expressions;

namespace CatFoodManager.Domain.Interfaces;

/// <summary>
/// 规范接口，定义查询规范的标准。
/// Specification interface, defining standards for query specifications.
/// </summary>
/// <typeparam name="T">实体类型 / Entity type</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// 查询条件表达式。
    /// Query criteria expression.
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// 包含的导航属性表达式列表。
    /// List of include expressions for navigation properties.
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// 升序排序表达式。
    /// Order by expression.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// 降序排序表达式。
    /// Order by descending expression.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// 跳过的记录数。
    /// Number of records to skip.
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// 获取的记录数。
    /// Number of records to take.
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// 是否禁用更改跟踪。
    /// Whether to disable change tracking.
    /// </summary>
    bool AsNoTracking { get; }
}
